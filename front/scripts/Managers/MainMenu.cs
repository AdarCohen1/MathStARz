using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class MainMenu : MonoBehaviour
{
    public GameObject[] screens;
    [SerializeField] private TextMeshProUGUI frameTxt;
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject newStudentPanel;
    [SerializeField] private GameObject newTeacherPanel;
    // Student input fields
    [SerializeField] private TMP_InputField studentFirstNameInput;
    [SerializeField] private TMP_InputField studentLastNameInput;
    [SerializeField] private TMP_InputField studentUsernameInput;
    [SerializeField] private TMP_InputField studentPasswordInput;

    // Teacher input fields
    [SerializeField] private TMP_InputField teacherFirstNameInput;
    [SerializeField] private TMP_InputField teacherLastNameInput;
    [SerializeField] private TMP_InputField teacherUsernameInput;
    [SerializeField] private TMP_InputField teacherPasswordInput;
    [SerializeField] private TMP_InputField studentIdInput;
    [SerializeField] private TMP_InputField teacherIdInput;
    [SerializeField] private GameObject idWrongLabel; // Assign in Unity
    [SerializeField] private GameObject studentDataPanel;

    [SerializeField] private TMP_InputField checkIdInput;
    [SerializeField] private TMP_Text studentShapeDataText;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private Sprite speakerOnIcon;
    [SerializeField] private Sprite speakerOffIcon;
    [SerializeField] private Image volumeButtonImage;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject buttons;
    [SerializeField] private TMP_Text wrongIDLabel, wrongUsernameLabel,fillInEnglish;
    [SerializeField] private TMP_Text studentFirstNameText;
    [SerializeField] private TMP_Text studentLastNameText;
    [SerializeField] private TMP_Text studentUsernameText;
    [SerializeField] private TMP_Text studentPasswordText;
    [SerializeField] private TMP_Text studentIdText;
    [SerializeField] private TMP_Text loginUsernameText, loginPasswordText, teacherFirstNameText, teacherLastNameText, teacherUsernameText, teacherPasswordText, teacherIdText;



    [System.Serializable]
    public class RegisterUserRequest
    {
        public string id;
        public string firstName;
        public string lastName;
        public string username;
        public string password;
        public int userType;
    }
    [System.Serializable]
    public class ShapeStats
    {
        public int triangle;
        public int circle;
        public int square;
    }

    [System.Serializable]
    public class StudentShapeData
    {
        public string id;
        public int totalPoints;
        public ShapeStats shapes;
    }


    public LeaderBoards leaderboards;

    private bool isSettingsOpen = false;
    private bool isLeaderboardOpen = false;
    private bool leaderboardLoaded = false;
    private float lastVolumeBeforeMute = 1f;
    public static int loginScreenToShow = 0;

    // Called on scene start. Initializes settings panel, volume, language direction, and localized labels.
    public void Start()
    {
        buttons.SetActive(false);

        if (studentDataPanel != null)
            studentDataPanel.SetActive(false);

        // Set initial volume and icon
        float volume = AudioListener.volume;
        volumeSlider.value = volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeButtonImage.sprite = volume == 0f ? speakerOffIcon : speakerOnIcon;

        // Optional: Reset after use
        loginScreenToShow = 0;

        if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
            ApplyTextDirectionByLanguage();
        LocalizedLabel[] allLabels = FindObjectsOfType<LocalizedLabel>(true);
        foreach (var label in allLabels)
            label.SendMessage("UpdateText", SendMessageOptions.DontRequireReceiver);
    }
// Called when the GameObject becomes active. Sets up listeners and ensures UI panels are closed.
    void OnEnable()
    {
        if (loginManager == null)
            loginManager = FindAnyObjectByType<LoginManager>();

        loginManager.onLogin -= displayData;
        loginManager.onLogin += displayData;
        LanguageManager.OnLanguageChanged -= ApplyTextDirectionByLanguage;
        LanguageManager.OnLanguageChanged += ApplyTextDirectionByLanguage;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (studentDataPanel != null)
            studentDataPanel.SetActive(false);
        if (newStudentPanel != null)
            newStudentPanel.SetActive(false);

        if (newTeacherPanel != null)
            newTeacherPanel.SetActive(false);

        studentFirstNameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(studentFirstNameInput, text));
        studentLastNameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(studentLastNameInput, text));
        studentUsernameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(studentUsernameInput, text));
        studentIdInput.onValueChanged.AddListener((text) => AdjustInputAlignment(studentIdInput, text));
        studentPasswordInput.onValueChanged.AddListener((text) => AdjustInputAlignment(studentPasswordInput, text));

        teacherFirstNameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(teacherFirstNameInput, text));
        teacherLastNameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(teacherLastNameInput, text));
        teacherUsernameInput.onValueChanged.AddListener((text) => AdjustInputAlignment(teacherUsernameInput, text));
        teacherIdInput.onValueChanged.AddListener((text) => AdjustInputAlignment(teacherIdInput, text));
        teacherPasswordInput.onValueChanged.AddListener((text) => AdjustInputAlignment(teacherPasswordInput, text));

    }
