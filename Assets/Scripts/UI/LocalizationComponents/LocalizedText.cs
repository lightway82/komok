using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : BaseLocalizedComponent {
    [LocalizationId(false)]
    public string key;
    private Text text;
    private bool registered;
    private void Awake()
    {
        text = GetComponent<Text> ();
        if (text == null)
        {
            Debug.LogError(key+" Комплнент локализациитекста назначен не тому геймобъекту. Требуется наличие компонента Text");
            
        }
        
    }

    void Start ()
    {
        //если мы временно деактивировали объект, то не нужно его удалять из зарегистрированных в менеджере
        //при активации старт будет опять вызван.
        if (!registered)
        {
            Managers.Localization.RegisterLocalizedItem(this);
            registered = true;
        }
        SetLocalizedData();
    }

    public override void SetLocalizedData()
    {
        text.text = Managers.Localization.GetLocalizedValue (key);
    }

    public void setID(string ID)
    {
        key = ID;
    }


    private void OnDestroy()
    {
        //отпишемся только при удалении
        Managers.Localization.UnRegisterLocalizedItem(this); 
    }

    
}