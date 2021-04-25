﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using OneSignalPush.MiniJSON;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

namespace Spaces {
    public class GameManagerScript : MonoBehaviourPunCallbacks {
        // Start is called before the first frame update
        // public GameObject ChatManager;

        public CharacterScript[] PlayerPrefabs;

        private CharacterScript PlayerPrefab;

        [HideInInspector]
        public CharacterScript LocalPlayer;

        private int index;

        GameObject Voice;

        int roomCount = 1;

        string roomIDToJoin;

        public GameObject ModTerrainPrefab;

        public SaveSystem SaveSystem;
        public GameObject LoadingScreen;
        string currentUsername;

        public GameObject CurrentRoomUsername;
        private bool initialConnection = true;
        private bool reconnect = false;

        private GameObject currentTerrain;
        private GameObject currentPrebuiltTerrain;
        private string currentWorldType;
        private string previousWorldType;
        public GameObject ItemController;

        private string myUsername = null;

        private string myRoomID;
        private string currentSkin;

        public InnerNotifManagerScript innerNotifManagerScript;

        public GameObject MalePrefab, FemalePrefab;
        // world type is based on the type of world the user posseses (3 kinds) // roomID is to join the same photon room based on player id


        void Awake() {
            int inPublic = PlayerPrefs.GetInt("isInPublicWorld", -1);
            if (inPublic == -1) {
                PlayerPrefs.SetInt("isInPublicWorld", 0);
                inPublic = 0;
            } 
            if (inPublic == 1) {
                PlayerPrefs.SetInt("isInPublicWorld", 0);
            }
            OneSignal.StartInit("73edd87b-7555-4075-b728-5a27c4fb1a9f")
                .HandleNotificationOpened(HandleNotificationOpened)
                .Settings(new Dictionary<string, bool>() {
                { OneSignal.kOSSettingsAutoPrompt, false },
                { OneSignal.kOSSettingsInAppLaunchURL, false } })
                .EndInit();
            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
            OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
        }

        // testing function for previous error / perhaps get the FriendManager and on notification of accept request get id and name and add button to friend panel
        private static void HandleNotificationOpened(OSNotificationOpenedResult result) {
        }

        void OnApplicationFocus(bool focus) {
            if (focus && !initialConnection) {
                StartCoroutine(CheckIfDisconnected());
                if (roomIDToJoin == myRoomID) {
                    LogToFirebase(myUsername.ToLower() + "'s World", 1);
                } else {
                    LogToFirebase(currentUsername.ToLower() + "'s World", 1);
                }
            }
            if (!focus && !initialConnection) {
                if (roomIDToJoin == myRoomID) {
                    LogToFirebase(myUsername.ToLower() + "'s World", 0);
                } else {
                    LogToFirebase(currentUsername.ToLower() + "'s World", 0);
                }
            }
        }

        void OnApplicationQuit() {
            if (myUsername == null) {
                return;
            }
            if (roomIDToJoin == myRoomID) {
                LogToFirebase(myUsername.ToLower() + "'s World", -1);
            } else {
                LogToFirebase(currentUsername.ToLower() + "'s World", -1);
            }        
        }

        IEnumerator CheckIfDisconnected() {
            yield return new WaitForSeconds(1);
            if (!PhotonNetwork.IsConnectedAndReady) {
                LoadingScreen.SetActive(true);               
                PhotonNetwork.ConnectUsingSettings();
                reconnect = true;
            }
        }

        void Start() {
            PhotonNetwork.Disconnect();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            LoadingScreen.SetActive(true);
            currentWorldType = PlayerPrefs.GetString("currentWorldType");
            roomIDToJoin = PlayerPrefs.GetString("currentRoomID");
            myUsername = PlayerPrefs.GetString("username");
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
            PhotonNetwork.NickName = "Pablo";
            PhotonNetwork.GameVersion = "v1";
            if (PhotonNetwork.IsConnected) {
                OnClickConnectRoom();
            } else {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster() {
            base.OnConnectedToMaster();
            Debug.Log("connected to master");
            OnClickConnectRoom();
        }

       public void OnClickConnectRoom() {
            // currentSkin = PlayerPrefs.GetString("CurrentSkin");
            // GameObject playerPrefab = Resources.Load<GameObject>("Characters/" + currentSkin);
            // PlayerPrefab = playerPrefab.GetComponent<CharacterScript>();
            GameObject selectedPrefab = (PlayerPrefs.GetInt("isMale") == 1) ? MalePrefab : FemalePrefab;
            PlayerPrefab = selectedPrefab.GetComponent<CharacterScript>();
            PhotonNetwork.JoinRoom(roomIDToJoin);
        }

        public override void OnJoinedRoom() {
            base.OnJoinedRoom();
            CharacterScript.RefreshInstance(ref LocalPlayer, PlayerPrefab);
            initialConnection = false;
            if (reconnect) {
                Debug.Log("111: reconnecting active");
                LoadingScreen.SetActive(false);
                reconnect = false;
                return;
            }
            SetUpNewTerrain();
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log(message);
            PhotonNetwork.CreateRoom(roomIDToJoin);
            Voice = GameObject.FindGameObjectWithTag("Voice");
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
            base.OnCreateRoomFailed(returnCode, message);
        }

        public void DisconnectPlayer() {
            StartCoroutine(DisconnectAndLoad());
        }

        IEnumerator DisconnectAndLoad() {
            PhotonVoiceNetwork.Instance.Disconnect();
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected) {
                Debug.Log("In room");
                yield return null;
            }
            SceneManager.LoadScene("WorldEdit");
        }

        public void GoToPublicWorld(string worldName) {
            if (innerNotifManagerScript.GetCurrentCoins() - 5 < 0) {
                // not enough coins to travel;
                return;
            }
            LogToFirebase(worldName, 1, true);
            PlayerPrefs.SetString("currentPublicWorld", worldName);
            StartCoroutine(DisconnectToPublicWorld(worldName));
        }

        IEnumerator DisconnectToPublicWorld(string worldName) {
            PhotonVoiceNetwork.Instance.Disconnect();
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected) {
                Debug.Log("In room");
                yield return null;
            }
            PlayerPrefs.SetInt("isInPublicWorld", 1);
            SceneManager.LoadScene(worldName);
        }

        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player player) {
            base.OnPlayerEnteredRoom(player);
            Debug.Log("player has tag object "  + player.TagObject);
            // ChatManager manager = ChatManager.GetComponent<ChatManager>();
            // maybe don't need to refresh instance?
        }