// Called when the GameObject becomes inactive. Unsubscribes from language change events.
    void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= ApplyTextDirectionByLanguage;
    }
    // Updates text direction based on the current language (e.g., right-to-left for Hebrew).
    private void ApplyTextDirectionByLanguage()
    {
        bool isRTL = LanguageManager.Instance.CurrentLanguage == "Hebrew";

        SetTextDirection(studentFirstNameText, isRTL);
        SetTextDirection(studentLastNameText, isRTL);
        SetTextDirection(studentUsernameText, isRTL);
        SetTextDirection(studentPasswordText, isRTL);
        SetTextDirection(studentIdText, isRTL);

        SetTextDirection(loginUsernameText, isRTL);
        SetTextDirection(loginPasswordText, isRTL);

        SetTextDirection(teacherFirstNameText, isRTL);
        SetTextDirection(teacherLastNameText, isRTL);
        SetTextDirection(teacherUsernameText, isRTL);
        SetTextDirection(teacherPasswordText, isRTL);
        SetTextDirection(teacherIdText, isRTL);
    }

// Helper to apply RTL or LTR text settings on a given TMP_Text field.
    private void SetTextDirection(TMP_Text textComponent, bool isRTL)
    {
        if (textComponent != null)
        {
            textComponent.isRightToLeftText = isRTL;
            // textComponent.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        }
    }
// Aligns input text based on the detected language direction (Hebrew vs. English).
    public void AdjustInputAlignment(TMP_InputField inputField, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        bool hasHebrew = text.Any(c => c >= 0x0590 && c <= 0x05FF);

        inputField.textComponent.isRightToLeftText = hasHebrew;
        inputField.textComponent.alignment = hasHebrew ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
    }

// Validates and submits a new student registration request after checking all fields.
    public void SubmitNewStudent()
    {
        string id = studentIdInput.text.Trim();
        string username = studentUsernameInput.text.Trim();

        if (!IsDigitsOnly(id))
        {
            wrongIDLabel.text = LanguageManager.Instance.GetTranslation("id_digits_only");
            wrongIDLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongIDLabel.gameObject, 3f));
            return;
        }

        if (!IsValidUsername(username))
        {
            wrongUsernameLabel.text = LanguageManager.Instance.GetTranslation("username_invalid");
            wrongUsernameLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongUsernameLabel.gameObject, 3f));
            return;
        }

        RegisterUserRequest student = new RegisterUserRequest
        {
            id = id,
            firstName = studentFirstNameInput.text,
            lastName = studentLastNameInput.text,
            username = username,
            password = studentPasswordInput.text,
            userType = 0
        };


        if (!IsEnglishOnly(student.firstName) || !IsEnglishOnly(student.lastName) || !IsEnglishOnly(student.username))
        {
            string message = LanguageManager.Instance.GetTranslation("fill_in_english");
            fillInEnglish.text = message;
            fillInEnglish.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(fillInEnglish.gameObject, 3f));
            return;
        }

        StartCoroutine(ValidateAndRegister(student));
    }



    private bool IsEnglishOnly(string text)
    {
        foreach (char c in text)
        {
            if (char.IsLetter(c) && (c >= 0x0590 && c <= 0x05FF)) 
            {
                return false;
            }
        }
        return true;
    }
