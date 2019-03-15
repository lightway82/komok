using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


//TODO если мы обновили Languages, но редактор открыт, то как будет  добавлен файл? Изучить жизненный цикл окна!
/// <summary>
/// Для добавления языка, добавляем его в Languages.cs, при открытии редактора у нас создастся необходимый файл.
/// </summary>
public class LocalizedTextEditor : EditorWindow
{
    private  class LocalizedStringItem
    {
        public string value;
        public SystemLanguage lang;
        public string key;
    }
    
    private Dictionary<string,List<LocalizedStringItem>> localizationData;

    [MenuItem ("Инструменты/Редактор переводов")]
    static void Init()
    {
        EditorWindow.GetWindow (typeof(LocalizedTextEditor)).Show ();
    }

    private string localizationDir  = Application.streamingAssetsPath+"/localization";
    private string newKey = "";
    private Vector2 _scrollPosition;
    private Vector2 _scrollPositionTextareas;
    private int selectionGridIndex = -1;
    private GUIContent[] _guiContents;
    private string addedKey;
    private void OnGUI()
    {
        GUILayout.Label("Путь к директории локализации:"+localizationDir);
        GUILayout.Label("Добавляя ключи, учтите, что точка разделяет подкатегории и важна для работы. Группируйте ключи по категориям и подкатегориям!");
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(350f), GUILayout.MaxWidth(350f));
                EditorGUILayout.BeginHorizontal(GUILayout.Width(350f), GUILayout.MaxWidth(350f));
                    GUI.SetNextControlName("SAVE_ALL");
                    newKey = EditorGUILayout.TextField("", newKey, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("+",GUILayout.Width(50f)))
                    {
                      AddNewKey(newKey);
                    }
                    
                    
                    if (GUILayout.Button("-", GUILayout.Width(50f)))
                    {
                        if (selectionGridIndex != -1)
                        {
                            DeleteKey(guiContents[selectionGridIndex].text, selectionGridIndex);
                        }
                        
                    }
                 EditorGUILayout.EndHorizontal();
                _scrollPosition = GUILayout.BeginScrollView (_scrollPosition);
                 _guiContents = GetGUIContentsFromItems();
                 if (selectionGridIndex == -2) selectionGridIndex = GetGUIIndexByKey(addedKey);
                 SelectedItemDisplay (selectionGridIndex);
                selectionGridIndex = GUILayout.SelectionGrid (selectionGridIndex, _guiContents,1,GetGUIStyleKeys ());
               
                GUILayout.EndScrollView ();
            EditorGUILayout.EndVertical();
        
