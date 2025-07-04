using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// Data structure for sending login credentials to the server
[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;

    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
// Manages user login flow including auto-login, credential validation,
// server-side checks, and transitioning to the main game.
public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text errorLabel;
    [SerializeField] private MainMenu mainMenu;


    public Action onLogin;
    // Called on startup — sets up input listeners and checks for cached login
    private void Start()
    {
        Debug.Log("🧪 LoginManager Start()");

        if (mainMenu == null)
            mainMenu = FindAnyObjectByType<MainMenu>();

        onLogin -= mainMenu.displayData;
        onLogin += mainMenu.displayData;
        // Adjust text alignment dynamically based on input
        username.onValueChanged.AddListener((text) => mainMenu.AdjustInputAlignment(username, text));
        password.onValueChanged.AddListener((text) => mainMenu.AdjustInputAlignment(password, text));
        // Attempt auto-login using saved ID
        string cachedId = PlayerPrefs.GetString("userId", null);
        Debug.Log("🧪 cachedId = " + cachedId);

        if (!string.IsNullOrEmpty(cachedId))
        {
            StartCoroutine(CheckIfUserIsLoggedIn(cachedId));
        }
        else
        {
            FallbackToLoginScreen();
        }
    }
    // Clears login fields
    public void ClearLoginInputs()
    {
        username.text = "";
        password.text = "";
    }
    // Starts the full login flow with field validation and server-side checks

    public void LoginUser()
    {
        errorLabel.text = "";

        string user = username.text.Trim();
        string pass = password.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            SetLocalizedError("login_missing_fields");
            return;
        }

        if (!IsEnglishOnly(user) || !IsEnglishOnly(pass))
        {
            SetLocalizedError("login_english_only");
            return;
        }
        // Chain of validations: username exists → password correct → not already logged in → login
        StartCoroutine(CheckIfUsernameExists(user, (exists) =>
        {
            if (!exists)
            {
                SetLocalizedError("login_user_not_exist");
                return;
            }

            StartCoroutine(CheckIfPasswordCorrect(user, pass, (correctPassword) =>
            {
                if (!correctPassword)
                {
                    SetLocalizedError("login_wrong_password");
                    return;
                }

                StartCoroutine(CheckIfUserAlreadyLoggedIn(user, (loggedIn) =>
                {
                    if (loggedIn)
                    {
                        SetLocalizedError("login_already_logged");
                        return;
                    }

                    StartCoroutine(SendLoginRequest(user, pass));
                }));
            }));
        }));
    }

    // Sets localized error text based on translation key
    private void SetLocalizedError(string translationKey)
    {
        string msg = LanguageManager.Instance.GetTranslation(translationKey);

        bool isRTL = LanguageManager.Instance.CurrentLanguage == "Hebrew";

        errorLabel.text = msg;
        errorLabel.isRightToLeftText = isRTL;
        errorLabel.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
    }

    // Rejects Hebrew letters from input to enforce English-only usernames and passwords
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

    // Sends login credentials to the server and sets the user data if valid
    private IEnumerator SendLoginRequest(string username, string password)
    {
        string url = "https://mathstarz-server-1.onrender.com/users/login";

        LoginRequest loginPayload = new LoginRequest(username, password);
        string jsonData = JsonUtility.ToJson(loginPayload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            string fullJson = request.downloadHandler.text;

            int userKeyIndex = fullJson.IndexOf("\"user\":");
            if (userKeyIndex == -1)
            {
                Debug.LogError("User field not found in JSON.");
                yield break;
            }

            int braceStart = fullJson.IndexOf('{', userKeyIndex);
            int braceCount = 0;
            int endIndex = braceStart;

            for (int i = braceStart; i < fullJson.Length; i++)
            {
                if (fullJson[i] == '{') braceCount++;
                else if (fullJson[i] == '}') braceCount--;

                if (braceCount == 0)
                {
                    endIndex = i;
                    break;
                }
            }

            string userJson = fullJson.Substring(braceStart, endIndex - braceStart + 1);

            try
            {
                UserData data = JsonUtility.FromJson<UserData>(userJson);
                GameData.Instance.SetUserData(data);

                PlayerPrefs.SetString("userId", data.id);
                PlayerPrefs.Save();

                onLogin?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Parsing failed: " + e.Message);
                Debug.LogError("JSON was: " + userJson);
            }
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            errorLabel.text = "Login failed. Please try again.";
        }
    }
    // Manually loads a Unity scene by index
    public void LoadScene(int num)
    {
        SceneManager.LoadScene(num);
    }

    private IEnumerator CheckIfUserIsLoggedIn(string userId)
    {
        string url = $"https://mathstarz-server-1.onrender.com/users/check-loggedin?id={userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        bool fallbackTriggered = false;

        if (request.result == UnityWebRequest.Result.Success)
        {
            string fullJson = request.downloadHandler.text;

            try
            {
                int userStart = fullJson.IndexOf("\"user\":");
                if (userStart == -1)
                {
                    fallbackTriggered = true;
                }
                else
                {
                    int braceStart = fullJson.IndexOf('{', userStart);
                    int braceCount = 0;
                    int i = braceStart;

                    for (; i < fullJson.Length; i++)
                    {
                        if (fullJson[i] == '{') braceCount++;
                        else if (fullJson[i] == '}') braceCount--;

                        if (braceCount == 0) break;
                    }

                    if (braceCount != 0)
                    {
                        fallbackTriggered = true;
                    }
                    else
                    {
                        string userJson = fullJson.Substring(braceStart, i - braceStart + 1);
                        UserData data = JsonUtility.FromJson<UserData>(userJson);

                        if (data != null)
                        {
                            GameData.Instance.SetUserData(data);
                            onLogin?.Invoke();
                        }
                        else
                        {
                            fallbackTriggered = true;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Auto-login parse failed: " + e.Message);
                fallbackTriggered = true;
            }
        }
        else
        {
            fallbackTriggered = true;
        }

        if (fallbackTriggered)
        {
            yield return new WaitForSeconds(0.1f);
            FallbackToLoginScreen();
        }
    }
    // Returns user to the login UI screen
    private void FallbackToLoginScreen()
    {
        if (mainMenu == null)
            mainMenu = FindAnyObjectByType<MainMenu>();

        if (mainMenu != null)
        {
            Debug.Log("📺 Calling ManageScreens(0) to show login UI");
            mainMenu.ManageScreens(0);
        }
        else
        {
            Debug.LogError("❌ MainMenu is still null — login screen cannot be shown.");
        }
    }
    // Verifies if the username exists on the server
    private IEnumerator CheckIfUsernameExists(string username, Action<bool> callback)
    {
        string url = $"https://mathstarz-server-1.onrender.com/users/exists?username={UnityWebRequest.EscapeURL(username)}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            bool exists = request.downloadHandler.text.Contains("true");
            callback?.Invoke(exists);
        }
        else
        {
            Debug.LogError("❌ Username check failed: " + request.error);
            callback?.Invoke(false);
        }
    }
    // Verifies if the given password matches the server-stored hash
    private IEnumerator CheckIfPasswordCorrect(string username, string password, Action<bool> callback)
    {
        string url = "https://mathstarz-server-1.onrender.com/users/verify-password";

        LoginRequest payload = new LoginRequest(username, password);
        string jsonData = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            bool correct = request.downloadHandler.text.Contains("true");
            callback?.Invoke(correct);
        }
        else
        {
            Debug.LogError("❌ Password verification failed: " + request.error);
            callback?.Invoke(false);
        }
    }
    // Checks whether the user is already marked as logged in on the backend
    private IEnumerator CheckIfUserAlreadyLoggedIn(string username, Action<bool> callback)
    {
        string url = $"https://mathstarz-server-1.onrender.com/users/is-logged-in?username={UnityWebRequest.EscapeURL(username)}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            bool loggedIn = request.downloadHandler.text.Contains("true");
            callback?.Invoke(loggedIn);
        }
        else
        {
            Debug.LogError("❌ Logged-in check failed: " + request.error);
            callback?.Invoke(false);
        }
    }
}
