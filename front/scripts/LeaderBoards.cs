using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
// Handles fetching and displaying leaderboard data from the server
public class LeaderBoards : MonoBehaviour
{
    // URL endpoint for retrieving the leaderboard JSON data
    public string leaderboardURL = "https://mathstarz-server-1.onrender.com/users/leaderboard";
    // Prefab for each leaderboard row (UI element)
    public GameObject leaderboardItemPrefab;
    // Parent container that will hold all leaderboard items
    public Transform leaderboardContainer;
    // Represents a single user's data in the leaderboard
    [System.Serializable]
    public class UserEntry
    {
        public string username;
        public int totalPoints;
    }
    // Wrapper class to deserialize the list of users from JSON
    [System.Serializable]
    public class UserList
    {
        public List<UserEntry> users;
    }

    // Only used by MainMenu to fetch leaderboard
    public IEnumerator FetchLeaderboard()
    {
        UnityWebRequest request = UnityWebRequest.Get(leaderboardURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"users\":" + request.downloadHandler.text + "}";
            UserList userList = JsonUtility.FromJson<UserList>(json);

            // Clear old entries
            foreach (Transform child in leaderboardContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < userList.users.Count; i++)
            {
                UserEntry user = userList.users[i];
                GameObject item = Instantiate(leaderboardItemPrefab, leaderboardContainer);
                item.GetComponent<LeaderboardItem>().SetData(i + 1, user.username, user.totalPoints);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch leaderboard: " + request.error);
        }
    }
}
