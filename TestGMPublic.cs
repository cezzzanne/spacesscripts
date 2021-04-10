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
    public class TestGMPublic : MonoBehaviourPunCallbacks {
        // Start is called before the first frame update
        // public GameObject ChatManager;

        private string currentSkin, username;

        GameObject player;

        public List<string> otherPlayers = new List<string>();

        private bool readyToAddOthers = false;

        void Start() {
            currentSkin = PlayerPrefs.GetString("CurrentSkin");
            username = PlayerPrefs.GetString("username");
            GameObject playerPrefab = Resources.Load<GameObject>("Characters/00testCharacter");
            playerPrefab.transform.position = new Vector3(2, 1, 4);
            player = Instantiate(playerPrefab) as GameObject;  
            player.AddComponent<TestCharacterScript>();
            StartCoroutine(AddOtherPlayers());
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("inWorld").GetValueAsync().ContinueWith(task => {
            DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null) {
                    Dictionary<string, object> players = snapshot.Value as Dictionary<string, object>;
                    foreach(KeyValuePair<string, object> player in players) {
                        if (player.Key != username) {
                            otherPlayers.Add(player.Key);
                        }
                    }
                    readyToAddOthers = true;
                }
            });
        }

        public IEnumerator AddOtherPlayers() {
            while (!readyToAddOthers) {
                yield return new WaitForSeconds(0.2f);
            }
            foreach(string player in otherPlayers) {
                CreateOtherPlayer(player);
            }
        }

        public void CreateOtherPlayer(string thisusername) {
            GameObject playerPrefab = Resources.Load<GameObject>("Characters/00testCharacter");
            playerPrefab.transform.position = new Vector3(2, 1, 4);
            playerPrefab = Instantiate(playerPrefab);
            playerPrefab.AddComponent<TestRemoteCharacterScript>();
            playerPrefab.GetComponent<TestRemoteCharacterScript>().StartCharacter(thisusername);
            // DestroyImmediate(playerPrefab.GetComponent<TestCharacterScript>(), true);
            // playerPrefab.GetComponent<TestRemoteCharacter>().StartCharacter(thisusername);
        }


    }
}
