using UnityEngine;
using UnityEngine.Events;


public class DeathZoneEvent: UnityEvent{}


[RequireComponent(typeof(BoxCollider))]
public class DeathZone : MonoBehaviour
{
    public DeathZoneEvent ZoneEvent { get; } = new DeathZoneEvent();


   

    private void OnTriggerEnter(Collider other)
    {
        
        ZoneEvent.Invoke();
    }

    private void OnDestroy()
    {
        ZoneEvent.RemoveAllListeners();
    }
}
