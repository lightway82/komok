using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController: MonoBehaviour
{
    
    private Vector3 _lastPos;
    private Vector3 _moveDirection;
    public Vector3 moveDirection => _moveDirection;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody =  GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _lastPos = transform.position;
    }
    
    void Update () 
    {
        _moveDirection = (transform.position -_lastPos).normalized;
        _lastPos = transform.position;
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _moveDirection);

    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
    
#endif
}
