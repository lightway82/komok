using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Активирует Rigidbody у всех объектов
/// </summary>

[RequireComponent(typeof(BoxCollider))]
public class ActivateRigidBodyObjectsZone : MonoBehaviour
{
    [Tooltip("Гейм-обьекты для которых надо активировать rigidbody при проходе данного триггера")]
    [SerializeField]
    private GameObject[] targets;
   
    [Tooltip("Объект на котором весит скрипт, который надо активировать")]
    [SerializeField]
    private GameObject controller;
    
    [Tooltip("Отключить объекты после прохождения зоны, те чтобы они исчезли")]
    [SerializeField]
    private bool switchOf;

    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
   
    private void Awake()
    {
        foreach (var target in targets)
        {
            PrepareRigidBodies(target);

        }
        foreach (var body in _rigidbodies) body.isKinematic=true;
        controller.SetActive(false);
    }

    private void PrepareRigidBodies(GameObject o)
    {
        _rigidbodies.AddRange(o.GetComponents<Rigidbody>());
        for (int i = 0; i < o.transform.childCount; i++)
        {
            PrepareRigidBodies(o.transform.GetChild(i).gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        controller.SetActive(true);
        foreach (var body in _rigidbodies) body.isKinematic=false;
    }

  
    private void OnTriggerExit(Collider other)
    {
        if (switchOf)
        {
            foreach (var target in targets)target.SetActive(false);
        }
    }

  


#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;//уже содержит инф о повороте месе и масштабе
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = matrix;



    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
        
    }
    
#endif
}
