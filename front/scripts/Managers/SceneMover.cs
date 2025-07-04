using UnityEngine;
using UnityEngine.SceneManagement;

// Static helper class for changing scenes or quitting the game
public class SceneMover : MonoBehaviour
{
        // Loads a scene by name
    public static void LoadScene(string sceneName)
    {
        Debug.Log($"Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    // Quits the game (only works in build, not in editor)
    public static void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
