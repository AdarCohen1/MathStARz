using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Singleton manager that handles puzzle tracking for each user.
// Keeps track of how many pieces were collected per puzzle and syncs with backend.
public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;


    [SerializeField]
    private List<PuzzleEntry> puzzleList = new List<PuzzleEntry>();
    // Ensures only one PuzzleManager exists across scenes
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Adds a new puzzle to the internal list (if not already added)
    public void AddPuzzle(string id)
    {
        if (!ContainsPuzzle(id))
        {
            Puzzle newPuzzle = new Puzzle(id);
            puzzleList.Add(new PuzzleEntry(id, newPuzzle));
            Debug.Log($"Puzzle {id} added.");
        }
    }
    // Adds a piece to a puzzle and saves it
    public void GivePiece(string id)
    {
        Puzzle puzzle = GetPuzzle(id);
        if (puzzle != null)
        {
            puzzle.AddPiece();
            Debug.Log($"Puzzle {id}: now has {puzzle.piecesCollected} pieces.");
        }
        else
        {
            Debug.LogWarning($"Puzzle ID '{id}' not found. Did you forget to call AddPuzzle()?");
        }
    }

    // Returns how many pieces have been collected for a given puzzle ID
    public int GetPieces(string id)
    {
        Puzzle puzzle = GetPuzzle(id);
        return puzzle != null ? puzzle.piecesCollected : 0;
    }
    // Checks whether a puzzle is complete (all pieces collected)
    public bool IsPuzzleComplete(string id)
    {
        Puzzle puzzle = GetPuzzle(id);
        return puzzle != null && puzzle.IsComplete();
    }
    // Retrieves a Puzzle object by ID
    private Puzzle GetPuzzle(string id)
    {
        foreach (var entry in puzzleList)
        {
            if (entry.puzzleId == id)
                return entry.puzzle;
        }
        return null;
    }
    // Checks if a puzzle with the given ID exists in the list
    private bool ContainsPuzzle(string id)
    {
        foreach (var entry in puzzleList)
        {
            if (entry.puzzleId == id)
                return true;
        }
        return false;
    }
        // Public method to begin loading puzzle progress for a given user
    public void LoadUserPuzzles(int userId, PuzzleUI[] puzzleUIs)
    {
        StartCoroutine(GetPuzzleData(userId, puzzleUIs));
    }
    // Coroutine to fetch puzzle progress data from server and update internal state + UI
    private IEnumerator GetPuzzleData(int userId, PuzzleUI[] puzzleUIs)
    {
        string url = $"https://mathstarz-server-1.onrender.com/puzzles/user?userId={userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawJson = "{\"items\":" + request.downloadHandler.text + "}";
            PuzzleProgressList list = JsonUtility.FromJson<PuzzleProgressList>(rawJson);

            foreach (var puzzle in list.items)
            {
                Debug.Log($"✅ Loaded Puzzle {puzzle.puzzleId} for user {puzzle.userId} with {puzzle.piecesCollected} pieces");

                // 🧠 Use consistent format: "userId_puzzleId"
                string fullId = $"{puzzle.userId}_{puzzle.puzzleId}";

                AddPuzzle(fullId);
                Puzzle p = GetPuzzle(fullId);
                if (p != null)
                {
                    p.piecesCollected = puzzle.piecesCollected;
                    p.Save();
                }

                // UI Update (if needed)
                foreach (PuzzleUI ui in puzzleUIs)
                {
                    string expectedId = $"{userId}_{ui.puzzleId}";

                    if (expectedId == fullId)
                    {
                        ui.UpdateUI();
                        break;
                    }
                }


            }

            // ✅ After puzzle data loaded, update NPCs
            NPCbehaviour[] npcs = FindObjectsOfType<NPCbehaviour>();
            foreach (var npc in npcs)
            {
                npc.InitializeQuestionIndex();
            }
        }
        else
        {
            Debug.LogError("❌ Failed to load puzzle data: " + request.error);
        }
    }


}
// Data structure pairing a puzzle ID with its Puzzle object
[System.Serializable]
public class PuzzleEntry
{
    public string puzzleId;
    public Puzzle puzzle;

    public PuzzleEntry(string id, Puzzle puzzle)
    {
        this.puzzleId = id;
        this.puzzle = puzzle;
    }
}
// Holds server puzzle progress information
[System.Serializable]
public class PuzzleProgress
{
    public int puzzleId;
    public int userId;
    public int piecesCollected;
}
// Wrapper for a list of puzzle progress records (used for JSON parsing)
[System.Serializable]
public class PuzzleProgressList
{
    public PuzzleProgress[] items;
}

