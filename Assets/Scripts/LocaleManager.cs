using UnityEngine;

public class LocaleManager : MonoBehaviour
{
    private static LocaleManager _instance;
    private TextAsset _languagesJson;
    private Locales _locale;
    private string[] _languages;

    public static string GetLocalizedText(string key)
    {
        if (_instance == null)
        {
            GameObject gameObject = new GameObject {name = "LocaleManager"};
            gameObject.AddComponent<LocaleManager>();
        }

        return _instance._locale.GetTextFromKey(key);
    }

    public static string GetCurrentLocale()
    {
        return _instance._locale.currentLocale;
    }
    void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 1;
       _instance = this;
        _languagesJson = Resources.Load("Languages") as TextAsset;
        if (_languagesJson != null) _locale = JsonUtility.FromJson<Locales>(_languagesJson.text);
        else
        {
            Debug.LogError("Languages json file is null!");
            _locale = new Locales();
        }

        if (_locale != null)
        {
            if (Application.systemLanguage == SystemLanguage.Russian ||
                Application.systemLanguage == SystemLanguage.Belarusian ||
                Application.systemLanguage == SystemLanguage.Ukrainian) _locale.currentLocale = "ru";
            else _locale.currentLocale = "en";
            _languages = _locale.GetLocales();
        }

        DontDestroyOnLoad(this);
    }

    public string[] GetLanguages()
    {
        return _languages;
    }
}

[System.Serializable]
class Locales
{
    public Locale[] localeList;
    public string currentLocale = "en";

    public Locale[] LocaleList
    {
        get => localeList;
        set => localeList = value;
    }

    public string CurrentLocale
    {
        get => currentLocale;
        set => currentLocale = value;
    }

    public string[] GetLocales()
    {
        var locales = new string[localeList.Length];
        for (var i = 0; i < locales.Length; i++)
        {
            locales[i] = localeList[i].name;
        }

        return locales;
    }

    public string GetTextFromKey(string key)
    {
        Locale needLocale = null;
        foreach (Locale locale in localeList)
        {
            if (locale.lang == currentLocale)
            {
                needLocale = locale;
                break;
            }
        }

        if (needLocale == null) return "null";
        LocaleText needText = null;
        foreach (LocaleText text in needLocale.texts)
        {
            if (text.key.ToLower() == key.ToLower())
            {
                needText = text;
                break;
            }
        }

        if (needText == null) return "null";
        return needText.text;
    }
}

[System.Serializable]
class Locale
{
    public string lang;
    public string name;
    public LocaleText[] texts;

    public string Lang
    {
        get => lang;
        set => lang = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public LocaleText[] Texts
    {
        get => texts;
        set => texts = value;
    }
}

[System.Serializable]
class LocaleText
{
    public string key;
    public string text;

    public string Key
    {
        get => key;
        set => key = value;
    }

    public string Text
    {
        get => text;
        set => text = value;
    }
}