// Validates and submits a new teacher registration request after checking all fields.
    public void SubmitNewTeacher()
    {
        string id = teacherIdInput.text.Trim();
        string firstName = teacherFirstNameInput.text.Trim();
        string lastName = teacherLastNameInput.text.Trim();
        string username = teacherUsernameInput.text.Trim();
        string password = teacherPasswordInput.text.Trim();

        if (!IsEnglishOnly(firstName) || !IsEnglishOnly(lastName) ||
            !IsEnglishOnly(username) || !IsEnglishOnly(password) || !IsEnglishOnly(id))
        {
            string message = LanguageManager.Instance.GetTranslation("fill_in_english");
            fillInEnglish.text = message;
            fillInEnglish.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(fillInEnglish.gameObject, 3f));
            return;
        }

        if (!IsDigitsOnly(id))
        {
            wrongIDLabel.text = LanguageManager.Instance.GetTranslation("id_digits_only");
            wrongIDLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongIDLabel.gameObject, 3f));
            return;
        }

        if (!IsValidUsername(username))
        {
            wrongUsernameLabel.text = LanguageManager.Instance.GetTranslation("username_invalid");
            wrongUsernameLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongUsernameLabel.gameObject, 3f));
            return;
        }

        RegisterUserRequest teacher = new RegisterUserRequest
        {
            id = id,
            firstName = firstName,
            lastName = lastName,
            username = username,
            password = password,
            userType = 1
        };

        StartCoroutine(ValidateAndRegister(teacher));
    }

// Ensures a string contains only digits.
    private bool IsDigitsOnly(string text)
    {
        return text.All(char.IsDigit);
    }
// Validates username: must be alphanumeric and not only digits.
    private bool IsValidUsername(string username)
    {

        bool isAlphanumeric = username.All(char.IsLetterOrDigit);


        bool notOnlyDigits = username.Any(char.IsLetter);

        return isAlphanumeric && notOnlyDigits;
    }


// Checks uniqueness of ID and username before sending registration data to the server.
    private IEnumerator ValidateAndRegister(RegisterUserRequest user)
    {
        bool idExists = false;
        bool usernameExists = false;
        string idUrl = $"https://mathstarz-server-1.onrender.com/users?id={user.id}";
        UnityWebRequest idRequest = UnityWebRequest.Get(idUrl);
        idRequest.SetRequestHeader("Content-Type", "application/json");
        yield return idRequest.SendWebRequest();

        if (idRequest.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(idRequest.downloadHandler.text))
        {
            string message = LanguageManager.Instance.GetTranslation("id_label_on_register");
            wrongIDLabel.text = message;
            wrongIDLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongIDLabel.gameObject, 3f));
            idExists = true;
        }
        else
        {
            wrongIDLabel.gameObject.SetActive(false);
        }

        string usernameUrl = $"https://mathstarz-server-1.onrender.com/users?username={user.username}";
        UnityWebRequest usernameRequest = UnityWebRequest.Get(usernameUrl);
        usernameRequest.SetRequestHeader("Content-Type", "application/json");
        yield return usernameRequest.SendWebRequest();

        if (usernameRequest.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(usernameRequest.downloadHandler.text))
        {
            string message = LanguageManager.Instance.GetTranslation("username_label_on_register");
            wrongUsernameLabel.text = message;
            wrongUsernameLabel.gameObject.SetActive(true);
            StartCoroutine(HideLabelAfterSeconds(wrongUsernameLabel.gameObject, 3f));
            usernameExists = true;
        }
        else
        {
            wrongUsernameLabel.gameObject.SetActive(false);
        }
        if (!idExists && !usernameExists)
        {
            StartCoroutine(SendRegisterRequest(user));
        }
    }

