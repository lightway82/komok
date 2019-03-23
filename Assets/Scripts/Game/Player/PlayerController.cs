using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
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
    
    public float SphereRadius { get; private set; }
    
    private Vector3 _totalForce;
    public Vector3 TotalForce => _totalForce;


    public Vector3 MoveDirection => _rigidbody.velocity.normalized;
    //public Vector3 MoveDirection => _moveDirection.normalized;
    public Vector3 Velocity => _rigidbody.velocity;

    private Vector3 _otherForce;

    //private MeshCollider _collider;
    
    
    

    public float SphereMass => _rigidbody.mass;
    
    /// <summary>
    /// Сумма сил сторонних. Силу сюда добавляются и вычитаются через AddForce и DeleteForce
    /// </summary>
    public Vector3 OtherForce => _otherForce;

    private Rigidbody _rigidbody;

    private float _moveHorizontal;
    private float _moveVertical;

    public bool IsOnGround { get; private set; }

    private void Awake()
    {
        //_collider = GetComponent<MeshCollider>();
        _rigidbody =  GetComponent<Rigidbody>();
    }
    
    void Start()
    {

        _rigidbody.mass = CalcSphereMass(SphereStartRadius, SphereDensity);//масссферы от ее радиуса и плотности
        SetupRadius(SphereMass, SphereDensity);
        
        _rigidbody.AddForce(Vector3.forward*StartKickForce, ForceMode.Impulse);//стартовый пиннок под зад

        _lastPos = transform.position;
    }

    private Vector3 _moveDirection;
    private Vector3 _lastPos;
   

    void Update () 
    {
        _moveDirection = (transform.position -_lastPos);
        _lastPos = transform.position;
        _moveHorizontal = Input.GetAxis(HorizontalAxis);
        _moveVertical = Input.GetAxis(VerticalAxis);
        
    }
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


   

    private void FixedUpdate()
    {
        _totalForce = CalculateForcesWithControls() + OtherForce;
        _rigidbody.AddForce(_totalForce);

        if(IsOnGround) _rigidbody.mass += Velocity.magnitude * Time.fixedDeltaTime * DistanceMassMultiplier;
        
        SetupRadius(SphereMass, SphereDensity);
    }

    private void SetupRadius(float mass, float density)
    {
        SphereRadius = CalcSphereRadius(mass, density);
        //_collider.radius = SphereRadius;
        transform.localScale = new Vector3(2*SphereRadius,2*SphereRadius,2*SphereRadius);
    }


    private Vector3 CalculateForcesWithControls()
    {
        
        if (_moveHorizontal == 0 && _moveVertical == 0)
        {
            return MoveDirection*ConstantForce;
        }else if (_moveHorizontal != 0 && _moveVertical == 0)
        {
           return  Vector3.Cross(MoveDirection, Vector3.down).normalized*ConstantForce*RotateMultipier*_moveHorizontal*(IsOnGround?1:InFlyMultiplier);
        }else if (_moveHorizontal == 0 && _moveVertical != 0)
        {
            if (_moveVertical > 0)
            {
              return MoveDirection*ConstantForceMultiplier*ConstantForce*(IsOnGround?1:InFlyMultiplier);
            }
            else
            {
                return MoveDirection*BreakMultipier*ConstantForce*-1*(IsOnGround?1:InFlyMultiplier);
            }
            
        }
        else
        {
           // var vector = (Vector3.Cross(MoveDirection, Vector3.down).normalized+MoveDirection*_moveHorizontal) * _moveHorizontal;
            var vector = Vector3.Cross(MoveDirection, Vector3.down).normalized * _moveHorizontal;
            //ускоренный поворот или поворот с торможением. Сила направляется не влево и в право а + еще на 45 гразусов назад
            if (_moveVertical > 0)
            {
               return vector*ConstantForce*RotateAccelerateMultipier*RotateMultipier*0.5f*(IsOnGround?1:InFlyMultiplier);
            }
            else
            {
               return vector*ConstantForce*RotateBreakMultipier*RotateMultipier*0.5f*(IsOnGround?1:InFlyMultiplier);
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
}
