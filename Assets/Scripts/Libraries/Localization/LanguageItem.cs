using UnityEngine;

public class LanguageItem
{
    public string Name { get; }
    public string Abbr { get; }
    public SystemLanguage Language { get; }

    public LanguageItem(string name, string abbr, SystemLanguage lang)
    {
        Name = name;
        Abbr = abbr;
        Language = lang;
    }
}