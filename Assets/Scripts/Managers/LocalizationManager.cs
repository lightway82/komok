using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// Менеджер локализации отвечает за предоставление корректных данных о локализованных строках.
/// Менеджер корректно обрабатывает ситуацию смены локали в настройках.
/// </summary>
public class LocalizationManager: BaseGameManager
{
    
    private Dictionary<string, string> localizedText = new Dictionary<string, string> ();
    private readonly HashSet<ILocalizedItem> registeredItems = new HashSet<ILocalizedItem>();
    
    public override void Initializations()
    {
      
        Debug.Log("Loading Localization Manager.");
    }

    public override void PostInitialization()
    {
        Debug.Log("Post Loading Localization Manager.");
        //здесь расчитываем на данные SettingsManager
        
        //следим за загрузкой новой сцены и очищаем список зарегистрированных элементов.
        Managers.App.WantChangeSceneEvent.AddListener(ClearRegistered);
        Managers.Settings.ChangeLanguageEvent.AddListener(OnChangedLanguage);
        LoadLocalizedText("localization\\strings_"+Managers.Settings.getCurrentLanguageItem().Abbr+".json");
        //если в рдакторе запустили, то перезагрузить надо всеэлементы языковые, тк при запуске отдельного уровня
        //LoadLocalizedText не успевает отработать до того как элементы вставлены и инициализированы(они в start инициализируются). В штатном запуске все будет загружено еще при начальной загрузке игры
        if (Application.isEditor) OnChangedLanguage(Managers.Settings.getCurrentLanguageItem());
    }

    private void OnChangedLanguage(LanguageItem lang)
    {
        Debug.Log("Смена значений всех локализованных элементов после изменения языка "+lang.Name);
        StartCoroutine(ChangeLanguage(lang));
    }



    private IEnumerator ChangeLanguage(LanguageItem lang)
    {
        //TODO если текстов много и видны подвисания, то следует процессы загрузки делать через yield разбив по кадрам или вообще сделать таск в отдельном потоке
        LoadLocalizedText("localization\\strings_"+Managers.Settings.getCurrentLanguageItem().Abbr+".json");
        yield return null;
        int i = -1;
        foreach (var registeredItem in registeredItems)
        {
            i++;
            registeredItem.SetLocalizedData();
            if (i % 100 != 0) yield return null;//разобьем по 100 элементов на кадр.
        }
    }

    /// <summary>
    /// Очистка списка зарегистрированных локализованных элементов между сценами
    /// 
    /// </summary>
    private void ClearRegistered(int sceneIndex)
    {
        Debug.Log("Очиска зарегистрированных локализованных элементов");
        registeredItems.Clear();
        registeredItems.TrimExcess();//чтобы избежать потери памяти  из-за раздутого списка, после выгрузки большой сцены
    }


    private void LoadLocalizedText(string fileName)
    {
        string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
        
        localizedText.Clear();
        
        if (File.Exists (filePath)) {
            string dataAsJson = File.ReadAllText (filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (dataAsJson);
            for (int i = 0; i < loadedData.items.Length; i++) 
            {
                localizedText.Add (loadedData.items [i].key, loadedData.items [i].value);   
            }

            Debug.Log ("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        } else 
        {
            Debug.LogError ("Cannot find localization file: "+filePath);
        }
    }

    /// <summary>
    /// Вернет локализованное значение иначе вернет значение ключа, чтобы можно было понять где нет перевода
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalizedValue(string key)
    {
       return localizedText.ContainsKey(key) ? localizedText[key] : key; 
    } 

    /// <summary>
    /// Проверяет есть ли для данного ключа перевод
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool CheckKey(string key) => localizedText.ContainsKey(key);

    /// <summary>
    /// Регистрирует локализованный элемент для возможности его обновить после переключения локали
    /// </summary>
    /// <param name="item"></param>
    public void RegisterLocalizedItem(ILocalizedItem item)
    {
      registeredItems.Add(item);  
    }
    
    /// <summary>
    /// Можно удаляться из списка регистрации, но при перезагрузке сцены менеджер сам всех удалит из списка.
    /// </summary>
    /// <param name="item"></param>
    public void UnRegisterLocalizedItem(ILocalizedItem item)
    {
        registeredItems.Remove(item);  
    }
    
}
