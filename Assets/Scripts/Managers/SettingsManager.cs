using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private Resolution[] _rsl;
    private List<string> _resolutions;



    public List<string> getResolutions() => _resolutions;

    //НЕОБХОДИМО БУДЕТ выводить диалог с подтверждением что норм и через 10 сек возвращать те запоминать надо настройку при изменении
    
    /// <summary>
    /// Индекс в массиве разрешений _rsl
    /// </summary>
    public int ScreenResolution
    {
        get => _screenResolution;
        set
        {
            _screenResolution = value;
            Screen.SetResolution(_rsl[_screenResolution].width, _rsl[_screenResolution].height, _fullscreen);
        }
    }

    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            _fullscreen = value;
            Screen.fullScreen = _fullscreen;
        }
    }

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            Managers.Audio.AudioVolume(_masterVolume);
            //как сохранять? не стоит это делать непрерывно. Возможно сохранять по таймеру - если в течении 5-10 сек ничего не меняли то сохранить
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
            _quality = value;
            QualitySettings.SetQualityLevel(_quality);
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
       
        Debug.Log("Loading Settings Manager");
    }
}
