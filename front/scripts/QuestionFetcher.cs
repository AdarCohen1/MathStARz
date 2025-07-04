using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Responsible for fetching question data from the server using a given question ID.
public class QuestionFetcher : MonoBehaviour
{
    public string questionId = "1";
    string baseUrl = "https://mathstarz-server-1.onrender.com/questions/";

    // Coroutine to fetch a question by ID from the server
    // Calls the provided callback with a parsed QuestionData object
    public IEnumerator GetQuestionById(string qid, Action<QuestionData> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + qid);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to fetch question: " + request.error);
            callback(null); // signal failure
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("📦 Raw JSON: " + json);

            QuestionData question = JsonUtility.FromJson<QuestionData>(json);
            callback(question);
        }
    }
    // (Optional) Local method for testing or debugging received question data
    void OnQuestionReceived(QuestionData question)
    {
        if (question == null)
        {
            Debug.LogWarning("⚠️ Failed to receive question.");
            return;
        }

        Debug.Log("✅ Question text: " + question.question_text);
        Debug.Log("✅ Options: " + string.Join(", ", question.options));
      
    }
}
