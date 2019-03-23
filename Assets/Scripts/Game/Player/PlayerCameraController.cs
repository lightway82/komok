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
    [Tooltip("Включить свободновращающуюся камеру")]
    private bool freeRotateCamera;

    public bool FreeRotateCamera
    {
        get { return freeRotateCamera; }
        set
        {
            freeRotateCamera = value;
            initOrbitCamera();
        }
    }

    [SerializeField]
    [Range(0,10)]
    [Tooltip("Высота камеры")]
    private float CamHeight = 1f;
   
    [Range(0.01f, 1)]
    [Tooltip("Демпфер доводки камеры по углу")]
    public float sharpness = 0.1f;
   
    [SerializeField]
    [Range(0,20)]
    [Tooltip("Смещение камеры от игрока. Дистанция ближе которой она не приблизится")]
    public float DistanceFromPlayer = 4f;

    private PlayerController _playerController;

    private Camera camera;
   
    [SerializeField]
    [Range(0,1)]
    [Tooltip("Время за которое камера догонит объект, если он остановится. Сглаживание камеры от скорости.")]
    private float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private Vector3 targetPos;
   
    
    [SerializeField]
    [Tooltip("Скорость вращения камеры в свободном режиме")]
    [Range(0.1f, 10)]
    public float RotationCameraSpeed = 1.5f;

    private float _rotY;
    private float _rotX;
    private Vector3 _offset;
    
   

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
    void Start()
    {
        if (FreeRotateCamera) initOrbitCamera();
    }

    private void Update()
    {

        if (Input.GetAxis("Camera Mode")>0)
        {
            FreeRotateCamera = !freeRotateCamera;
        }
    }

    private void initOrbitCamera()
    {
        _rotY = transform.eulerAngles.y;
        _rotX = transform.eulerAngles.x;
        _offset = Target.position - transform.position;
        
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate ()
    {

        if (FreeRotateCamera) Test();
        else UseTrackingCamera();
    }

    private void UseFreeRotateCamera()
    {
        float horInput = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Mouse Y") != 0)
        {
			
            _rotX += Input.GetAxis("Mouse Y") * RotationCameraSpeed * 3;
            //_rotX = Mathf.Clamp(_rotX, -85, 70);
        }
        if (horInput != 0) {
            _rotY += horInput * RotationCameraSpeed;
        } else {
            _rotY += Input.GetAxis("Mouse X") * RotationCameraSpeed * 3;
        }
        //углы относительно осей, а не вокруг осей
        Quaternion rotation = Quaternion.Euler(0,_rotY, _rotX);
        transform.position = Target.position - rotation * _offset;
        transform.LookAt(Target);
    
    }
    
    private void UseTrackingCamera()
    {
        targetPos = Target.position - _playerController.MoveDirection * DistanceFromPlayer;
        targetPos+=Vector3.Cross(_playerController.MoveDirection, Vector3.left) * CamHeight;//вызывает дергание, возможно устанавливается не верное направление вектора, что заставляет камеру отскакивать назад
        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position,targetPos, ref velocity, smoothTime);
        
        // This constructs a rotation looking in the direction of our target,
        Quaternion targetRotation = Quaternion.LookRotation(Target.position - ThisTransform.position);

        // This blends the target rotation in gradually.
        // Keep sharpness between 0 and 1 - lower values are slower/softer.
        
        ThisTransform.rotation = Quaternion.Lerp(ThisTransform.rotation, targetRotation, sharpness);
    }
    
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;
 
    float x = 0.0f;
    float y = 0.0f;
    
    void Test () 
    {
      
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 
            y = ClampAngle(y, yMinLimit, yMaxLimit);
 
            Quaternion rotation = Quaternion.Euler(y, x, 0);
 
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
 
//            RaycastHit hit;
//            if (Physics.Linecast (Target.position, transform.position, out hit)) 
//            {
//                distance -=  Vector3.Distance(Target.position, transform.position)-hit.distance;
//            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + Target.position;
 
            transform.rotation = rotation;
            transform.position = position;
       
    }
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
