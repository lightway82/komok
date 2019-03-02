using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersLoaderForMainMenu : MonoBehaviour
{
   [SerializeField]
    private Managers managersPrefab;

    private void Awake()
    {
        if (GameObject.FindWithTag("Managers") == null)
        {
            GameObject.Instantiate(managersPrefab);
            StartCoroutine(initLevel());//тк надо дождаться инициализации всех менеджеров
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