// Sends user registration request to backend server.
    private IEnumerator SendRegisterRequest(RegisterUserRequest newUser)
    {
        string url = "https://mathstarz-server-1.onrender.com/users/register";
        string json = JsonUtility.ToJson(newUser);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        try
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ User registered successfully!");

                if (idWrongLabel != null)
                    idWrongLabel.SetActive(false);

                if (newUser.userType == 0)
                    ClearStudentFields();
                else if (newUser.userType == 1)
                    ClearTeacherFields();
            }
            else
            {
                long status = request.responseCode;
                string errorText = request.downloadHandler.text?.ToLower() ?? "";

                Debug.LogWarning($"❌ Register failed: {status} - {errorText}");

                // === Handle 500 and unknown server errors ===
                if (status == 500 || string.IsNullOrEmpty(errorText))
                {
                    Debug.LogWarning("Unknown registration error or internal server issue.");

                    if (idWrongLabel != null)
                    {
                        idWrongLabel.SetActive(true);
                        StartCoroutine(HideLabelAfterSeconds(idWrongLabel, 3f));
                    } // Show generic error
                    yield break;
                }

                // === Handle known duplicate ID ===
                if (errorText.Contains("duplicate") || errorText.Contains("id") || errorText.Contains("already exists"))
                {
                    if (idWrongLabel != null)
                        idWrongLabel.SetActive(true);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("⚠️ Exception during response handling: " + ex.Message);
            if (idWrongLabel != null)
                idWrongLabel.SetActive(true);
        }
    }
// Switches between UI screens based on user type or login progress.
    public void ManageScreens(int index)
    {
        Debug.Log($"🧭 Switching to screen index: {index}");

        for (int i = 0; i < screens.Length; i++)
            screens[i].SetActive(false);

        if (index < screens.Length && screens[index] != null)
        {
            screens[index].SetActive(true);
            Debug.Log($"✅ Enabled screen {index}: {screens[index].name}");
        }
        else
        {
            Debug.LogError($"❌ Invalid screen index or null screen at index {index}");
        }
    }

// Called after successful login. Displays welcome message and opens relevant user panel.
    public void displayData()
    {
        UserData user = GameData.Instance.GetUserData();
        if (user == null)
        {
            Debug.LogWarning("displayData called but no user is logged in.");
            return;
        }
        buttons.SetActive(true);
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");

        if (user.userType >= screens.Length)
        {
            Debug.LogError($"Invalid userType: {user.userType}, but only {screens.Length} screens defined.");
            return;
        }

        switch (user.userType)
        {
            case 0: ManageScreens(1); break; // student
            case 1: ManageScreens(2); break; // teacher
            case 2: ManageScreens(3); break; // admin
            default: Debug.LogError("Unknown user type."); return;
        }


        string raw = LanguageManager.Instance.GetTranslation("welcome_message");
        string formatted = string.Format(raw, user.firstName, user.totalPoints);

        if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
            formatted = ReverseHebrew(formatted);

        frameTxt.text = formatted;

    }

// Reverses Hebrew text line-by-line for proper UI rendering in LTR layout.
    public static string ReverseHebrew(string input)
    {
        string[] lines = input.Split('\n');
        List<string> reversedLines = new List<string>();

        foreach (string line in lines)
        {
            // Split to words and symbols
            var wordList = new List<string>();
            var current = new List<char>();
            bool? inHebrew = null;

            bool IsHebrew(char c) => (c >= 0x0590 && c <= 0x05FF);

            void Flush()
            {
                if (current.Count > 0)
                {
                    if (inHebrew == true)
                        current.Reverse();

                    wordList.Add(new string(current.ToArray()));
                    current.Clear();
                }
            }

            foreach (char c in line)
            {
                bool isHebrewChar = IsHebrew(c);

                // Handle punctuation as a separate "word"
                if (char.IsPunctuation(c) || char.IsWhiteSpace(c))
                {
                    Flush();
                    wordList.Add(c.ToString());
                    inHebrew = null;
                    continue;
                }

                if (inHebrew == null)
                    inHebrew = isHebrewChar;

                if (inHebrew != isHebrewChar)
                {
                    Flush();
                    inHebrew = isHebrewChar;
                }

                current.Add(c);
            }

            Flush();
            wordList.Reverse();
            reversedLines.Add(string.Join("", wordList));
        }

        return string.Join("\n", reversedLines);
    }
