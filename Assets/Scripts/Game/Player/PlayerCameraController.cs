using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    //---------------------------------------------------------------
    //Follow target
    private Transform Target;

    //Reference to local transform
    private Transform ThisTransform;

    //Linear distance to maintain from target (in world units)
    public float DistanceFromTarget = 10.0f;

    //Height of camera above target
    public float CamHeight = 1f;

    //Damping for rotation
    public float RotationDamp = 4f;

    //Damping for position можно создавать эффект приближения и отдаления
    public float PosDamp = 4f;

    private PlayerController _playerController;
    //---------------------------------------------------------------
    void Awake()
    {
        //Get transform for camera
        ThisTransform = GetComponent<Transform>();
        Target =  GameObject.FindWithTag("Player").transform;
        if(Target==null) Debug.LogError("Отсутствует Player");
        _playerController = Target.gameObject.GetComponent<PlayerController>();
        if(_playerController==null) Debug.LogError("У Player отсутствует компонент PlayerController");
    }

    private void Start()
    {
      
    }

    //---------------------------------------------------------------
    // Update is called once per frame
    void LateUpdate () 
    {
     /*   //Get output velocity
        Vector3 Velocity = Vector3.zero;

        //Calculate rotation interpolate
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, Target.rotation, RotationDamp * Time.deltaTime);

        //Get new position
        Vector3 Dest = ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, Target.position, ref Velocity, PosDamp * Time.deltaTime);

        //Move away from target
       ThisTransform.position = Dest - ThisTransform.forward * DistanceFromTarget;

        //Set height
        ThisTransform.position = new Vector3(ThisTransform.position.x, CamHeight, ThisTransform.position.z);

        //Look at dest
        ThisTransform.LookAt(Dest);
        */
       
       

        if (_playerController.moveDirection.magnitude > 0.04)
        {
            // ThisTransform.position = Target.position + moveDirection*30+new Vector3(1,10,1); 
            ThisTransform.position = Vector3.Slerp(ThisTransform.position,
                Target.position 
                - _playerController.moveDirection * PosDamp*_playerController.moveDirection.magnitude + new Vector3(0, 3, 0), PosDamp * Time.deltaTime);   
        }
      
        
        ThisTransform.LookAt(Target);
    }
}
