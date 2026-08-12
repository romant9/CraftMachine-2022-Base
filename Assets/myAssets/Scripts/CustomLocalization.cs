using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CustomLocalization", menuName = "Custom/Create CustomLocalization")]
public class CustomLocalization : ScriptableObject
{
    private static CustomLocalization instance;

    public static CustomLocalization Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<CustomLocalization>("CustomLocalization");
            }
            return instance;
        }
    }
    public static List<string> AvailableLanguages = new List<string>() { "ru", "en", "es" };

    public List<CustomLocItem> CustomLocItems;

    public const string BlankKey = "Custom.Blank";


    //для Tooltip, AlertPopup, ConfirmationPopup
    public static string GetText(string key)
    {
        string value = "";
        if (key.StartsWith("Custom.")) key = key.Substring(7);
        var currentLang = LocalizationManager.Instance.CurrentLanguage;
        int index = 1;
        if (AvailableLanguages.Contains(currentLang))
        {
            index = AvailableLanguages.IndexOf(currentLang);
        }
        var locItem = Instance.CustomLocItems.FirstOrDefault(x => x.Name == key);
        if (locItem != null)
        {
            if (locItem.Values != null && locItem.Values.Count > index)
            {
                value = locItem.Values[index];
            }
            else
            {
                value = locItem.Values[1];
            }
        }      
        return value;
    }
}

[Serializable]
public class CustomLocItem
{
    public string Name;
    public List<string> Values;
}
