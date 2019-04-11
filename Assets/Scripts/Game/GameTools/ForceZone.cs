using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Задает силу действующую на игрока в зоне коллидера.
/// 
/// </summary>

[RequireComponent(typeof(BoxCollider))]
public class ForceZone : MonoBehaviour
{
    private PlayerController player;
    private Vector3 force;
    private Vector3 forceDirection;
    
    [Header("Создает силу в указанном направлении. В мировой системе.")]
    [Header("На объекте можжет быть несколько коллайдеров.")]
    [Header("Силу создают только с параметром isTrigger")]
    [SerializeField]
    [Tooltip("Режим силы или импульс или постоянная сила")]
    private bool ImpulseMode=false;
    
    [SerializeField]
    [Tooltip("Координата X вектора силы")]
    [Range(-1,1)]
    private float x;
    
    [SerializeField]
    [Tooltip("Координата Y вектора силы")]
    [Range(-1,1)]
    private float y;
    
    [SerializeField]
    [Tooltip("Координата Z вектора силы")]
    [Range(-1,1)]
    private float z=1;

    
    [SerializeField]
    [Tooltip("Величина силы в ньютонах")]
    private float ForceValue;

    private BoxCollider[] _colliders;
    
    
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if(player==null) Debug.LogError("Необходимо добавить игрока в сцену с скриптом PlayerController и тэгом Player");
        forceDirection = transform.TransformVector(new Vector3(x, y, z)).normalized;
        force = forceDirection*ForceValue;
        _colliders = GetComponents<BoxCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ImpulseMode)
        {
            player.AddImpulseForce(force);
        }else  player.AddForce(force);
    }

  
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (!ImpulseMode){
            player.DeleteForce(force);
        }
    }

  


#if UNITY_EDITOR
    
    [Tooltip("Длина вектора силы в редакторе. Для удобтва")]
    public float vectorLength=3;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            var v = forceDirection * vectorLength+transform.position;
            Gizmos.DrawLine(transform.position, v);
            Gizmos.DrawWireSphere(v, 0.05f);
        }
        else
        {
           var v =  transform.TransformVector(new Vector3(x, y, z)).normalized*vectorLength+transform.position;
            Gizmos.DrawLine(transform.position,v);
            Gizmos.DrawWireSphere(v, 0.05f);
        }
        
      
        
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
        
    }
    
#endif
}
