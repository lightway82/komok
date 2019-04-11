using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


public class ShpereDeathEvent: UnityEvent{

}


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RespawnZonesController))]
public class PlayerController: MonoBehaviour
{
    
    [Header("Важно. Массу не задавать! Вычисляется из радиуса.")]
    [Header("Важно. Сферу не масштабировать! Радиус задать ниже!!!.")]
    [Header("Ниже задаем старт. радиус и плотность")]
    [SerializeField]
    [Tooltip("Горизонтальная ось из настроек осей")]
    private string HorizontalAxis="Horizontal";
    
    [SerializeField]
    [Tooltip("Вертикальная ось из настроек осей")]
    private string VerticalAxis="Vertical";
   
    [SerializeField]
    [Range(10,5000)]
    [Tooltip("Значение силы природы в ньютонах. Это постоянная и перенаправляемая управлением сила.")]
    private float ConstantForce=10;
   
    [SerializeField]
    [Range(0.1f,100)]
    [Tooltip("Множитель силы торможения. Умножится на значение силы природы. ")]
    private float BreakMultipier=3;
  
    [SerializeField]
    [Range(0.1f,100)]
    [Tooltip("Множитель ускорения. То во сколько возрастет постоянная силы природы при ускорении шара")]
    private float ConstantForceMultiplier=3;
   
    [SerializeField]
    [Range(0.1f,100)]
    [Tooltip("Множитель силы поворота. Умножится на значение силы природы.")]
    private float RotateMultipier=3;
    
    [SerializeField]
    [Range(0.1f,100)]
    [Tooltip("Множитель силы поворота c ускорением. Сила поворота = сила природы * Множитель силы поворота * этот множитель.")]
    private float RotateAccelerateMultipier=3;
  
    [SerializeField]
    [Range(0.1f,100)]
    [Tooltip("Множитель силы поворота c замедлением. Сила поворота = сила природы * Множитель силы поворота * этот множитель.")]
    private float RotateBreakMultipier=3;
    
    [SerializeField]
    [Range(0.01f,1)]
    [Tooltip("Множитель  уменьшает управляемость в полете")]
    private float InFlyMultiplier=0.1f;
    
    
    [SerializeField]
    [Range(10,5000)]
    [Tooltip("Сила стартового пинка в направлении оси Z для игрока")]
    private float StartKickForce=200;
    
    [SerializeField]
    [Range(0.01f,20)]
    [Tooltip("Коэффициент нарастания массы при движении(зависит от пройденного расстояния). Это постоянная величина, как бы всегда происходящаа. Другие источники массы учитываются отдельно")]
    private float DistanceMassMultiplier=1;
    
    [SerializeField]
    [Range(1,1000)]
    [Tooltip("Плотность сферы. Плотность 12 это для массы 50 кг радиус будет метр ")]
    private float SphereDensity=300;
   
    
    [SerializeField]
    [Range(0.5f,100)]
    [Tooltip("Стартовый радиус сферы")]
    private float SphereStartRadius=1;
    
    [SerializeField]
    [Range(0.1f,20)]
    [Tooltip("Множитель массы. Позволяет управлять зависимостью размера от массы. Физически корректно это 1. Возможно стоит увеличить  чтобы массу слишком не поднимать. Чем он выше тем силнее растем размер от массы(линейно)")]
    private float MassRadiusMultiplaier=1;
    
    
    [SerializeField]
    [Range(0.01f,10)]
    [Tooltip("Множитель сброса массы при ударе. Сброс линейно зависит от угла удара и массы объекта и этого коэфициента. Кратко говоря - это процент от общей массы которая сойдет при лобовом ударе в стену под прямым углом(примерно)")]
    private float DropMassMultiplaier=0.3f;
    
    [SerializeField]
    [Range(0.01f,1)]
    [Tooltip("Процентное соотношение сбрасываемой массы от общей  в секунду при ускорении")]
    private float DropMassAcceleratePersent=0.1f;
    
    [SerializeField]
    [Range(0.01f,1)]
    [Tooltip("Процентное соотношение набираемой массы при торможении от общей масссы в секунду")]
    private float GetMassBreakPersent=0.1f;
    
    
    [SerializeField]
    [Range(0.01f,0.95f)]
    [Tooltip("Процент от стартового радиуса ниже которого будет кончина шара.")]
    private float DeathRadiusPersent=0.5f;
    
