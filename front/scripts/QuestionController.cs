using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controls logic for rendering a question, checking answers, and updating the user score.
// Supports both open and multiple choice question types, with multilingual translation.
public class QuestionController : MonoBehaviour
{
    public QuestionData data;
    public QuestionItem view;

    public PlayerBehaviour player => FindAnyObjectByType<PlayerBehaviour>();

    public Action onCorrect;
    // Initializes the question UI with translated content
    public void Initilize(QuestionData newData)
    {
        data = newData;
        // Translate the question text
        string key = LanguageManager.Instance.FindKeyByEnglishValue(data.question_text);
        string translatedQuestion = key != null ? LanguageManager.Instance.GetTranslation(key) : data.question_text;
        // Translate answer options (if applicable)
        string[] translatedOptions = null;
        if (data.options != null)
        {
            translatedOptions = new string[data.options.Length];
            for (int i = 0; i < data.options.Length; i++)
            {
                string optKey = LanguageManager.Instance.FindKeyByEnglishValue(data.options[i]);
                translatedOptions[i] = optKey != null ? LanguageManager.Instance.GetTranslation(optKey) : data.options[i];
            }
        }
        // Initialize the view with the translated content
        view.Initialize(
            data.question_type,
            translatedQuestion,
            translatedOptions,
            data.image_filename
        );
        // Register the submit button
        view.submit.onClick.AddListener(OnSubmit);
    }

    // Called when the user submits an answer
    public void OnSubmit()
    {
        switch (view.q_Type)
        {
            case QuestionItem.Q_type.open:
                view.wrong.gameObject.SetActive(false);
                OnOpen();
                break;

            case QuestionItem.Q_type.multiple_choice:
                view.wrong.gameObject.SetActive(false);
                OnMultiple();
                break;
        }
    }
    // Handles multiple choice answer checking
    public void OnMultiple()
    {
        string answer = "";

        foreach (var toggle in view.toggles)
        {
            if (toggle.isOn)
            {
                answer = toggle.GetComponentInChildren<TextMeshProUGUI>().text;
            }
        }

        if (isCorrect(answer))
        {
            // Display correct answer feedback
            string correctText = LanguageManager.Instance.GetTranslation("correct_answer");
            if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                correctText = "!" + MainMenu.ReverseHebrew(correctText);

            view.correct.isRightToLeftText = false;
            view.correct.text = correctText;
            view.correct.gameObject.SetActive(true);
            Debug.Log("Correct!");
            player.UpdateUserScore(data.points, data.shape);
            onCorrect?.Invoke();

            if (!player.otherTransform.GetComponent<NPCbehaviour>().hasMoreQuestions())
                Destroy(gameObject, 2f);
            else
                Destroy(gameObject, 2f);
        }
        else
        {
            // Display wrong answer feedback            
            string wrongText = LanguageManager.Instance.GetTranslation("wrong_answer");
            if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                wrongText = "!" + MainMenu.ReverseHebrew(wrongText);

            view.wrong.isRightToLeftText = false;
            view.wrong.text = wrongText;
            view.wrong.gameObject.SetActive(true);
            Debug.Log("Wrong!");
        }
    }

    // Handles open text answer checking
    public void OnOpen()
    {
        string answer = view.answerInput.text;

        if (isCorrect(answer))
        {
            string correctText = LanguageManager.Instance.GetTranslation("correct_answer");
            if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                correctText = "!" + MainMenu.ReverseHebrew(correctText);

            view.correct.isRightToLeftText = false;
            view.correct.text = correctText;
            view.correct.gameObject.SetActive(true);
            Debug.Log("Correct!");
            player.UpdateUserScore(data.points, data.shape);
            onCorrect?.Invoke();
            Destroy(gameObject, 2f);
        }
        else
        {
            string wrongText = LanguageManager.Instance.GetTranslation("wrong_answer");
            if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                wrongText = "!" + MainMenu.ReverseHebrew(wrongText);

            view.wrong.isRightToLeftText = false;
            view.wrong.text = wrongText;
            view.wrong.gameObject.SetActive(true);
            Debug.Log("Wrong!");
        }
    }
    // Compares user answer with the correct answer
    public bool isCorrect(string answer)
    {
        return data.answer == answer;
    }
}
