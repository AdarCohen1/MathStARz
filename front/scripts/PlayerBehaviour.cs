using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Handles player interactions with NPCs, question panels, puzzle pieces, and score updates.
public class PlayerBehaviour : MonoBehaviour
{
    public Transform holder;
    public QuestionFetcher fetcher => FindAnyObjectByType<QuestionFetcher>();

    public Collider otherTransform;
    private DialogBox dialogBox => FindAnyObjectByType<DialogBox>();
    public GameObject collectionsPanel;
    private bool justFinished = false;
    public GameObject homeButton;
    public GameObject collectionsButton;
    private bool isInNpcDialog = false;
    private int npcProgressCounter = 0;

    NPCbehaviour nPCbehaviour;

    // Local struct for backend puzzle progress data
    [System.Serializable]
    public class PuzzleData
    {
        public int userId;
        public int puzzleId;
        public int piecesCollected;
    }

    public bool finish = false;
    public bool isQuestionOpen = false;

    // Handle touch or mouse input each frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("🔵 Touch on UI — ignoring object detection.");
                return;
            }

            DetectObject(Input.GetTouch(0).position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("🔵 Click on UI — ignoring object detection.");
                return;
            }

            DetectObject(Input.mousePosition);
        }
    }
    // On start, load user data and puzzle progress
    void Start()
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("❌ GameData.Instance is null.");
            return;
        }

        var userData = GameData.Instance.GetUserData();
        if (userData == null)
        {
            Debug.LogError("❌ GameData.GetUserData() returned null.");
            return;
        }

        if (string.IsNullOrEmpty(userData.id))
        {
            Debug.LogError("❌ UserData.id is null or empty.");
            return;
        }

        int userId = int.Parse(userData.id);

        // ⬇️ Load npcProgressCounter from PlayerPrefs
        string key = $"NPC_PROGRESS_COUNTER_USER_{userId}";
        npcProgressCounter = PlayerPrefs.GetInt(key, 0);
        Debug.Log($"📥 Loaded NPC progress counter = {npcProgressCounter}");

        PuzzleUI[] allPuzzleUIs = FindObjectsOfType<PuzzleUI>();
        PuzzleManager.Instance.LoadUserPuzzles(userId, allPuzzleUIs);
    }

    // Raycasts to detect objects on screen tap/click
    void DetectObject(Vector2 screenPosition)
    {
        if (isQuestionOpen)
        {
            Debug.Log("🔒 Question open — skipping object detection.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit object: " + hit.collider.name);
            hit.collider.gameObject.SendMessage("OnTapped", SendMessageOptions.DontRequireReceiver);
        }
    }
    // Triggered when entering an NPC collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            Debug.Log("Entered NPC area: " + other.name);
            otherTransform = other;

            if (other.GetComponent<NPCbehaviour>() == null)
            {
                Debug.LogError("❌ Missing NPCbehaviour on " + other.name);
            }

        }
    }

    // Starts dialog with NPC when tapped externally
    public void ManageNPCFromOutside(GameObject npcObject)
    {
        otherTransform = npcObject.GetComponent<Collider>();
        if (otherTransform == null)
        {
            Debug.LogError("❌ Collider not found on NPC object.");
            return;
        }

        nPCbehaviour = npcObject.GetComponent<NPCbehaviour>();
        if (nPCbehaviour == null)
        {
            Debug.LogError("❌ NPCbehaviour component not found on: " + npcObject.name);
            return;
        }

        int npcIdInt;
        if (int.TryParse(nPCbehaviour.npcId, out npcIdInt))
        {
            if (npcIdInt != npcProgressCounter + 1)
            {
                string key = $"npc_finish_{npcProgressCounter + 1}";
                string warning = LanguageManager.Instance.GetTranslation(key);

                if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                    warning = MainMenu.ReverseHebrew(warning);

                dialogBox.chatIndex = 0;
                dialogBox.SetChat(new string[] { warning });
                return;
            }
        }

        isInNpcDialog = true;

        if (homeButton != null) homeButton.SetActive(false);
        if (collectionsButton != null) collectionsButton.SetActive(false);

        nPCbehaviour.InitializeQuestionIndex();

        dialogBox.onEndChat -= CreateQuestion;

        if (nPCbehaviour.hasMoreQuestions())
        {
            dialogBox.chatIndex = 0;
            dialogBox.onEndChat += CreateQuestion;
            dialogBox.SetChat(nPCbehaviour.getChatLines());
        }
        else
        {
            if (justFinished)
            {
                StartCoroutine(ShowEndChatWithDelay());
                justFinished = false;
            }
            else
            {
                dialogBox.chatIndex = 0;
                dialogBox.SetChat(nPCbehaviour.getChatLines());
            }

            EndNpcInteraction();
        }

    }

    // Restores UI after NPC interaction
    private void EndNpcInteraction()
    {
        isInNpcDialog = false;

        if (homeButton != null)
            homeButton.SetActive(true);

        if (collectionsButton != null)
            collectionsButton.SetActive(true);
    }
    // Called after a correct answer is submitted
    public void onCorrectAnswer()
    {
        nPCbehaviour.increaseQuestionIndex();

        if (nPCbehaviour.hasMoreQuestions())
        {
            CreateQuestion();
        }
        else
        {
            finish = true;
            justFinished = true;
            dialogBox.onEndChat -= CreateQuestion;
            dialogBox.chatIndex = 0;
            StartCoroutine(ShowEndChatWithDelay());

            isQuestionOpen = false;
            EndNpcInteraction();

            // ⬇️ Save NPC progress counter
            npcProgressCounter++;
            string userId = GameData.Instance.GetUserData().id;
            string key = $"NPC_PROGRESS_COUNTER_USER_{userId}";
            PlayerPrefs.SetInt(key, npcProgressCounter);
            PlayerPrefs.Save();
            Debug.Log($"💾 Saved NPC progress counter = {npcProgressCounter}");
        }

        string uId = GameData.Instance.GetUserData().id;
        string npcPuzzleId = nPCbehaviour.GetPuzzleId();
        PuzzleManager.Instance.GivePiece(uId + "_" + npcPuzzleId);

        StartCoroutine(SendPuzzlePieceUpdate());
    }

    // Displays end-of-chat lines after a short delay
    private IEnumerator ShowEndChatWithDelay()
    {
        yield return new WaitForSeconds(2f);
        dialogBox.chatIndex = 0;
        dialogBox.SetChat(nPCbehaviour.getChatLines());
    }
    // Sends updated puzzle progress to the backend
    public IEnumerator SendPuzzlePieceUpdate()
    {
        int userId = int.Parse(GameData.Instance.GetUserData().id);
        string puzzleId = nPCbehaviour.GetPuzzleId();

        string fullId = $"{userId}_{puzzleId}";
        int pieces = PuzzleManager.Instance.GetPieces(fullId);

        PuzzleData payload = new PuzzleData
        {
            userId = userId,
            puzzleId = int.Parse(puzzleId),
            piecesCollected = pieces
        };

        string jsonData = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest("https://mathstarz-server-1.onrender.com/puzzles/update", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"🧩 Puzzle progress for {fullId} sent to backend successfully.");
        }
        else
        {
            Debug.LogError("❌ Failed to send puzzle progress: " + request.error);
        }
    }
    // Instantiates a new question UI and fetches its data
    public void CreateQuestion()
    {
        isQuestionOpen = true;
        QuestionController newQuestion = CreateNewQuestion();
        StartCoroutine(fetcher.GetQuestionById(otherTransform.GetComponent<NPCbehaviour>().GetQuestionID(), newQuestion.Initilize));
        newQuestion.onCorrect += onCorrectAnswer;
    }
    // Cleans up on exiting NPC area
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            dialogBox.onEndChat -= CreateQuestion;
            Debug.Log("Exited NPC area.");
        }
    }
    // Instantiates and initializes the question UI prefab
    public QuestionController CreateNewQuestion()
    {
        Debug.Log("🟢 CreateQuestion() called");
        if (homeButton != null) homeButton.SetActive(false);
        if (collectionsButton != null) collectionsButton.SetActive(false);

        GameObject prefab = Resources.Load("QuestionItem") as GameObject;

        if (prefab == null)
        {
            Debug.LogError("❌ Prefab QuestionItem not found in Resources folder!");
            return null;
        }

        GameObject newQuestion = Instantiate(prefab, Vector2.zero, Quaternion.identity, holder);
        newQuestion.transform.localPosition = Vector3.zero;
        newQuestion.transform.localScale = Vector3.one;
        newQuestion.transform.localRotation = Quaternion.identity;
        newQuestion.SetActive(true);
        dialogBox.onEndChat -= CreateQuestion;

        QuestionController controller = newQuestion.GetComponent<QuestionController>();
        controller.view = newQuestion.GetComponent<QuestionItem>();
        if (controller.view != null && controller.view.exit != null)
        {
            controller.view.exit.onClick.RemoveAllListeners();
            controller.view.exit.onClick.AddListener(() =>
            {
                if (homeButton != null) homeButton.SetActive(true);
                if (collectionsButton != null) collectionsButton.SetActive(true);
                newQuestion.SetActive(false);
                isQuestionOpen = false;
            });
        }
        else
        {
            Debug.LogWarning("⚠️ Could not hook exit button — controller.view or exit is null.");
        }
        return controller;
    }

    // Updates the user's score and shape-specific stats, then sends them to the backend
    public void UpdateUserScore(int newScore, string shape)
    {
        if (GameData.Instance != null)
        {
            var user = GameData.Instance.GetUserData();

            user.totalPoints += newScore;

            if (user.shapes == null)
                user.shapes = new ShapeStats();

            switch (shape.ToLower())
            {
                case "triangle":
                    user.shapes.triangle += newScore;
                    break;

                case "square":
                    user.shapes.square += newScore;
                    break;

                default:
                    Debug.LogWarning("Unknown shape: " + shape);
                    break;
            }

            StartCoroutine(SendScoreUpdate(user));
        }
        else
        {
            Debug.LogWarning("UserData not loaded, can't update score.");
        }
    }
    // Sends updated user score data to the backend
    private IEnumerator SendScoreUpdate(UserData updatedUser)
    {
        string url = "https://mathstarz-server-1.onrender.com/users/update";

        string jsonData = JsonUtility.ToJson(updatedUser);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User score updated successfully.");
        }
        else
        {
            Debug.LogError("Failed to update user score: " + request.error);
        }
    }
    // Returns to login screen
    public void OnHomeButtonClicked()
    {
        Debug.Log("Home button clicked — loading LoginScene and setting screen 1.");
        MainMenu.loginScreenToShow = 1;
        SceneManager.LoadScene("LoginScene");
    }
    // Shows or hides the collection panel
    public void ToggleCollectionsPanel()
    {
        if (collectionsPanel != null)
        {
            bool isActive = collectionsPanel.activeSelf;
            collectionsPanel.SetActive(!isActive);

            if (!isActive)
            {
                int userId = int.Parse(GameData.Instance.GetUserData().id);
                PuzzleUI[] allPuzzleUIs = FindObjectsOfType<PuzzleUI>();
                PuzzleManager.Instance.LoadUserPuzzles(userId, allPuzzleUIs);
            }
        }
    }
    // Destroys the active question panel
    public void CloseQuestionPanel()
    {
        if (holder.childCount > 0)
        {
            Destroy(holder.GetChild(0).gameObject);
            isQuestionOpen = false;
            Debug.Log("❌ Question panel closed.");
        }
    }
}
