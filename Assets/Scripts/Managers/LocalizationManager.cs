using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager: BaseGameManager
{
    //можно при изменении настроек подменять реализацию, те отсюда выдавать по ключам строки, но брать их согласно настройке языка.
    
    
    public override void Initializations()
    {
      
        Debug.Log("Loading Localization Manager.");
    }
}