// Opens or closes the settings panel.
    public void ToggleSettingsPanel()
    {
        isSettingsOpen = !isSettingsOpen;
        if (settingsPanel != null)
            settingsPanel.SetActive(isSettingsOpen);
    }
// Opens or closes the leaderboard panel. Loads leaderboard data if not already loaded.
    public void ToggleLeaderboardPanel()
    {
        isLeaderboardOpen = !isLeaderboardOpen;
        leaderboardPanel.SetActive(isLeaderboardOpen);

        // Show/hide quit button
        if (quitButton != null)
            quitButton.SetActive(!isLeaderboardOpen);

        if (isLeaderboardOpen && !leaderboardLoaded)
        {
            StartCoroutine(leaderboards.FetchLeaderboard());
            leaderboardLoaded = true;
        }
    }
// Logs out the current user: clears session, returns to login, and notifies backend.
    public void LogoutUser()
    {
        string username = GameData.Instance.GetUserData()?.username;

        // Clear user data immediately on the client side
        GameData.Instance.SetUserData(null);
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.Save();

        buttons.SetActive(false);

        // Reconnect login event handler
        loginManager.onLogin -= displayData;
        loginManager.onLogin += displayData;
        settingsPanel.SetActive(false);
        leaderboardPanel.SetActive(false);

        // Go back to login screen
        ManageScreens(0);

        // Send logout request to the server in the background
        if (!string.IsNullOrEmpty(username))
            StartCoroutine(SendLogoutRequest(username));
    }
// Sends logout request to backend for the given username.
    private IEnumerator SendLogoutRequest(string username)
    {
        string url = "https://mathstarz-server-1.onrender.com/users/logout";
        string jsonData = $"\"{username}\"";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Logout request succeeded on the server.");
        }
        else
        {
            Debug.LogError($"❌ Logout request failed: {request.responseCode} - {request.downloadHandler.text}");
        }
    }
    // Starts the AR scene (gameplay) after login.
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("ARScene");
    }
    // Opens the "New Student" registration panel and hides other UI.
    public void OpenNewStudentPanel()
    {
        if (newStudentPanel != null)
        {
            buttons.SetActive(false);

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                isSettingsOpen = false;
            }

            if (leaderboardPanel != null && leaderboardPanel.activeSelf)
            {
                leaderboardPanel.SetActive(false);
                isLeaderboardOpen = false;
            }

            newStudentPanel.SetActive(true);
        }
    }
    // Opens the "New Teacher" registration panel and hides other UI.
    public void OpenNewTeacherPanel()
    {
        if (newTeacherPanel != null)
        {
            buttons.SetActive(false);

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                isSettingsOpen = false;
            }

            if (leaderboardPanel != null && leaderboardPanel.activeSelf)
            {
                leaderboardPanel.SetActive(false);
                isLeaderboardOpen = false;
            }

            newTeacherPanel.SetActive(true);
        }
    }
    // Closes a given panel and re-enables the main button menu.
    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
        {
            buttons.SetActive(true);
            panel.SetActive(false);
        }
    }
// Clears all student registration input fields.
    public void ClearStudentFields()
    {
        studentFirstNameInput.text = "";
        studentLastNameInput.text = "";
        studentUsernameInput.text = "";
        studentPasswordInput.text = "";
        studentIdInput.text = "";
    }
// Clears all teacher registration input fields.
    public void ClearTeacherFields()
    {
        teacherFirstNameInput.text = "";
        teacherLastNameInput.text = "";
        teacherUsernameInput.text = "";
        teacherPasswordInput.text = "";
        teacherIdInput.text = "";
    }
    // Hides a UI label after a set number of seconds (used for error/warning messages).
    private IEnumerator HideLabelAfterSeconds(GameObject label, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (label != null)
            label.SetActive(false);
    }
