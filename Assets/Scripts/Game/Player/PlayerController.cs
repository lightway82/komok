using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController: MonoBehaviour
{
    [SerializeField]
    [Tooltip("Горизонтальная ось из настроек осей")]
    private string HorizontalAxis="Horizontal";
    
    [SerializeField]
    [Tooltip("Вертикальная ось из настроек осей")]
    private string VerticalAxis="Vertical";
   
    [SerializeField]
    [Range(10,1000)]
    [Tooltip("Значение силы природы в ньютонах. Это постоянная и перенаправляемая управлением сила.")]
    private float ConstantForce=10;
   
    [SerializeField]
    [Range(10,1000)]
    [Tooltip("Значение силы торможения в ньютонах. ")]
    private float BreakForce=500;
    
    [SerializeField]
    [Range(10,1000)]
    [Tooltip("Сила стартового пинка в направлении оси Z для игрока")]
    private float StartKickForce=200;
    
    [SerializeField]
    [Range(1,5)]
    [Tooltip("Множитель ускорения. То во сколько возрастет постоянная силы природы при ускорении шара")]
    private float ConstantForceMultiplier=3;
   
    [SerializeField]
    [Range(0.01f,1)]
    [Tooltip("Множитель  уменьшает управляемость в полете")]
    private float InFlyMultiplier=0.1f;
   
    public Vector3 MoveDirection => _rigidbody.velocity.normalized;
    //public Vector3 MoveDirection => _moveDirection.normalized;
    public Vector3 Velocity => _rigidbody.velocity;
    
    private Rigidbody _rigidbody;

    private float _moveHorizontal;
    private float _moveVertical;

    public bool IsOnGround { get; private set; }

    private void Awake()
    {
        _rigidbody =  GetComponent<Rigidbody>();
    }
    
    void Start()
    {
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

        CalculateForcies();
        
        
    }


    private void CalculateForcies()
    {
        
        if (_moveHorizontal == 0 && _moveVertical == 0)
        {
            _rigidbody.AddForce(MoveDirection*ConstantForce, ForceMode.Force);
        }else if (_moveHorizontal != 0 && _moveVertical == 0)
        {
            _rigidbody.AddForce( Vector3.Cross(MoveDirection, Vector3.down).normalized*ConstantForce*_moveHorizontal*(IsOnGround?1:InFlyMultiplier), ForceMode.Force);
        }else if (_moveHorizontal == 0 && _moveVertical != 0)
        {
            if (_moveVertical > 0)
            {
                _rigidbody.AddForce(MoveDirection*ConstantForce*ConstantForceMultiplier*(IsOnGround?1:InFlyMultiplier), ForceMode.Force);
            }
            else
            {
                _rigidbody.AddForce(MoveDirection*BreakForce*-1*(IsOnGround?1:InFlyMultiplier), ForceMode.Force);
            }
            
        }
        else
        {
            _rigidbody.AddForce( Vector3.Cross(MoveDirection, Vector3.down).normalized*ConstantForce*_moveHorizontal*ConstantForceMultiplier*(IsOnGround?1:InFlyMultiplier), ForceMode.Force);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, MoveDirection);
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
        
    }
    
#endif
}
