// ✅ SettingsController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Controls the Settings panel behavior: volume control and language selection.
public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;
    // Maps visible dropdown labels to internal language codes
    private Dictionary<string, string> languageOptions = new Dictionary<string, string>
    {
        { "English", "English" },
        { "תירבע", "Hebrew" }
    };
    // Initializes volume and language settings when the panel loads
    private void Start()
    {
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        SetupLanguages();
        languageDropdown.onValueChanged.AddListener(SetLanguage);
    }
    // Updates the global volume based on slider value
    private void SetVolume(float value)
    {
        AudioListener.volume = value;
    }
    // Populates the language dropdown and sets the initial selection
    private void SetupLanguages()
    {
        languageDropdown.ClearOptions();
        List<string> languages = new List<string> { "English", ReverseHebrew("עברית") };
        languageDropdown.AddOptions(languages);
        languageDropdown.value = LanguageManager.Instance.CurrentLanguage == "Hebrew" ? 1 : 0;
        languageDropdown.RefreshShownValue();
    }
    // Changes the application language based on selected dropdown option
    private void SetLanguage(int index)
    {
        string selectedDisplay = languageDropdown.options[index].text;

        if (languageOptions.TryGetValue(selectedDisplay, out string internalLang))
        {
            LanguageManager.Instance.SetLanguage(internalLang);
            Debug.Log("Language set to: " + internalLang);
        }
        else
        {
            Debug.LogWarning("Unknown language: " + selectedDisplay);
        }

        FixDropdownLabelAlignment();
    }
    // Adjusts dropdown text alignment for RTL or LTR based on the selected language
    private void FixDropdownLabelAlignment()
    {
        string selectedDisplay = languageDropdown.options[languageDropdown.value].text;
        bool isHebrew = selectedDisplay == "עברית";

        var mainLabel = languageDropdown.transform.Find("Label")?.GetComponent<TMP_Text>();
        if (mainLabel != null)
        {
            mainLabel.alignment = isHebrew ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            mainLabel.isRightToLeftText = isHebrew;
        }

        var template = languageDropdown.template;
        if (template != null)
        {
            var items = template.Find("Viewport/Content");
            if (items != null)
            {
                foreach (Transform item in items)
                {
                    var label = item.Find("Item Label")?.GetComponent<TMP_Text>();
                    if (label != null)
                    {
                        bool optionIsHebrew = label.text == "עברית";
                        label.alignment = optionIsHebrew ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                        label.isRightToLeftText = optionIsHebrew;
                    }
                }
            }
        }
    }
    // Reverses Hebrew text to display correctly in left-to-right layout
    private string ReverseHebrew(string input)
    {
        char[] array = input.ToCharArray();
        System.Array.Reverse(array);
        return new string(array);
    }
}
