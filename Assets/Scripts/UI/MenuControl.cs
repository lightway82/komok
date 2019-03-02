using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MenuControl : MonoBehaviour
{
    [SerializeField]
    private Dropdown ResolutionsDropdown;

    public void NewGameStart() => Managers.App.LoadNewGameWithCoroutine();

    public void PlayTestLevelPressed() => Managers.App.LoadLevelWithCoroutine(2);
    
    public void PlayContinue()
    {
        Debug.Log("Продолжить игру с ранее сохраненного места");
        Managers.App.ContinueGameWithCoroutine();
    }

    public void ExitGame() => Managers.App.ExitApp();
    
    
    public void Quality(int q)
    {
        Managers.Settings.Quality = q;
       
    }

    public void setMasterVolume(float value)
    {
        Managers.Settings.MasterVolume = value;
    }

    public void setFullscreen(bool value)
    {
        Managers.Settings.Fullscreen = value;
    }

    private void Start()
    {
        ResolutionsDropdown.ClearOptions();
        ResolutionsDropdown.AddOptions(Managers.Settings.getResolutions());
    }
}
