using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersLoaderForMainMenu : MonoBehaviour
{
   [SerializeField]
    private Managers managersPrefab;
    
     
    [Tooltip("Позволяет при запуске очищать сохраненные настройки(звук, разрешение итп). Для тестирования!!!! Дублирует настройку для менеджера settings, но она сработает только если вю игру запускать, а эта при запуске уровня непосредственно")]
    [SerializeField]
    private bool ClearSavedSettings;


    private void Awake()
    {
        if (GameObject.FindWithTag("Managers") == null)
        {
            GameObject.Instantiate(managersPrefab);
            //вся основная инициализация менеджеров прошла, можно их юзать частично.
            if(ClearSavedSettings) Managers.Settings.ClearSettings();
            
            StartCoroutine(initLevel());//будет асинхронно, те все загрузиться но старт уже наступит.
        }
    }

    IEnumerator initLevel()
    {
        while (!Managers.isManagersLoadingDone)
        {
            yield return null;
        }
        Managers.App.SetStateManual_DO_NOT_USE_IN_YOUR_CODE(AppManager.ApplicationState.MainMenu);
    }
}