// Quits the application (or stops play mode in the editor).
    public void QuitGame()
    {
        Debug.Log("Quit Game clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

// Fetches and displays shape-specific stats for a student by ID.
    public void OnCheckIdClicked()
    {
        string studentId = checkIdInput.text.Trim();
        if (!string.IsNullOrEmpty(studentId))
        {
            StartCoroutine(FetchStudentShapeData(studentId));
        }
        else
        {
            studentShapeDataText.text = "Please enter a valid ID.";
        }
    }
    // Coroutine to fetch student shape stats from the server.
    private IEnumerator FetchStudentShapeData(string studentId)
    {
        string url = $"https://mathstarz-server-1.onrender.com/users?id={studentId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            try
            {
                StudentShapeData parsed = JsonUtility.FromJson<StudentShapeData>(json);
                List<string> lines = new List<string>
            {
                string.Format(LanguageManager.Instance.GetTranslation("circle_label"), parsed.shapes.circle),
                string.Format(LanguageManager.Instance.GetTranslation("square_label"), parsed.shapes.square),
                string.Format(LanguageManager.Instance.GetTranslation("triangle_label"), parsed.shapes.triangle)
            };
                if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                {
                    for (int i = 0; i < lines.Count; i++)
                        lines[i] = ReverseHebrew(lines[i]);
                }
                studentShapeDataText.text = string.Join("\n", lines);
                if (studentShapeDataText.transform.parent != null)
                    studentShapeDataText.transform.parent.gameObject.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing shape data: " + e.Message);
                studentShapeDataText.text = ReverseHebrew("Error parsing student data.");

                if (studentShapeDataText.transform.parent != null)
                    studentShapeDataText.transform.parent.gameObject.SetActive(true);
            }
        }
        else
        {
            string message = LanguageManager.Instance.GetTranslation("student_not_found");
            if (LanguageManager.Instance.CurrentLanguage == "Hebrew")
                message = ReverseHebrew(message);
            studentShapeDataText.text = message;
            if (studentShapeDataText.transform.parent != null)
                studentShapeDataText.transform.parent.gameObject.SetActive(true);
        }
    }

// Toggles visibility of the "Check Student Data" panel.
    public void ToggleStudentDataPanel()
    {
        if (studentDataPanel != null)
        {
            bool isActive = studentDataPanel.activeSelf;
            studentDataPanel.SetActive(!isActive);
            if (!isActive)
            {
                studentShapeDataText.text = "";
                checkIdInput.text = "";
                studentShapeDataText.transform.parent.gameObject.SetActive(false);
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    settingsPanel.SetActive(false);
                    isSettingsOpen = false;
                }
                if (leaderboardPanel != null && leaderboardPanel.activeSelf)
                {
                    leaderboardPanel.SetActive(false);
                    isLeaderboardOpen = false;
                }
            }
        }
    }
// Updates the global volume and volume icon based on slider input.
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;

        // Update icon
        if (volumeButtonImage != null)
            volumeButtonImage.sprite = value == 0f ? speakerOffIcon : speakerOnIcon;

        // Update lastVolumeBeforeMute if not muted
        if (value > 0f)
            lastVolumeBeforeMute = value;

        Debug.Log("🔊 Volume changed to: " + value);
    }

// Toggles mute/unmute state and restores previous volume level if unmuted.
    public void ToggleMute()
    {
        if (volumeSlider == null || volumeButtonImage == null)
            return;

        // Temporarily remove listener to avoid recursion
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

        bool willMute = AudioListener.volume > 0f;

        if (willMute)
        {
            // Save current volume and mute
            lastVolumeBeforeMute = volumeSlider.value;
            volumeSlider.value = 0f;
            AudioListener.volume = 0f;
            volumeButtonImage.sprite = speakerOffIcon;
        }
        else
        {
            // Restore previous volume
            volumeSlider.value = lastVolumeBeforeMute;
            AudioListener.volume = lastVolumeBeforeMute;
            volumeButtonImage.sprite = speakerOnIcon;
        }

        // Re-attach listener
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        Debug.Log("🔈 Toggled mute. Volume now: " + AudioListener.volume);

    }
}