        public void GoToRoom(string newRoomID, string username, string worldType) {
            previousWorldType = currentWorldType;
            currentWorldType = worldType;
            if (roomIDToJoin == newRoomID) {
                Debug.Log("zz same fucking room");
                return;
            }
            LoadingScreen.SetActive(true);
            PlayerPrefs.SetString("currentRoomID", newRoomID);
            PlayerPrefs.SetString("currentRoomUsername", username);
            PlayerPrefs.SetString("currentWorldType", worldType);
            PlayerPrefab.DestroyCamera();
            roomIDToJoin = newRoomID;
            currentUsername = username;
            // PhotonVoiceNetwork.Instance.Disconnect();
            PhotonNetwork.LeaveRoom();
        }

        public void SetUpNewTerrain() {
            myRoomID = PlayerPrefs.GetString("myRoomID");
            // check if same terrain type as previous (first check its not null // actually maybe itll just default to false if null)
            if (currentWorldType != previousWorldType) {
                if (currentTerrain != null && currentPrebuiltTerrain != null) {
                    Destroy(currentPrebuiltTerrain);
                    Destroy(currentTerrain);
                }
                GameObject terrain = Resources.Load<GameObject>("Worlds/" + currentWorldType + "-Terrain");
                GameObject prebuiltTerrain = Resources.Load<GameObject>("Worlds/" + currentWorldType + "-PrebuiltTerrain");
                currentTerrain = Instantiate(terrain);
                // ItemController.GetComponent<ItemPlacementController>().SetTerrain(currentTerrain);
                currentPrebuiltTerrain = Instantiate(prebuiltTerrain);
            }
            if (myRoomID == roomIDToJoin) {
                CurrentRoomUsername.SetActive(false);
                LogToFirebase(myUsername.ToLower() + "'s World", 1);
            } else {
                CurrentRoomUsername.SetActive(true);
                if (currentUsername == null) {
                    currentUsername = PlayerPrefs.GetString("currentRoomUsername");
                }
                CurrentRoomUsername.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "@" + currentUsername.ToLower();
                LogToFirebase(currentUsername.ToLower() + "'s World", 1);
                SendNotification(currentUsername);
            }
            GameObject oldTerrain = GameObject.FindGameObjectWithTag("ModifiedTerrain");
            Destroy(oldTerrain);
            GameObject newTerrain = Instantiate(ModTerrainPrefab) as GameObject;
            newTerrain.transform.position = new Vector3(0, 0, 0);
            ItemController.GetComponent<ItemPlacementController>().SetTerrain(newTerrain);
            // todo : this is wrong; I should be checking the transform of something else but definetly no this transform
            if (!(transform.childCount > 0)) {
                SaveSystem.LoadSpace(roomIDToJoin, newTerrain, null);
            }
            LoadingScreen.SetActive(false);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
            base.OnPlayerLeftRoom(otherPlayer);
            CharacterScript otherScript = (otherPlayer.TagObject as GameObject).GetComponent<CharacterScript>();
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }

        public void SendNotification(string friendUsername) {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("usernameList").Child(friendUsername).GetValueAsync().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                Dictionary<string, object> notification = new Dictionary<string, object>();
                notification["headings"] = new Dictionary<string, string>() { {"en", "Someone is in your world 🌎" } };
                notification["contents"] = new Dictionary<string, string>() { {"en", "@" + myUsername.ToLower() + " is visiting you! Come say hi 👋" } };
                notification["include_player_ids"] = new List<string>() { snapshot.Value.ToString().Trim() };
                OneSignal.PostNotification(notification);
            });
        }


        public void LogToFirebase(string seenAt, int state, bool goingToTown=false) {
            // -1 is left; 0 is sleeping; 1 is active
            string lastSeen = (state == 1) ? "1" : ((state == 0) ? "0" : DateTime.Now.ToString());
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                    { "LastSeen", lastSeen},
                    { "Place", seenAt}
            };
            if (goingToTown) {
                payload.Add("coins", innerNotifManagerScript.GetCurrentCoins() - 5);
            }
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("users").Child(myUsername).UpdateChildrenAsync(payload);
        } 

    }
}
