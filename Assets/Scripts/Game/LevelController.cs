using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    
    [SerializeField]
    private GameObject MenuContainerPrefab;
    private GameObject MenuContainer;
    
    private void Start()
    {
        
        Managers.App.AddAppStateListener(AppStateListener);
        MenuContainer =  Instantiate(MenuContainerPrefab);
        MenuContainer.SetActive(false);
    }

    private void AppStateListener(Enum eventtype)
    {
        
        switch ((AppManager.ApplicationState)eventtype)
        {
                case AppManager.ApplicationState.ApplicationPause:
                    PauseActions();
                break;
                case AppManager.ApplicationState.LevelInProcess:
                    OnContinuePlayActions();
                break;
        }
    }

    private void PauseActions()
    {
        Debug.Log("PAUSED");
    }

    private void OnContinuePlayActions()
    {
        Debug.Log("PLAY");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMenu();
        }
       
    }

    private void ToggleMenu()
    {
        Managers.App.TogglePause();
        MenuContainer.SetActive(!MenuContainer.activeSelf);
        
    }
    
    
}
