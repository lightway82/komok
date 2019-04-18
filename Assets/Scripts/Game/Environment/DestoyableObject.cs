using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

   /// <summary>
   /// Базовый клас разрушаемых объектов.
   /// В объекте может быть несколько колидеров в томчисле и в дочерних объектах. Они корректно учитываются
   /// </summary>
   [RequireComponent(typeof(Collider))]
public abstract  class DestoyableObject : MonoBehaviour
{
   [Header("Если объект сложный то можно MeshCollider")]
   [Header("Не поддерживает вложенные объекты с коллайдерами")]
   [Header("Должны быть добавлены коллайдеры c IsTrigger=true")]
   [Header("Компонент сам управляет физикой столкновений")]
  
   [SerializeField]
   [Tooltip("Масса объекта. Если есть Rigidbody, то она ему присвоится")]
   private float Mass = 100;
  
   [SerializeField]
   [Tooltip("Хрупкость объекта. Значение от 0 до 1.  Чем ближе к единице тем более хрупок. Физически обосновонное около 1.0, хотя можно уменьшать есть это нужно для баланса")]
   private float Strength = 0.9f;

   private Collider[] _colliders;
   private PlayerController _player;

   public static readonly int LAYER = 9;

   private void Awake()
   {
      gameObject.layer = LAYER;
      _player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
      if(!_player) Debug.LogError("Отсутствует Player с скриптом PlayerController");
      _colliders = GetComponentsInChildren<Collider>();
      if(_colliders==null) Debug.LogError("На разрушаемых объектах с DestoyableObject нужны коллидеры");
   }

   private void OnTriggerEnter(Collider collider)
   {
      if (collider.gameObject.tag.Equals("Player"))
      {
        
         float dissipation = _player.OnDestroyableTrigger(Mass, Strength, collider);
         if (dissipation >= Strength && _colliders!=null)//если мы остановились, то включаем колайдер на объекте
         {
            foreach (var coll in _colliders) coll.isTrigger = false;
         }
         
         OnTriggerEnterAction(dissipation < Strength);
      }
      
   }

  

  /// <summary>
  /// Дополнительные действия в начале столкновения
  /// </summary>
  /// <param name="destroy">принято решение о разрушении объекта</param>
   public abstract void OnTriggerEnterAction(bool destroy);
   
}
