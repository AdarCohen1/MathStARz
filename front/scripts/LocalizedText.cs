using TMPro;
using UnityEngine;
// Automatically updates a text label with a localized translation
// Supports reversing Hebrew strings for correct display
public class LocalizedLabel : MonoBehaviour
{
    public string translationKey;
    private TMP_Text label;


    private void Awake()
    {
        label = GetComponent<TMP_Text>();
    }
    // Subscribes to the language change event and updates text when enabled
    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }
    // Unsubscribes from the language change event when disabled
    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }
    // Updates the text content based on current language and translation key
    public void UpdateText()
    {
        if (LanguageManager.Instance == null)
        {
            Debug.LogWarning($"⛔ LanguageManager.Instance is null – skipping UpdateText for: {gameObject.name}");
            return;
        }
        if (string.IsNullOrEmpty(translationKey)) return;

        string translated = LanguageManager.Instance?.GetTranslation(translationKey);
        bool isHebrew = LanguageManager.Instance.CurrentLanguage == "Hebrew";

        if (label == null)
            label = GetComponent<TMP_Text>();

        if (label != null && !string.IsNullOrEmpty(translated))
        {
            label.text = isHebrew ? ReverseHebrew(translated) : translated;
        }
        // Also localize placeholder text for TMP_InputFields if present
        if (TryGetComponent<TMP_InputField>(out var inputField))
        {
            if (inputField.placeholder is TextMeshProUGUI placeholderText)
            {
                placeholderText.text = isHebrew ? ReverseHebrew(translated) : translated;
            }
        }
    }
    // Reverses a Hebrew string to display correctly in left-to-right layout
    private string ReverseHebrew(string input)
    {
        char[] array = input.ToCharArray();
        System.Array.Reverse(array);
        return new string(array);
    }
} 
