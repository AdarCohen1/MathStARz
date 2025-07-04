using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Controls NPC logic, including dialog flow, question handling, and puzzle progress tracking.
public class NPCbehaviour : MonoBehaviour
{
    [SerializeField] private int q_index = 0;
    [SerializeField] private string[] chatLines;        
    [SerializeField] private string[] end_chatLines;  
    [SerializeField] public string[] questionID;  
    [SerializeField] public string npcId;
    [SerializeField] public string puzzleId;

    // Called on object creation – registers this puzzle instance for the user
    void Awake()
    {
        string userId = GameData.Instance.GetUserData().id;
        string fullPuzzleId = userId + "_" + puzzleId;
        PuzzleManager.Instance.AddPuzzle(fullPuzzleId);
        Debug.Log($"🕒 Waiting for puzzle data to initialize q_index for NPC {npcId}");
    }
    // Initializes the question index based on the number of collected puzzle pieces
    public void InitializeQuestionIndex()
    {
        if (GameData.Instance == null || GameData.Instance.GetUserData() == null)
        {
            Debug.LogError("❌ GameData.Instance or UserData is null. Cannot initialize question index.");
            return;
        }

        string userId = GameData.Instance.GetUserData().id;
        string fullPuzzleId = $"{userId}_{puzzleId}";
        string key = $"USER_{userId}_NPC_{npcId}_qIndex";
        int piecesCollected = PuzzleManager.Instance.GetPieces(fullPuzzleId);

        q_index = Mathf.Min(piecesCollected, questionID.Length);
        PlayerPrefs.SetInt(key, q_index);
        PlayerPrefs.Save();

        Debug.Log($"🧠 NPC {npcId}: puzzle {puzzleId} → q_index = {q_index} (pieces = {piecesCollected})");
    }
    // Returns a localized and formatted string from a translation key
    private string Translate(string key, params object[] args)
    {
        string raw = LanguageManager.Instance?.GetTranslation(key) ?? key;
        string formatted;

        try
        {
            formatted = (args != null && args.Length > 0) ? string.Format(raw, args) : raw;
        }
        catch (System.FormatException e)
        {
            Debug.LogWarning($"⚠️ Format error for key '{key}' with value '{raw}' → {e.Message}");
            formatted = raw;
        }

        if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
        {
            return MainMenu.ReverseHebrew(formatted);
        }

        return formatted;
    }
    // Returns the appropriate chat lines (either main or end), localized and personalized
    public string[] getChatLines()
    {
        bool more = hasMoreQuestions();
        string[] keys = more ? chatLines : end_chatLines;
        if (keys == null || keys.Length == 0) return new string[] { "..." };

        string[] translated = new string[keys.Length];
        string name = GameData.Instance?.GetUserData()?.firstName ?? "Player";

        for (int i = 0; i < keys.Length; i++)
        {
            if (more && i == 0 && keys[i] == "npc_hello")
                translated[i] = Translate("npc_hello", name);
            else
                translated[i] = Translate(keys[i]);
        }

        return translated;
    }
    // Returns the puzzle ID associated with this NPC
    public string GetPuzzleId()
    {
        return puzzleId;
    }
    // Returns the current question ID based on the player's progress
    public string GetQuestionID()
    {
        return questionID[q_index];
    }
    // Increments the question index and saves progress locally
    public void increaseQuestionIndex()
    {
        if (hasMoreQuestions())
        {
            q_index++;
            PlayerPrefs.SetInt($"NPC_{npcId}_qIndex", q_index);
            PlayerPrefs.Save();
        }
    }
    // Checks whether there are more questions left for this NPC
    public bool hasMoreQuestions()
    {
        return q_index < questionID.Length;
    }
    // Called when the NPC is tapped – triggers interaction through the player controller
    public void OnTapped()
    {
        Debug.Log("🧠 NPC was tapped!");
        PlayerBehaviour player = FindAnyObjectByType<PlayerBehaviour>();
        if (player != null)
        {
            player.ManageNPCFromOutside(this.gameObject);
        }
    }
}
