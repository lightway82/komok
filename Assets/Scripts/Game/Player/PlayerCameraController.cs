using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCameraController : MonoBehaviour
{
    //---------------------------------------------------------------
    //Follow target
    private Transform Target;

    //Reference to local transform
    private Transform ThisTransform;

    

    [SerializeField]
    [Range(0,10)]
    [Tooltip("Высота камеры")]
    private float CamHeight = 1f;
   

   
    [SerializeField]
    [Range(0,20)]
    [Tooltip("Смещение камеры от игрока. Дистанция ближе которой она не приблизится")]
    public float DistanceFromPlayer = 4f;

    private PlayerController _playerController;

    private Camera camera;
    
    
    //---------------------------------------------------------------
    void Awake()
    {
        camera = GetComponent<Camera>();
        //Get transform for camera
        ThisTransform = camera.transform;
        Target =  GameObject.FindWithTag("Player").transform;
        if(Target==null) Debug.LogError("Отсутствует Player");
        _playerController = Target.gameObject.GetComponent<PlayerController>();
        if(_playerController==null) Debug.LogError("У Player отсутствует компонент PlayerController");
    }

    private void Start()
    {
      
    }
    [SerializeField]
    [Range(0,1)]
    [Tooltip("Время за которое камера догонит объект, если он остановится. Сглаживание камеры от скорости.")]
    private float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private Vector3 targetPos;
    //---------------------------------------------------------------
    // Update is called once per frame
    void LateUpdate () 
    {
        
//        if (_playerController.Velocity.magnitude > 0.04)
//        {
//            // ThisTransform.position = Target.position + moveDirection*30+new Vector3(1,10,1); 
//            ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position,
//                Target.position - _playerController.MoveDirection * PosDamp + new Vector3(0, 3, 0),
//                
//                ref velocity, PosDamp * Time.deltaTime);   
//        }

       // if (_playerController.MoveDirection.magnitude > 0.04)
        //{
        //ThisTransform.position = Vector3.Lerp(ThisTransform.position, Target.position, 0.1f);

       // }
        targetPos = Target.position - _playerController.MoveDirection * DistanceFromPlayer;
        targetPos+=Vector3.Cross(_playerController.MoveDirection, Vector3.left) * CamHeight;//вызывает дергание, возможно устанавливается не верное направление вектора, что заставляет камеру отскакивать назад
        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position,targetPos, ref velocity, smoothTime);
        
      
        
        // This constructs a rotation looking in the direction of our target,
        Quaternion targetRotation = Quaternion.LookRotation(Target.position - ThisTransform.position);

        // This blends the target rotation in gradually.
        // Keep sharpness between 0 and 1 - lower values are slower/softer.
        
        ThisTransform.rotation = Quaternion.Lerp(ThisTransform.rotation, targetRotation, sharpness);
    }
    
    [Range(0.01f, 1)]
    public float sharpness = 0.1f;
}
