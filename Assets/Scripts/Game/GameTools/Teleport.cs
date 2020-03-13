using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Создает событие телепортации игрока в заданную точку
/// </summary>

public class TeleportPlayerEvent : UnityEvent<Transform>
{
}


[RequireComponent(typeof(BoxCollider))]
public class Teleport : MonoBehaviour
{
    [SerializeField]
    [Tooltip("GameObject  в позицию и поворот которого телепортнется игрок")]
    private Transform toPosition;

    public TeleportPlayerEvent OnTeleportPlayerEvent { get; } = new TeleportPlayerEvent();

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        OnTeleportPlayerEvent.Invoke(toPosition);
        Debug.Log("Teleport");
    }

    private void OnDestroy()
    {
        
        OnTeleportPlayerEvent.RemoveAllListeners();
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
