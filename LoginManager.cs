using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.Events;


public class LoginManager : MonoBehaviour {

    private TouchScreenKeyboard keyboard;

    private GameObject CurrentPanel;

    private bool keyboardActive = false;

    private Vector3 previousInputHeight;

    public GameObject usernameInput, passwordInput, loginButton, errorPanel;

    private string username, password;

    private int currentInput = 0;

    private int usernameConfirmed = -1;

    public GameObject loadingIndicator, nextButton;

    void Update() {
        if (keyboard != null) {
            // comment this to test on editor 
            if (keyboard.active && TouchScreenKeyboard.visible) {
                CurrentPanel.transform.position = new Vector3(CurrentPanel.transform.position.x, TouchScreenKeyboard.area.height + 15, CurrentPanel.transform.position.z);
                keyboardActive = true;
            } else {
                if (keyboardActive) {
                    keyboardActive = false;
                    CurrentPanel.transform.position = previousInputHeight;
                }
            }
        }
    }


    public void ToggleKeyboard() {
        previousInputHeight = CurrentPanel.transform.position;
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
    }

    public void StartLogin() {
        CurrentPanel = usernameInput.transform.GetChild(1).gameObject;
        nextButton.SetActive(true);
        loginButton.SetActive(false);
        usernameInput.SetActive(true);
    }

    public void UpdateInput(string input) {
        if (currentInput == 0) {
            username = input;
        } else {
            password = input;
        }
    }

    public void NextInput() {
        if (currentInput == 0) {
            loadingIndicator.SetActive(true);
            nextButton.SetActive(true);
            errorPanel.SetActive(false);
            CheckUsername();
            StartCoroutine(VerifyUsername());
        } else {
            loadingIndicator.SetActive(true);
            nextButton.SetActive(false);
            errorPanel.SetActive(false);
            StartCoroutine(MakeRequest("world-login", username, SetUpAccount));
        }
    }


    public void CheckUsername() {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("usernameList").GetValueAsync().ContinueWith(task => {
            DataSnapshot snapshot = task.Result;
            Dictionary<string, object> usernames = snapshot.Value as Dictionary<string, object>;
            foreach(KeyValuePair<string, object> user in usernames) {
                if (username.ToLower() == user.Key) {
                    usernameConfirmed = 1;
                    return;
                }
            }
            usernameConfirmed = 0;
        });
    }

    public IEnumerator VerifyUsername() {
        // stop loading indicator
        while (usernameConfirmed == -1) {
            yield return null;
        }
        if (usernameConfirmed == 0) {
            //error
            errorPanel.SetActive(true);
            usernameConfirmed = -1;
        } else {
            // success
            usernameInput.SetActive(false);
            errorPanel.SetActive(false);
            passwordInput.SetActive(true);
            currentInput = 1;
            CurrentPanel = passwordInput.transform.GetChild(1).gameObject;
        }
        nextButton.SetActive(true);
        loadingIndicator.SetActive(false);
    }

    public void SetUpAccount() {
        SetFirebaseProfile();
        SceneManager.LoadScene("TestMoon");
    }


    public void SetFirebaseProfile() {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        OSPermissionSubscriptionState status = OneSignal.GetPermissionSubscriptionState();
        string oneSignalID = (status.subscriptionStatus.userId != null) ? status.subscriptionStatus.userId : "1";
        Dictionary<string, object> usernameList = new Dictionary<string, object> {
            {username.ToLower(), oneSignalID}
        };
        reference.Child("usernameList").UpdateChildrenAsync(usernameList);
    }




    public delegate void GoToNextScene();

    static IEnumerator MakeRequest(string url, string username, GoToNextScene callback) {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/" + url, form);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            string response = www.downloadHandler.text;
            yield return response;
            Debug.Log(response);
            JsonResponse data = JsonUtility.FromJson<JsonResponse>(response);
            if (data.success == "false") {
                Debug.Log(data.message);
            } else {
                PlayerPrefs.SetString("myRoomID", data.userID);
                PlayerPrefs.SetString("currentWorldType", "MainGame");
                PlayerPrefs.SetString("currentRoomID", data.userID);
                PlayerPrefs.SetString("username", username.ToLower());
                PlayerPrefs.SetString("accessories", "");
                PlayerPrefs.SetString("CurrentSkin", "casualMaleA");
                PlayerPrefs.SetString("myWorldType", "MainGame");
                callback();
            }
        }
      }
}