            EditorGUILayout.BeginVertical();
                    GUI.SetNextControlName("___");
                    if (GUILayout.Button("Сохранить изменения"))
                    {
                        SaveAllFiles();
                        EditorUtility.DisplayDialog("Сохранение изменений", "Все прошло как по маслу!", "Так точно!");
                        GUI.FocusControl("SAVE_ALL");//снимаем фокус с текстарий иначе в них не будет обновляться значение при открытии другого ключа
                    }
                    _scrollPositionTextareas = GUILayout.BeginScrollView (_scrollPositionTextareas);
                    SelectedItemAction (selectionGridIndex);
                    GUILayout.EndScrollView ();
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
       
    }

    private int GetGUIIndexByKey(string key)
    {
        int index = -1;
        foreach (var content in guiContents)
        {
            index++;
            if(content.text.Equals(key)) break;
        }

        return index;
    }

    private List<GUIContent> guiContents = new List<GUIContent> ();
    private GUIContent[] GetGUIContentsFromItems () {
        guiContents.Clear();
        foreach (var key in localizationData.Keys)
        {
            GUIContent guiContent = new GUIContent ();
            guiContent.text = key;
            guiContents.Add (guiContent);
        }
        return guiContents.ToArray ();
    }

    private int prevIndex = -1;
    private void SelectedItemAction(int index)
    {
        if(prevIndex!=index)  {
            GUI.FocusControl("SAVE_ALL");//снимаем фокус с текстарий иначе в них не будет обновляться значение при открытии другого ключа
            
        }
        if (index >-1)
        {
            
            string key = guiContents[index].text;
            foreach (var item in localizationData[key])
            {
                Languages.GetLanguageItem(item.lang, out LanguageItem lang);
                GUILayout.Label(lang.Name+":");
               
                item.value = EditorGUILayout.TextArea(item.value, GUILayout.Height(150));
            }

            prevIndex = index;
        }
    }
    
    private void SelectedItemDisplay(int index)
    {
            if (index >-1) EditorGUILayout.TextField("", guiContents[index].text, GUILayout.ExpandWidth(true));
           
    }
    
    private GUIStyle GetGUIStyleKeys () {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = 350f
        };
        return guiStyle;
    }
  
    
    private void LoadGameData(out Dictionary<string,List<LocalizedStringItem>> localizationData)
    {
        //нужно при создании ключей проверить консистентность полей. Можно создать поля если их нет - те дополнить LocalizationData и серилизовать их в файлы, чтобы решить эту проблему(данные брать из имеющихся локализаций)
        localizationData = new Dictionary<string,List<LocalizedStringItem>>();
        LocalizationData data = null;
        //на данный момент файлы точно соответствуют языкам в игре.
        foreach (var item in Languages._languages)
        {
            string filePath = localizationDir + "/strings_" + item.Abbr + ".json";
            if (!string.IsNullOrEmpty (filePath))
            {
                data = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(filePath));
               
                foreach (var dataItem in data.items)
                {
                    if (!localizationData.ContainsKey(dataItem.key))
                    {
                        localizationData.Add(dataItem.key, new  List<LocalizedStringItem>());
                    }
                    
                    //TODO тут бы сортед лист, и сортировать по языку!!!
                    localizationData[dataItem.key].Add(new LocalizedStringItem
                    {
                        value = dataItem.value,
                        key = dataItem.key,
                        lang = item.Language
                        
                    });
                }

               

            }
        }
        
        CheckConsistency();
    }

    /// <summary>
    /// Проверит все ли ключи словаря есть и переводы. Если переводлв не хватает, то добавит их
    /// если все в порядке, то процесс происходит очень быстро
    /// </summary>
    private void CheckConsistency()
    {
        HashSet<LanguageItem> langsToSave = new HashSet<LanguageItem>();
        foreach (var item in localizationData)
        {
            if(item.Value.Count != Languages._languages.Count) 
            {
                
                foreach (var language in Languages._languages)
                {
                    //если нет элемента с данным языком, то мы его создадим и добавим в список
                    if (item.Value.Find(it => it.lang == language.Language) == null)
                    {
                        langsToSave.Add(language);
                        item.Value.Add(new LocalizedStringItem
                        {
                            key   = item.Key,
                            value = "",
                            lang = language.Language
                        });
                    }
                }
                
                
            }
        }

        //сохраняем данные для языков, в которых добавились недостающие элементы
        foreach (var languageItem in langsToSave)
        {
            SaveLocalizedData(extractLocalizationData(languageItem), languageItem.Language);
        }
    }

    
    private void AddNewKey(string newKey)
    {
        //проверить сущестование.
        //добавить ключ, обновить все файлы.
        //выбрать ключ в списке, чтобы вывелись редактируемые поля
        

        if (localizationData.ContainsKey(newKey) || String.IsNullOrEmpty(newKey))
        {
            EditorUtility.DisplayDialog("Создание нового ключа", "Ключ  " + newKey + " уже существует или пустой!",
                "Ясно!");
            return;
        }
        List<LocalizedStringItem> NewLocalizations = new List<LocalizedStringItem>();
        localizationData.Add(newKey, NewLocalizations);
        foreach (var language in Languages._languages)
        {
            NewLocalizations.Add(new LocalizedStringItem
            {
                key = newKey,
                value = "",
                lang = language.Language
            });
        }
        
        SaveAllFiles();
        selectionGridIndex = -2;
        addedKey = newKey;
    }

    private void DeleteKey(string key, int index)
    {
        //как будут себя вести поля в инспекторе? Строка должна выдаваться равная ключу переданному.
        if (!localizationData.ContainsKey(key)) return;
        localizationData[key].Clear();
        localizationData.Remove(key);
        guiContents.RemoveAt(index);
        SaveAllFiles();
        selectionGridIndex = -1;
    }

    /// <summary>
    /// Извлекает их общего словаря данные касающие только указанного языка.
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    private LocalizationData extractLocalizationData(LanguageItem language)
    {
        LocalizationData data = new LocalizationData();
        List<LocalizationItem> ldata = new List<LocalizationItem>();
            
        foreach (var value in localizationData.Values)
        {
            foreach (var localizedStringItem in value)
            {
                if(localizedStringItem.lang==language.Language){ ldata.Add(new LocalizationItem
                    {
                        key = localizedStringItem.key,
                        value = localizedStringItem.value
                    });
                    break;
                }
            }
        }

        data.items = ldata.ToArray();
        return data;
    }

        //TODO тупой алгоритм и структура данных. переработать
    private void SaveAllFiles()
    {

        foreach (var language in Languages._languages)
        {
           var data =  extractLocalizationData(language);
            try
            {
                SaveLocalizedData(data, language.Language);
            }
            catch (Exception e)
            {
               Debug.LogError(e);
                EditorUtility.DisplayDialog("Сохранение файлов переводов", "Ошибка записи файлов переводов. Язык "+language.Name,
                    "Ясно!");
                return;
            }
        }
    }

    private class SavedLanguageFileException : Exception
    {
        public SavedLanguageFileException(string message) : base(message)
        {
        }

        public SavedLanguageFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Сохраняет в файл данные языка.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="lang"></param>
    /// <exception cref="SavedLanguageFileException">Может выкинуть исключение если язык не найден в списке поддерживаемых или не удачная запись файла</exception>
    private void SaveLocalizedData(LocalizationData data, SystemLanguage lang)
    {
        if (Languages.GetLanguageItem(lang, out LanguageItem languageItem))
        {
            string filePath = localizationDir + "/strings_" + languageItem.Abbr + ".json"; 
            
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string dataToWrite = JsonUtility.ToJson(data, prettyPrint: true);
                    File.WriteAllText (filePath, dataToWrite);
                }
                catch (Exception e)
                {
                    throw new SavedLanguageFileException("Ошибка записи файла локализации. "+filePath, e);
                }
               
            }
        }else throw new SavedLanguageFileException("Попытка сохранить данные неизвестного игре языка");
      
    }
    

    private void CreateNewFile(LanguageItem language, bool showExistsDialog=false)
    {

        string path = localizationDir + "/strings_" + language.Abbr + ".json";
        if (File.Exists(path) )
        {
            return;
        }

       string[] files =  Directory.GetFiles(localizationDir,"*.json");
        if (files.Length == 0)
        {
            File.WriteAllText (path, "{\"items\":[]}");
            return;
        } 
        
        //проверяем есть ли русская локализация, если есть, то делаем ее копию для нового файла
        if (File.Exists(localizationDir + "/strings_ru.json"))
        {
            File.Copy(localizationDir + "/strings_ru.json", path);
        }
        else
        {
            File.Copy(files[0], path);
        }
    }

    /// <summary>
    /// Создает файлы переводасогласно списку языков, учитывает уже имеющиеся
    /// </summary>
    private void CreateLocalizationFiles()
    {
        foreach (var item in Languages._languages)
        {
            CreateNewFile(item);
            
        }
    }


    private void OnEnable()
    {
        Debug.Log("OnEnable LocalizedTextEditor");
        CreateLocalizationFiles();
        LoadGameData(out localizationData);
    }

    private void OnDisable()
    {
        
        
    }

    private void OnDestroy()
    {
        
        
    }
    
}