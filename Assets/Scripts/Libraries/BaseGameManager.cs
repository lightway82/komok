using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;





/// <summary>
/// Интерфефс, который должен реализовать любой класс менеджера. Маркерный интерфейс
/// Должен существовать контроллер объекта, который может обращаться к менеджеру. Менеджер об контролере и объекте ничего не знает и не должен! Менеджер может дергать другие менеджеры те если в контроллере мы вызываем убить персонажа, то в менеджере вызовем его аналог,далее именно он вызовет все что надо на других менеджерах, что в свою очередь может вызвать волну событий, если кто подписан на них. Если менеджер заинтересовать в изменении свойства другого, то нужно использовать события, а не вызывать на прямую методы из следящего. те менеджеры могут воспользоваться функциями др менеджеров, как сервисом,но не пытаться инициировать, действия в других местах напрямую. Например смерть персонажа - можно выставить эвент просто. Желательно, чтобы менеджер занимался только своими делами - те у наго была своя зна ответственности и он не лез в чужие. Чтобы не пораждать сильной связанности.
/// </summary>
public interface IManager
{
	
}

//TODO: в новой версии unity ввести Enum в ограничение типа Event_Type, чтобы каждый менеджер делал свой enum и он был четко ясен всем
/// <summary>
/// Реализует события жизненного цикла(цикл можно определять для каждого менеджера свой)
/// События с параметрами и без можно реализовывать через UniyEvents, на такие объекты можно подписываться
/// Если этого мало, то можно UniRX ReactiveProperty использовать
/// </summary>
public abstract  class BaseGameManager: MonoBehaviour,IManager
	
{
    /// <summary>
    /// Этап жизненного цикла менеджера. Состояние запуска итп
    /// Необходимо для менеджера менеджеров
    /// </summary>
    public ManagerStatus Status { get; private set; }

	
	public abstract void Initializations();

	/// <summary>
	/// Инициализация менеджера. Не используется на прямую.
	/// </summary>
	public void Startup()
	{
		Debug.Log("Player manager starting...");
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		Initializations();
		Status = ManagerStatus.Started;
	}
	
	
	/// <summary>
	/// Делегат для события типа жизненных циклов в менеджерах. Менеджер должен иметь свой Enum для этого
	/// </summary>
	/// <param name="eventType"></param>
	public delegate void OnLifeCycleEvent(Enum eventType);
	
	private List<OnLifeCycleEvent> Listeners = new  List<OnLifeCycleEvent>();
	
	
///	<summary>
/// Добавить обработчик жизненного цикла в менеджере.
/// Менеджер может управлять системой с жизненным циклом, именно эти события он указывает в Enum 
///	</summary>
///	<param name="listener">Object to listen for event</param>
	
	public void AddLifeCycleListener(OnLifeCycleEvent listener) => Listeners.Add(listener);

	//-----------------------------------------------------------
	/// <summary>
	/// Отправляет уведомление всем обработчикам о смене жизненного цикла
	/// </summary>
	/// <param name="eventType">Тип события(именование цикла)</param>
	protected void LifeCycleNotification(Enum eventType){
		
		foreach (var listener in Listeners)
		{
			if(!listener.Equals(null)) listener(eventType);
		}
	}
	
	
	/// <summary>
	/// Удаляет все обработчики события
	/// </summary>
	protected void RemoveLifeCycleListeners() => Listeners.Clear();

	/// <summary>
	///Удаляет конкретный обработчик события
	/// </summary>
	/// <param name="listener"></param>
	public void RemoveLifeCycleListener(OnLifeCycleEvent listener) => Listeners.Remove(listener);

	/// <summary>
	/// Удаляет обработчики у которых ссылки стали null
	/// </summary>
	private void RemoveRedundancies()
	{
		if(Listeners.Count==0) return;
		Listeners.RemoveAll(element => element == null);//здесь не проверяем что gameObject обнулился, тк обработчик может быть не в Component, а в любом классе
	}
	
	
	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		RemoveRedundancies();
	}

	private void OnSceneUnloaded(Scene current)
	{
		RemoveRedundancies();
	}
	
	protected virtual void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
}
