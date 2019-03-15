using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {
    [LocalizationId(false)]
    public string key;
    
    void Start ()
    {
        Text text = GetComponent<Text> ();
        if (text == null)
        {
            Debug.LogError(key+" Комплнент локализациитекста назначен не тому геймобъекту. Требуется наличие компонента Text");
            return;
        }
        text.text = Managers.Localization.GetLocalizedValue (key);
    }

}