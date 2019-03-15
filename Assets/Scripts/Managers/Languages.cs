using System.Collections.Generic;
using UnityEngine;

public static class Languages{
    
    public static readonly List<LanguageItem> _languages = new List<LanguageItem>
    {
        new LanguageItem("English","en", SystemLanguage.English),
        new LanguageItem("Русский","ru", SystemLanguage.Russian),
        new LanguageItem("Немецкий","de", SystemLanguage.German)
    };
    
    private static Dictionary<SystemLanguage, LanguageItem> dict = new Dictionary<SystemLanguage, LanguageItem>(); 

    static Languages()
    {
        foreach (var item in _languages)
        {
            dict[item.Language] = item;
        }
    }

    /// <summary>
    /// Получает LanguageItem по языку
    /// </summary>
    /// <param name="lang"> язык</param>
    /// <param name="languageItem"></param>
    /// <returns>true если все норм, false если нет такого языка в списке</returns>
    public static bool GetLanguageItem(SystemLanguage lang, out LanguageItem languageItem)
    {
        return dict.TryGetValue(lang, out languageItem);
    }
    
   
}