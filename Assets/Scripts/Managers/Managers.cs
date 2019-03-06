using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер менеджеров
/// Все менеджеры должны использоваться из контроллеров( те из классов управляющих визуалом, а не на прямую в событиях UI итп именно им доступен Managers, тк он должен инстанцироваться только в стартовой сцене, а в отстальные автоматом попадет, как синглтон)
/// </summary>
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(AppManager))]
[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(SettingsManager))]
[RequireComponent(typeof(LocalizationManager))]
[RequireComponent(typeof(SaveManager))]
public class Managers : MonoBehaviour
{
   public static PlayerManager Player { get; private set; }
   public static AudioManager Audio { get; private set; }
   public static AppManager App { get; private set; }
   public static GameManager Game { get; private set; }
   public static SettingsManager Settings { get; private set; }
   public static LocalizationManager Localization { get; private set; }
   public static SaveManager Save { get; private set; }
   
   private IList<BaseGameManager> _initListManagers;

   /// <summary>
   /// Указывает на то загрузились ли все менеджеры.
   /// В загрузочной сцене нужно ожидать этого, только после этого начинать манипуляции с менеджерами или загрузки др. сцен!!! Иначе могут быть глюки, если не все менеджеры загрузятся
   /// </summary>
   public static bool isManagersLoadingDone { get; private set; }

   void Awake()
   {
      DontDestroyOnLoad(gameObject);
      Player = GetComponent<PlayerManager>();
      Audio = GetComponent<AudioManager>();
      App = GetComponent<AppManager>();
      Game = GetComponent<GameManager>();
      Settings = GetComponent<SettingsManager>();
      Localization = GetComponent<LocalizationManager>();
      Save = GetComponent<SaveManager>();
      
      _initListManagers = new List<BaseGameManager>
      {
         Player,
         Audio,
         App,
         Game,
         Settings,
         Localization,
         Save
      };
      
      foreach (var manager in _initListManagers)
      {
         manager.Startup();
      }
      //если все менеджеры синхронно инициализируются, то тут уже все готово(это важно, чтобы к start все было готово)
      //если есть асинхронные процессы в initialization, то возможно далее придется покрутиться, но они уже будут завершены после awake
      //Асинхронно нужно грузить ресурсы итп, то что не нужно в стартовой сцене и не является критичным для работы менеджеров. Важно чтобы мы могли в start полагаться на менеджеры, а доп ресурсы они могут подгрузить асинхронно!
      StartCoroutine(StartupManagers());
   }

   private IEnumerator StartupManagers()
   {
      int numModules = _initListManagers.Count;
      int numReady = 0;
      int lastReady;
      
      while (numReady < numModules)
      {
         lastReady = numReady;
         numReady = 0;
         foreach (var manager in _initListManagers)
         {
            if (manager.Status == ManagerStatus.Started) numReady++;
            if(numReady > lastReady) Debug.Log("Progress: " + numReady+"/"+ numModules);
            yield return null;
         }
         
         foreach (var manager in _initListManagers)
         {
            manager.PostInitialization();
            yield return null;
         }
         Debug.Log("All managers started.");

         isManagersLoadingDone = true;
      }
   }


}
