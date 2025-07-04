using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// GameData is a singleton that stores and persists user data across scenes.
// Use this class to get or set the current user's data during gameplay.
public class GameData : MonoBehaviour
{
    // Static instance for the singleton pattern
    public static GameData Instance { get; private set; }
    // Holds the current user's data
    [SerializeField] private UserData userData;
    // Ensures only one instance exists and persists it between scenes
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }
    // Returns the current user data
    public UserData GetUserData()
    {
        return userData;
    }
    // Updates the current user data
    public void SetUserData(UserData data)
    {
        userData = data;
    }
}
