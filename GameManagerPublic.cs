using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

namespace Spaces {
    public class GameManagerPublic : MonoBehaviourPunCallbacks {
        // Start is called before the first frame update
        // public GameObject ChatManager;

        public CharacterScript[] PlayerPrefabs;

        private CharacterScript PlayerPrefab;

        [HideInInspector]
        public CharacterScript LocalPlayer;

        private int index;


        int roomCount = 1;


        public GameObject ModTerrainPrefab;

        public SaveSystem SaveSystem;
        public GameObject EditRoomButton;
        public GameObject GoBackHomeButton;
        public GameObject LoadingScreen;
        string currentUsername;

        public GameObject CurrentRoomUsername;
        private bool initialConnection = true;
        private bool reconnect = false;

        private string myUsername;
        public GameObject ChatManager;

        string publicWorldName;

        // FirebaseFirestore db;


        void Awake() {
            if (PlayerPrefs.GetInt("isInPublicWorld") == 0) {
                PlayerPrefs.SetInt("isInPublicWorld", 1);
            }
        }

       void OnApplicationFocus(bool focus) {
            if (focus && !initialConnection) {
                StartCoroutine(CheckIfDisconnected());
                LogToFirebase(1);
            }
            if (!focus && !initialConnection) {
                LogToFirebase(0);
            }
        }

        void OnApplicationQuit() {
            LogToFirebase(-1);        
        }

        IEnumerator CheckIfDisconnected() {
            yield return new WaitForSeconds(1);
            if (!PhotonNetwork.IsConnectedAndReady) {
                LoadingScreen.SetActive(true);
                ChatManager.GetComponent<PublicWorldChatManager>().ResetToStart();
                PhotonNetwork.ConnectUsingSettings();
                reconnect = true;
            }
        }

        void Start() {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            PlayerPrefs.SetString("currentPublicWorld", "Town");
            LoadingScreen.SetActive(true);
            myUsername = PlayerPrefs.GetString("username");
            string currentSkin = PlayerPrefs.GetString("CurrentSkin");
            publicWorldName = PlayerPrefs.GetString("currentPublicWorld");
            GameObject playerPrefab = Resources.Load<GameObject>("Characters/" + currentSkin);
            PlayerPrefab = playerPrefab.GetComponent<CharacterScript>();;
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.NickName = "Pablo";
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
            PhotonNetwork.GameVersion = "v1";
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster() {
            base.OnConnectedToMaster();
            Debug.Log("connected to master");
            OnClickConnectRoom();
        }
       public void OnClickConnectRoom() {
           Debug.Log("zzzz world name " + publicWorldName);
            PhotonNetwork.JoinRoom(publicWorldName);
        }

        public override void OnJoinedRoom() {
            base.OnJoinedRoom();
            CharacterScript.RefreshInstance(ref LocalPlayer, PlayerPrefab);
            initialConnection = false;
            if (reconnect) {
                reconnect = false;
                LoadingScreen.SetActive(false);
                return;
            }
            LoadingScreen.SetActive(false);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            Debug.Log("failed to join room");
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log(message);
            Debug.Log("going to create room now");
            PhotonNetwork.CreateRoom(publicWorldName);
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
            Debug.Log("failed to create room");
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("failed to create room");
        }

        public void DisconnectPlayer() {
            StartCoroutine(DisconnectAndLoad());
        }

        IEnumerator DisconnectAndLoad() {
            PhotonNetwork.Disconnect();
            PhotonVoiceNetwork.Instance.Disconnect();
            while (PhotonNetwork.IsConnected) {
                Debug.Log("In room");
                yield return null;
            }
            PlayerPrefs.SetInt("isInPublicWorld", 0);
            SceneManager.LoadScene("MainGame");
        }

        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player player) {
            base.OnPlayerEnteredRoom(player);
            Debug.Log("player has tag object "  + player.TagObject);
            // ChatManager manager = ChatManager.GetComponent<ChatManager>();
            // maybe don't need to refresh instance?
        }



        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
            Debug.Log("player leaving");
            base.OnPlayerLeftRoom(otherPlayer);
            CharacterScript otherScript = (otherPlayer.TagObject as GameObject).GetComponent<CharacterScript>();
            // otherScript.DestroyCamera();
            // ChatManager manager = ChatManager.GetComponent<ChatManager>();
            // if (manager.id > otherScript.id) {
            //     manager.ChangeID();
            // }
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);   
        }

        public void LogToFirebase(int state) {
            // -1 is left; 0 is sleeping; 1 is active 
            string lastSeen = (state == 1) ? "1" : ((state == 0) ? "0" : DateTime.Now.ToString());
            Dictionary<string, object> location = new Dictionary<string, object>
            {
                    { "LastSeen", lastSeen},
                    { "Place", "~spaces~"}
            };
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("users").Child(myUsername).UpdateChildrenAsync(location);
        }

    }
}
