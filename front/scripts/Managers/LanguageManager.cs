using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
// Represents a single translation entry (key-value pair)
[Serializable]
public class TranslationEntry
{
    public string key;
    public string value;
}
// Represents a block of translations for a specific language
[Serializable]
public class LanguageBlock
{
    public List<TranslationEntry> entries;
}
// Root structure of the JSON file containing all language blocks
[Serializable]
public class TranslationRoot
{
    public LanguageBlock English;
    public LanguageBlock Hebrew;
}
// Manages language selection and provides translation lookup at runtime
public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;
    private const string LANGUAGE_KEY = "selectedLanguage";
    public static event Action OnLanguageChanged;

    private Dictionary<string, Dictionary<string, string>> translations;
    public string CurrentLanguage { get; private set; }

    // Ensure singleton instance and load translations on awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTranslations();
            LoadLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load translation JSON file and parse it into memory
    private void LoadTranslations()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "translations.json");

#if UNITY_ANDROID && !UNITY_EDITOR
    StartCoroutine(LoadFromAndroid(path));
#else
        if (!File.Exists(path))
        {
            Debug.LogError("❌ Translations file not found at " + path);
            return;
        }

        string jsonText = File.ReadAllText(path);
        ApplyTranslations(jsonText);
#endif
    }

    // Android-specific loading (for StreamingAssets)
    private System.Collections.IEnumerator LoadFromAndroid(string path)

    {
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to load translations from Android path: " + request.error);
            yield break;
        }

        string jsonText = request.downloadHandler.text;
        ApplyTranslations(jsonText);
    }

    // Parse the JSON and populate the translations dictionary
    private void ApplyTranslations(string jsonText)
    {
        TranslationRoot root = JsonUtility.FromJson<TranslationRoot>(jsonText);

        translations = new Dictionary<string, Dictionary<string, string>>
    {
        { "English", ConvertToDict(root.English.entries) },
        { "Hebrew", ConvertToDict(root.Hebrew.entries) }
    };

        Debug.Log("✅ Translations loaded successfully.");
    }

    // Convert a list of translation entries to a dictionary
    private Dictionary<string, string> ConvertToDict(List<TranslationEntry> entries)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
    // Set the current language and update all relevant UI
    public void SetLanguage(string language)
    {
        if (translations.ContainsKey(language))
        {
            PlayerPrefs.SetString(LANGUAGE_KEY, language);
            PlayerPrefs.Save();
            CurrentLanguage = language;
            Debug.Log("✅ Language set to: " + language);

            OnLanguageChanged?.Invoke();

            // Update all localized UI labels in the scene
            LocalizedLabel[] allLabels = FindObjectsOfType<LocalizedLabel>(true);
            foreach (var label in allLabels)
                label.SendMessage("UpdateText", SendMessageOptions.DontRequireReceiver);
            // Refresh data display in the main menu (if applicable)
            MainMenu menu = FindAnyObjectByType<MainMenu>();
            if (menu != null)
                menu.displayData();
        }
        else
        {
            Debug.LogWarning("⚠️ Language not found in translations: " + language);
        }
    }

    // Return the translated value for a given key in the current language
    public string GetTranslation(string key)
    {
        if (translations != null &&
            translations.ContainsKey(CurrentLanguage) &&
            translations[CurrentLanguage].ContainsKey(key))
        {
            return translations[CurrentLanguage][key];
        }

        Debug.LogWarning($"⚠️ Translation key not found: {key}");
        return key;
    }
    // Load previously saved language preference or default to English
    private void LoadLanguage()
    {
        CurrentLanguage = PlayerPrefs.GetString(LANGUAGE_KEY, "English");
    }
    // Find the translation key based on an English value (used for reverse lookup)
    public string FindKeyByEnglishValue(string value)
    {
        if (translations.ContainsKey("English"))
        {
            foreach (var pair in translations["English"])
            {
                if (pair.Value == value)
                    return pair.Key;
            }
        }
        return null;
    }



}