    [SerializeField]
    [Tooltip("Максимальная масса")]
    private float MaxMass=2000f;
    
    [SerializeField]
    [Tooltip("Максимальная скорость")]
    private float MaxVelocity=70f;
    
    
    [Tooltip("Событие кончины шара")]
    public ShpereDeathEvent shpereDeathEvent = new ShpereDeathEvent();
    
    
    
    
    
    public float SphereRadius { get; private set; }
    
    private Vector3 _totalForce;
    public Vector3 TotalForce => _totalForce;


    public Vector3 MoveDirection => _rigidbody.velocity.normalized;
    //public Vector3 MoveDirection => _moveDirection.normalized;
    public Vector3 Velocity
    {
        get => _rigidbody.velocity;
        private set => _rigidbody.velocity = value;
    }

    private Vector3 _otherForce;

    //private MeshCollider _collider;
    
    
    

    public float SphereMass
    {
        get => _rigidbody.mass;
        private set
        {
            if (!float.IsNaN(value) && !(value < 1))
            {
                if (value < MaxMass)
                {
                    _rigidbody.mass = value;
                    SetupRadius(SphereMass, SphereDensity);
                }
            }
        }
    }

    /// <summary>
    /// Сумма сил сторонних. Силу сюда добавляются и вычитаются через AddForce и DeleteForce
    /// </summary>
    public Vector3 OtherForce => _otherForce;

    private Rigidbody _rigidbody;

    private float _moveHorizontal;
    private float _moveVertical;

    public bool IsOnGround { get; private set; }

    private GameObject _dynamics;//динамические объекты

    private RespawnZonesController _respawnZonesController;
    
    private void Awake()
    {
        //_collider = GetComponent<MeshCollider>();
        _rigidbody =  GetComponent<Rigidbody>();
        _dynamics = GameObject.FindGameObjectWithTag("DynamicObjects");
        _respawnZonesController = GetComponent<RespawnZonesController>();
        
        foreach (var deathZone in FindObjectsOfType<DeathZone>())
        {
            deathZone.ZoneEvent.AddListener(SphereDeath);
        }
            
    }

    void Start()
    {

        InitSphere();
    }

    private Vector3 _moveDirection;
    private Vector3 _lastPos;

 

    private RespawnZone FindNearRespawn()
    {
       return _respawnZonesController.LastZone;
    }


    private void InitSphere()
    {
        SphereMass = CalcSphereMass(SphereStartRadius, SphereDensity);//масссферы от ее радиуса и плотности
        _rigidbody.AddForce(Vector3.forward*StartKickForce, ForceMode.Impulse);//стартовый пиннок под зад
        _lastPos = transform.position;
        Velocity = Vector3.zero;
    }


    private void Respawn()
    {
        var respawn = FindNearRespawn();
        transform.position = respawn.transform.position;
        transform.rotation = respawn.transform.rotation;
        InitSphere();

    }


    void Update () 
    {
        _moveDirection = (transform.position -_lastPos);
        _lastPos = transform.position;
        _moveHorizontal = Input.GetAxis(HorizontalAxis);
        _moveVertical = Input.GetAxis(VerticalAxis);
    }

    
#if UNITY_EDITOR
    private void OnGUI()
    {
        
       GUI.color=Color.green;
       GUILayout.BeginVertical();
       GUILayout.BeginHorizontal();
       GUILayout.Label("Скорость:");
       GUILayout.Label($"{Velocity.magnitude:f2}");
       GUILayout.EndHorizontal();
       GUILayout.BeginHorizontal();
       GUILayout.Label("Радиус:");
       GUILayout.Label($"{SphereRadius:f2}");
       GUILayout.EndHorizontal();
       GUILayout.BeginHorizontal();
       GUILayout.Label("Радиус разрушения:");
       GUILayout.Label($"{DeathRadiusPersent:f2}");
       GUILayout.EndHorizontal();
       GUILayout.BeginHorizontal();
       GUILayout.Label("Масса:");
       GUILayout.Label($"{SphereMass:f2}");
       GUILayout.EndHorizontal();
       
       GUILayout.EndVertical();
       
        
        
    }
#endif

/*
 Можно  добавлять силы и применять их всегда в fixedupdate, в других местахтолько значение и вектор считать.
 Можно использовать вектор скорости, как обратную связь по силе.Использовать направление и значение.
 Можно корректировать силу исходя из реакции вектора скорости
 */
   
