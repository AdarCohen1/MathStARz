using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Loads the user's puzzle progress when this GameObject starts.
// Useful for initializing the puzzle collection screen.
public class PuzzleCollectionLoader : MonoBehaviour
{
    void Start()
    {
        // Get the current user's ID from the GameData singleton
        int userId = int.Parse(GameData.Instance.GetUserData().id);
        // Find all PuzzleUI elements in the scene (or could be assigned manually)       
        PuzzleUI[] allPuzzleUIs = FindObjectsOfType<PuzzleUI>();
        // Load and update each puzzle's visual state based on user progress         
        PuzzleManager.Instance.LoadUserPuzzles(userId, allPuzzleUIs);
    }

}
