using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnZoneEvent : UnityEvent<RespawnZone>
{
}

/// <summary>
/// Зона может содержать несколько коллидеров триггеров.
/// Точка возраждения устанавливается из инспектора. Зону можно масштабировать или изменять размер колидера
/// Стартовая зона или зоны всегда активированы, если игрок не дошел до другой, то он появиться в любой из стартовых
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class RespawnZone : MonoBehaviour
{
    [Header("Стартовая зона. Может быть несколько.")]
    [SerializeField]
    [Tooltip("Пометка что это стартовая зона, те первая в начале уровня. Если их несколько, то если игрок не дойдет до новой зоны после старта, то респавнется на случайно выбранной стартовой")]
    private bool startZone;
    
    [Header("Точка респавна игрока. Показываетс шариком, рад.=1")]
    [SerializeField]
    private float spawnPointX;
    [SerializeField]
    private float spawnPointY;
    [SerializeField]
    private float spawnPointZ;
    
    
    public Vector3 SpawnPoint => transform.TransformPoint(new Vector3(spawnPointX, spawnPointY, spawnPointZ));
    
   public RespawnZoneEvent RespawnZoneEvent { get; } = new RespawnZoneEvent();
   

  /// <summary>
  /// Прошел ли игрок через данную зону
  /// </summary>
   public bool IsActivated { get; private set; }

  
  
  /// <summary>
  /// Направление вперед от зоны. Это вектор forward геймобъекта
  /// </summary>
  public Vector3 DirectionZone => transform.forward;

  public bool IsStartZone => startZone;
  
  private void Awake()
  {
      if (startZone) IsActivated = true;
  }

  private void OnDestroy()
  {
     RespawnZoneEvent.RemoveAllListeners();
  }

  private void OnTriggerEnter(Collider other)
   {
      IsActivated = true;
      RespawnZoneEvent.Invoke(this);
   }

  private void OnTriggerExit(Collider other)
  {
      if(IsActivated) return;
      
      IsActivated = true;
      RespawnZoneEvent.Invoke(this);
      
          
  }

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(SpawnPoint, 1);
      Gizmos.DrawRay(SpawnPoint, transform.forward*2);
      
  }

  void OnDrawGizmosSelected()
  {
      OnDrawGizmos();
        
  }
    
#endif
   

}
