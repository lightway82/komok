using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//todo возможно стоит регулировать общую громкость и громкость эффектов, музыки отдельно?
public class AudioManager: BaseGameManager
{
    [SerializeField]
    private AudioMixer audioMixer;//аудиомиксер(в ассетах)
    
    [SerializeField]
    private string exposedVolumeName;//имя переменной уровня звука выставленной из миксера для скрипта
    
    public override void Initializations()
    {
       
        Debug.Log("AudioManager started.");
    }

    public override void PostInitialization()
    {
        
        
    }


    public void AudioVolume(float value)
    {
        audioMixer.SetFloat(exposedVolumeName, value);
    }
}
