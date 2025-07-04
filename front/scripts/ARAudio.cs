using UnityEngine;

// Plays a language-specific audio clip (Hebrew or English) when the scene starts.
// Uses the current language from LanguageManager to decide which clip to play.
public class ARAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hebrewClip;
    [SerializeField] private AudioClip englishClip;

    // On start, selects and plays the appropriate audio clip based on the current language.
    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("🎵 AudioSource missing on ARAudio");
            return;
        }

        string currentLang = LanguageManager.Instance.CurrentLanguage;

        if (currentLang == "Hebrew" && hebrewClip != null)
        {
            audioSource.clip = hebrewClip;
        }
        else if (currentLang == "English" && englishClip != null)
        {
            audioSource.clip = englishClip;
        }
        else
        {
            Debug.LogWarning("⚠️ No matching audio clip for language: " + currentLang);
            return;
        }

        audioSource.Play();
    }
}
