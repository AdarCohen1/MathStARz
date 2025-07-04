using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// Handles showing a dialog box with multiple lines of text.
// Useful for NPC interactions or instructions in the game.
public class DialogBox : MonoBehaviour
{
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    private string[] chat;
    public int chatIndex;

    public Action onEndChat;
// Initialize the dialog index when the script starts
    void Start()
    {
        chatIndex = 0;
    }

    // Call this to set the chat and begin displaying the first line
    public void SetChat(string[] _chat)
    {
        if (_chat == null || _chat.Length == 0)
        {
            Debug.LogWarning("🟡 Tried to set empty chat lines. Skipping dialog.");
            ToggleDialogBox(false);
            onEndChat?.Invoke();
            return;
        }

        chat = _chat;
        chatIndex = 0;

        ToggleDialogBox(true);
        dialogText.text = chat[chatIndex];
    }

    // Call this to move to the next line in the dialog
    public void NextChatLine()
    {
        if (chat == null || chat.Length == 0)
        {
            ToggleDialogBox(false);
            return;
        }

        chatIndex++;

        if (chatIndex < chat.Length)
        {
            dialogText.text = chat[chatIndex];
        }
        else
        {
            ToggleDialogBox(false);
            onEndChat?.Invoke();
        }
    }
    // Shows or hides the dialog box GameObject
    private void ToggleDialogBox(bool show)
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(show);
        }
        else
        {
            Debug.LogWarning("DialogBox GameObject not assigned.");
        }
    }
}
