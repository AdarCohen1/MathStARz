using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Manages the visual and interactive elements of a single question.
// Supports both open-ended and multiple choice question formats.
public class QuestionItem : MonoBehaviour
{
    public enum Q_type { open, multiple_choice };
    public Q_type q_Type;
    public GameObject openPanel, multiplePanel;
    public TextMeshProUGUI question_text, correct, wrong;
    
    public Toggle[] toggles;
    public TMP_InputField answerInput;

    public Button submit, exit;

    public Image img;

    public CanvasGroup canvasGroup => GetComponent<CanvasGroup>();

    // Set initial visibility
    void Awake()
    {
        canvasGroup.alpha = 0f;
    }
    
    // Disable all panels at start
    void Start()
    {
        openPanel.SetActive(false);
        multiplePanel.SetActive(false);
    }

    // Called when the user clicks the X button to close the question
    public void ClosePanel()
    {
        Destroy(gameObject);
        Debug.Log("❌ Question panel destroyed via X button.");
    }

    // Checks if the question text is in Hebrew (used for RTL detection)
    public static bool IsHebrewText(string text)
    {
        foreach (char c in text)
        {
            if (c >= '\u0590' && c <= '\u05FF')
            {
                return true;
            }
        }
        return false;
    }

    // Initializes and displays the question content
    public void Initialize(string _type, string questionText, string[] multiple_answers, string _img)
    {
        question_text.isRightToLeftText = IsHebrewText(questionText);
        Debug.Log("✅ QuestionItem initialized and displayed!");
        Debug.Log("Image path before strip: " + (_img ?? "null"));

        // Remove .png or other extension from image filename (for Resources.Load)
        if (!string.IsNullOrEmpty(_img))
            _img = System.IO.Path.GetFileNameWithoutExtension(_img);

        Debug.Log("Image path after strip: " + _img);
        // Handle multiple choice setup
        if (_type == "multiple_choice")
        {
            q_Type = Q_type.multiple_choice;

            if (!string.IsNullOrEmpty(_img))
            {
                Sprite newSprite1 = Resources.Load<Sprite>(_img);
                Debug.Log("Sprite: " + newSprite1);
                if (newSprite1 != null)
                {
                    img.sprite = newSprite1;
                    img.gameObject.SetActive(true);
                }
                else
                {
                    img.gameObject.SetActive(false);
                }
            }
            else
            {
                img.gameObject.SetActive(false);
            }
            // Populate multiple choice options
            if (multiple_answers != null && multiple_answers.Length > 0)
            {
                for (int i = 0; i < multiple_answers.Length && i < toggles.Length; i++)
                {
                    toggles[i].GetComponentInChildren<TextMeshProUGUI>().text = multiple_answers[i];
                }
            }

            multiplePanel.SetActive(true);
        }
        else
        {
            q_Type = Q_type.open;
            openPanel.SetActive(true);
        }
        // Handle open question setup
        question_text.text = questionText;
        // Load and assign image again (fallback)
        if (!string.IsNullOrEmpty(_img))
        {
            Sprite newSprite = Resources.Load<Sprite>(_img);
            if (newSprite != null)
            {
                img.sprite = newSprite;
                img.gameObject.SetActive(true);
            }
            else
            {
                img.gameObject.SetActive(false);
            }
        }
        else
        {
            img.gameObject.SetActive(false);
        }
        // Finalize panel display
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
