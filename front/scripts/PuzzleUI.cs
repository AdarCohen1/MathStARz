using UnityEngine;
using UnityEngine.UI;

// Controls the visual representation of a puzzle in the UI.
// Displays how many puzzle pieces have been collected and updates when progress changes.
public class PuzzleUI : MonoBehaviour
{
    public string puzzleId;
    [SerializeField] private Image[] pieces; // Assign 4 pieces in Inspector
    // On start, update the visual state to reflect saved progress
    private void Start()
    {
        UpdateUI(); // Show collected pieces on load
    }
    // Updates the UI based on the number of collected pieces
    public void UpdateUI()
    {
        // Construct the full puzzle ID based on user and puzzle
        string userId = GameData.Instance.GetUserData().id;
        string fullPuzzleId = $"{userId}_{puzzleId}";

        // Get number of collected pieces from PuzzleManager
        int count = PuzzleManager.Instance.GetPieces(fullPuzzleId);

        // Enable only the number of pieces collected
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].enabled = i < count;
        }
        // Log for debug purposes
        Debug.Log($"🧩 PuzzleUI {fullPuzzleId}: showing {count} pieces");
    }

    // Called when the player answers a question correctly
    public void OnCorrectAnswer()
    {
        PuzzleManager.Instance.GivePiece(puzzleId);
        UpdateUI();
        StartCoroutine(FindAnyObjectByType<PlayerBehaviour>().SendPuzzlePieceUpdate());
    }
    // Allows manually setting the number of pieces shown (e.g. from code)
    public void SetPieces(int count)
    {
        for (int i = 0; i < pieces.Length; i++)
            pieces[i].enabled = i < count;
    }

}
