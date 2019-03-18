using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллел меню
/// </summary>
public class MenuControl : MonoBehaviour
{
    
    [SerializeField]
    private GameObject MainPanel;
    
    [SerializeField]
    private GameObject SettingsPanel;
    
    [SerializeField]
    private DialogController DialogPanel;
    
    [SerializeField]
    private Dropdown ResolutionsDropdown;

    [SerializeField]
    private Dropdown QualitiesDropdown;
    
    [SerializeField]
    private Dropdown LanguagesDropdown;
    
    [SerializeField]
    private Slider MasterVolumeSlider;
    
    [SerializeField]
    private Toggle FullscreenToggle;

    private bool isMenuActive;

    private GameObject MenuContainer;
    private void Awake()
    {

        MenuContainer = transform.parent.gameObject;
    }


    private void OnEnable()
    {
        OpenMainPanel();//чтобы начать с главное панели
    }

    public void NewGameStart()
    {
        if (Managers.App.AppState != AppManager.ApplicationState.MainMenu)
        {
            DialogPanel.OpenChoiceDialog(
                Managers.Localization.GetLocalizedValue("menu.return_to_main_menu"),
                Managers.Localization.GetLocalizedValue("Yes"),
                Managers.Localization.GetLocalizedValue("No"),
                () => Managers.App.LoadMainMenuWithCoroutine(true), () =>
                {
                    DialogPanel.CloseDialogPanel();
                });
        }
        else
        {
            Managers.App.LoadNewGameWithCoroutine();
        }
    } 

    public void PlayTestLevelPressed() => Managers.App.LoadLevelWithCoroutine(2);
    
    public void PlayContinue()
    {
        if (Managers.App.AppState != AppManager.ApplicationState.MainMenu)
        {
            MenuContainer.SetActive(false);
            Managers.App.TogglePause();
        }
        else
        {
            Debug.Log("Продолжить игру с ранее сохраненного места");
            Managers.App.ContinueGameWithCoroutine(); 
        }
        
    }

    public void ExitGame()
    {
        
        DialogPanel.OpenChoiceDialog(
            Managers.Localization.GetLocalizedValue("menu.quit_the_game_quastion"),
            Managers.Localization.GetLocalizedValue("Yes"),
            Managers.Localization.GetLocalizedValue("No"),
            () => Managers.App.ExitApp(), () =>
            {
                DialogPanel.CloseDialogPanel();
            });
    }

    public void OpenMainPanel()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        DialogPanel.CloseDialogPanel();
    }
    
    public void OpenSettingsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        DialogPanel.CloseDialogPanel();
    }

   
    
    public void setQuality(int q)
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

    public void setLanguage(int q)
    {
        Managers.Settings.Language = q;
       
    }
    
    public void setResolution(int q)
    {
        Managers.Settings.ScreenResolution = q;
       
    }
    
    private void Start()
    {
        ResolutionsDropdown.ClearOptions();
        ResolutionsDropdown.AddOptions(Managers.Settings.getResolutions());
        ResolutionsDropdown.value = Managers.Settings.ScreenResolution;
        
        QualitiesDropdown.ClearOptions();
        QualitiesDropdown.AddOptions(Managers.Settings.getQualities());
        QualitiesDropdown.value = Managers.Settings.Quality;
        
        LanguagesDropdown.ClearOptions();
        List<string> langOpts = new List<string>();
        
        foreach (LanguageItem language in Managers.Settings.getLanguages())
        {
            langOpts.Add(language.Name);
        }

        LanguagesDropdown.AddOptions(langOpts);
        LanguagesDropdown.value = Managers.Settings.Language;


        MasterVolumeSlider.value = Managers.Settings.MasterVolume;

        FullscreenToggle.isOn = Managers.Settings.Fullscreen;
    }

    public void showInfo()
    {
        DialogPanel.OpenInfoDialog("Привет пасаны", "Ок");
    }
    
    public void showChoice()
    {
        DialogPanel.OpenChoiceDialog("Привет пасаны", "Ок","No",
            () =>
            {
                Debug.Log("))))))))))))))))))");
                DialogPanel.CloseDialogPanel();
            },
            ()=>DialogPanel.CloseDialogPanel());
    }

  
}
