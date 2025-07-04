using UnityEngine;

// Represents a single puzzle and tracks how many pieces the user has collected.
// Each puzzle has a maximum number of collectible pieces.
[System.Serializable]
public class Puzzle
{
    public string puzzleId;
    public int piecesCollected;

    public const int maxPieces = 4;
    // Constructor to create a new puzzle instance
    public Puzzle(string id)
    {
        puzzleId = id;
        piecesCollected = 0; 
    }

    // Returns true if the puzzle is fully completed
    public bool IsComplete()
    {
        return piecesCollected >= maxPieces;
    }
    // Increments the piece count if not already at max; optionally saves the state
    public void AddPiece(bool save = true)
    {
        if (piecesCollected < maxPieces)
        {
            piecesCollected++;
            if (save)
                Save();
        }
    }

    // Saves the current number of pieces to PlayerPrefs
    public void Save()
    {
        PlayerPrefs.SetInt(GetKey(), piecesCollected);
        PlayerPrefs.Save();
    }
    // Returns the key used to store puzzle progress in PlayerPrefs
    private string GetKey()
    {
        return $"Puzzle_{puzzleId}_Pieces";
    }
}