    private void OnCollisionStay(Collision other)
    {
        
        IsOnGround = true;
    }
    
    private void OnCollisionExit(Collision other)
    {
        IsOnGround = false;

    }

    private void OnCollisionEnter(Collision other)
    {
        var terrain = other.gameObject.GetComponent<TerrainCollider>();
        Vector3 velocity = other.relativeVelocity;

        DropMassByFall(other.impulse);
        
    }


    private void SphereDeath()
    {
        Respawn();
    }

    private void DropMassByFall(Vector3 impulse)
    {
        var dropped = Mathf.Abs(Vector3.Dot(impulse.normalized * -1, _moveDirection.normalized)) * SphereMass *
                      DropMassMultiplaier;
        SphereMass -= dropped;
       // Debug.Log("Dropped "+dropped);

    }

    private void DropMassByAccelerate()
    {
        if(IsOnGround) SphereMass -= SphereMass*DropMassAcceleratePersent*Time.deltaTime; 
    }
    
    private void GetMassByAccelerate()
    {
        if(IsOnGround) SphereMass += SphereMass*GetMassBreakPersent*Time.deltaTime; 
    }


    
   /// <summary>
   /// Метод вызывает объект с которым столкнулся игрок, чтобы узначть что делать дальше и инициировать сброс массы и с корости игрока
   /// </summary>
   /// <param name="destroyableMass">Масса объекта</param>
   /// <param name="strength">прочность объекта</param>
   /// <returns>Степень поглощенной энергии от начальной</returns>
    public float OnDestroyableTrigger(float destroyableMass, float strength, Collider other)
   {
       float normalizedImpactAngle = 0f;//должно влиять на расчет, тк учестьнадо касательные удары
       Vector3 reflectDirection = Vector3.zero;//повлияет на направление при разрушении. При не разрушении там колидер повлияет
       if (Physics.SphereCast(new Ray(transform.position-MoveDirection*SphereRadius, MoveDirection), SphereRadius*0.9f, out var hit, SphereRadius * 3, 1<<9, QueryTriggerInteraction.Collide))
       {
           normalizedImpactAngle = Mathf.Abs(Vector3.Dot(-hit.normal, MoveDirection));
           reflectDirection = Vector3.Reflect(MoveDirection, hit.normal);
       }
       
         Vector3 resultVelocity = SphereMass*Velocity/(destroyableMass+SphereMass);
         float deltaE = resultVelocity.magnitude * 0.5f * Velocity.magnitude * destroyableMass*normalizedImpactAngle;
         float E1 = SphereMass * Mathf.Pow(Velocity.magnitude, 2) / 2;

         float resultMass = SphereMass - 2 * deltaE / Mathf.Pow(Velocity.magnitude, 2);
         float dissipationRelation = deltaE/E1;//часть энергии потерянная на разрушение сферы

         //считаем что игрок остановился, если энергия потери значительная и отношение потери к изначальной больше заданного в объекте
         //иначе объект разрушается, те считаем что шар проходит сквозь объект с потерей массы и скорости.
         //масса тут теряется чуть читерски, считаем что при неупругом столкновении вся погашеная энергия идет на потерю массы. Скорость и энергия получается с учетом того что оба объекта слиплись, а потом отлепился от этого основной шар.
        
            Debug.Log("Масса старая "+SphereMass +"Новая "+ resultMass+" dissipation "+dissipationRelation);
         SphereMass = resultMass;
         if (dissipationRelation > strength)
         {
             Velocity = reflectDirection*resultVelocity.magnitude;
             
         }
         else
         {
             Velocity = resultVelocity;
         }
        return dissipationRelation;
    }
    
    
    private void FixedUpdate()
    {
        _totalForce = CalculateForcesWithControls() + OtherForce;
        _rigidbody.AddForce(_totalForce);
        if ( Velocity.sqrMagnitude > MaxVelocity*MaxVelocity) Velocity = Velocity.normalized * MaxVelocity;//ограничение скорости
        //набор массы от движения только при движении по земле и без ускорения и замедления 
        if (IsOnGround && !IsAccelerateOrBreak())
        {
            SphereMass += Velocity.magnitude * Time.fixedDeltaTime * DistanceMassMultiplier;
        }
        
        
            // SetupRadius(SphereMass, SphereDensity);//пересчитаем радиус через массу и плотность. После всех изменений

      
    }

