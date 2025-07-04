using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// Represents a single entry (row) in the leaderboard UI.
// Displays rank, username, and score for a player.
public class LeaderboardItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI scoreText;
    // Sets the data for this leaderboard item (rank, username, and score)
    public void SetData(int rank, string username, int score)
    {
        rankText.text = rank.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
        scoreText.alignment = TextAlignmentOptions.MidlineRight; 
    }


}


