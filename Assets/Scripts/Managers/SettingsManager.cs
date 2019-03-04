using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

/// <summary>
/// Применяет настройки графики итп.
/// управляет сохранениями и получениями сохраненных настроек игры( разрешение, язык, сложность, другие настройки)
/// </summary>
public class SettingsManager: BaseGameManager
{
    private int _screenResolution;
    private bool _fullscreen;
    private float _masterVolume;
    private int _quality;
    private int _language;

    [Tooltip("Позволяет при запуске очищать сохраненные настройки. Для тестирования!!!!")]
    [SerializeField]
    public bool ClearSavedSettings;
    

    private Resolution[] _rsl;
    private List<string> _resolutions;
    
    //TODO Список зависит от настройки билда, списка по качеству! Если что-то менять там, то здесь тоже
    private List<string> _qualities = new List<string>()
    {
        "Очень низко",
        "Низко",
        "Средне",
        "Высоко",
        "Очень высоко",
        "Ультра"
    };
    
    private OrderedDictionary _languages = new OrderedDictionary
    {
        {SystemLanguage.English, "English"},
        {SystemLanguage.Russian, "Русский"}
    };
  

    


    public List<string> getResolutions() => _resolutions;
    public List<string> getQualities() => _qualities;
    public ICollection getLanguages() => _languages.Values;

    /// <summary>
    /// ИНдекс языка в словаре
    /// </summary>
    public int Language
    {
        get => _language;
        set
        {
            _language = value;
            
            //TODO установка языка программы
            PlayerPrefs.SetInt("app.language", _language);
            PlayerPrefs.Save();
        }
    }
    
    //НЕОБХОДИМО БУДЕТ выводить диалог с подтверждением что норм и через 10 сек возвращать те запоминать надо настройку при изменении
    
    /// <summary>
    /// Индекс в массиве разрешений _rsl
    /// </summary>
    public int ScreenResolution
    {
        get => _screenResolution;
        set
        {
            if(value >= _rsl.Length ) _screenResolution = _rsl.Length-1;
            else _screenResolution = value;
          
           Screen.SetResolution(_rsl[_screenResolution].width, _rsl[_screenResolution].height, _fullscreen);
           PlayerPrefs.SetInt("screen.resolution", _screenResolution);
           PlayerPrefs.Save();
        }
    }

    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            _fullscreen = value;
            Screen.fullScreen = _fullscreen;
            PlayerPrefs.SetInt("screen.fullscreen", Convert.ToInt32(_fullscreen));
            PlayerPrefs.Save();
        }
    }

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            Managers.Audio.AudioVolume(_masterVolume);
            PlayerPrefs.SetFloat("audio.masterVolume", _masterVolume);
            PlayerPrefs.Save();
        }
    }

    
    /// <summary>
    /// Индекс  в массиве(в настройках пкачества проекта можно видеть эти настройки последовательно)
    /// </summary>
    public int Quality
    {
        get => _quality;
        set
        {
            if(value >= _qualities.Count )_quality =  _qualities.Count-1;
            else _quality = value;
            QualitySettings.SetQualityLevel(_quality);
            PlayerPrefs.SetInt("screen.quality", _quality);
            PlayerPrefs.Save();
        }
    }


    public string ResolutionToString(int r) => _resolutions[r];

    public override void Initializations()
    {
        _resolutions = new List<string>();
        _rsl = Screen.resolutions;
        foreach (var i in _rsl)
        {
            _resolutions.Add(i.width +"x" + i.height);
        }

        if (ClearSavedSettings)
        {
            Debug.LogWarning("Включена очистка сохраненных настроек при старте игры!!! Можно отключить в SettingsManager на стартовой сцене!");
            PlayerPrefs.DeleteAll();
        }
        loadSettings();
        Debug.Log("Loading Settings Manager");
    }

    /// <summary>
    /// Инициализирует настройки сохраненные и по умолчанию. применяет их при старте игры.
    /// Далее все могут читать и устанавливать их
    /// </summary>
    private void loadSettings()
    {
        void loadMasterVolumeSetting()
        {
            if (PlayerPrefs.HasKey("audio.masterVolume"))
                _masterVolume = PlayerPrefs.GetFloat("audio.masterVolume");
            else MasterVolume = 0;
        }

        void loadQualitySetting()
        {
            if (PlayerPrefs.HasKey("screen.quality"))
                _quality = PlayerPrefs.GetInt("screen.quality");
            else Quality = 3;
        }

        void loadResolutionSetting()
        {
            if (PlayerPrefs.HasKey("screen.resolution"))
                _screenResolution = PlayerPrefs.GetInt("screen.resolution");
            else
            {
                
                ScreenResolution = _rsl.Length-1;
            }
            
            
            
        }

        void loadFullScreenSetting()
        {
            if (PlayerPrefs.HasKey("screen.fullscreen"))
                _fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("screen.fullscreen"));
            else Fullscreen = true;
        }
        //TODO можно как-то определить язык системы, чтобы поставить по умолчанию
        void loadLanguageSetting()
        {
            if (PlayerPrefs.HasKey("app.language"))
                _language = PlayerPrefs.GetInt("app.language");
            else
            {

                if (_languages.Contains(Application.systemLanguage))
                {
                    int i = -1;
                    foreach (var key in _languages.Keys)
                    {
                        i++;
                        if (Application.systemLanguage.Equals(key)) break;
                    }
                    
                    _language = i < 0? 0 : i;
                } else _language = 0;
            }
        }

        loadFullScreenSetting();
        loadResolutionSetting();
        loadQualitySetting();
        loadMasterVolumeSetting();
        loadLanguageSetting();
    }

    /// <summary>
    /// Очистка настроек в памяти и сохраненных. Сразу же сменит все режимы
    /// </summary>
    public void ClearSettings()
    {
        PlayerPrefs.DeleteAll();
        loadSettings();
    }
}