    public bool IsAccelerate() => _moveVertical > 0;
    public bool IsBreak() => _moveVertical < 0;

    public bool IsAccelerateOrBreak() => _moveVertical > 0 || _moveVertical < 0;

    private void SetupRadius(float mass, float density)
    {
        SphereRadius = CalcSphereRadius(mass, density);
        //_collider.radius = SphereRadius;
        transform.localScale = new Vector3(2*SphereRadius,2*SphereRadius,2*SphereRadius);
        if(SphereRadius< SphereStartRadius*DeathRadiusPersent) SphereDeath();
    }


    private Vector3 CalculateForcesWithControls()
    {
        //наш player вращается, как следствие мы ведем расчеты в мировых координатах.
        if (_moveHorizontal == 0 && _moveVertical == 0)
        {
            return MoveDirection * ConstantForce;
        }
        else if (_moveHorizontal != 0 && _moveVertical == 0)
        {
            return Vector3.Cross(MoveDirection, Vector3.down).normalized * ConstantForce * RotateMultipier *
                   _moveHorizontal * (IsOnGround ? 1 : InFlyMultiplier);
        }
        else if (_moveHorizontal == 0 && _moveVertical != 0)
        {
            if (_moveVertical > 0)
            {
                DropMassByAccelerate();
                return MoveDirection * ConstantForceMultiplier * ConstantForce * (IsOnGround ? 1 : InFlyMultiplier);
            }
            else
            {
                GetMassByAccelerate();
                return MoveDirection * BreakMultipier * ConstantForce * -1 * (IsOnGround ? 1 : InFlyMultiplier);
            }

        }
        else
        {
            // var vector = (Vector3.Cross(MoveDirection, Vector3.down).normalized+MoveDirection*_moveHorizontal) * _moveHorizontal;
            var vector = Vector3.Cross(MoveDirection, Vector3.down).normalized * _moveHorizontal;
            //ускоренный поворот или поворот с торможением. Сила направляется не влево и в право а + еще на 45 гразусов назад
            if (_moveVertical > 0)
            {
                DropMassByAccelerate();
                return vector * ConstantForce * RotateAccelerateMultipier * RotateMultipier * 0.5f *
                       (IsOnGround ? 1 : InFlyMultiplier);
            }
            else
            {
                GetMassByAccelerate();
                return vector * ConstantForce * RotateBreakMultipier * RotateMultipier * 0.5f *
                       (IsOnGround ? 1 : InFlyMultiplier);
            }

        }
    }





    private readonly float sqrt_koeff = 1.0f / 3.0f;
   
    private float CalcSphereRadius(float mass, float density)
    {
        return Mathf.Pow(mass / (density * 4.19f), sqrt_koeff)*MassRadiusMultiplaier;
    }
    
    
    private float CalcSphereMass(float radius, float density)
    {
        return Mathf.Pow(radius, 3)*density*4.19f/MassRadiusMultiplaier;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, MoveDirection);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, TotalForce);
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
        
    }
    
#endif


    /// <summary>
    /// Применить импульс
    /// </summary>
    /// <param name="force"></param>
    public void AddImpulseForce(Vector3 force)
    {
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// Добавить постоянную силу
    /// </summary>
    /// <param name="force"></param>
    public void AddForce(Vector3 force)
    {
        _otherForce += force;
    }
    
    /// <summary>
    /// Убрать постоянную силу
    /// </summary>
    /// <param name="force"></param>
    public void DeleteForce(Vector3 force)
    {
        _otherForce -= force;
    }


    private void OnDestroy()
    {
        
        shpereDeathEvent.RemoveAllListeners();
    }
}
