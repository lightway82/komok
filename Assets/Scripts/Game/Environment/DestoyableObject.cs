using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public abstract  class DestoyableObject : MonoBehaviour
{
   [Header("Должны быть добавлены коллайдеры с isTrigger=true")]
   [Header("Компонент сам управляет физикой столкновений")]
   [Header("На объекте могут быть обычные коллайдеры")]
   [SerializeField]
   [Tooltip("Масса объекта. Если есть Rigidbody, то она ему присвоится")]
   private float Mass = 100;
   
   private Rigidbody _rigidbody;
   private PlayerController _player;

   private void Awake()
   {
      _rigidbody = GetComponent<Rigidbody>();
      if (_rigidbody) _rigidbody.mass = Mass;
      _player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
      if(!_player) Debug.LogError("Отсутствует Player с скриптом PlayerController");
   }


   private void OnTriggerEnter(Collider other)
   {
      Debug.Log("Столкнулись");
      OnTriggerEnterAction(); 
      
   }

   private void OnTriggerExit(Collider other)
   {
      
      OnTriggerExitAction();
   }

   private void OnTriggerStay(Collider other)
   {
      OnTriggerStayAction();
   }

   /// <summary>
   /// Дополнительные действия в начале столкновения
   /// </summary>
   public abstract void OnTriggerEnterAction();
   
   /// <summary>
   /// Дополнительные действия в конце столкновения
   /// </summary>
   public abstract void OnTriggerExitAction();
  
   /// <summary>
   /// Дополнительные действия в процессе столкновения
   /// </summary>
   public abstract void OnTriggerStayAction();
}
