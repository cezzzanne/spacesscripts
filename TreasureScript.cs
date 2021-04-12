using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;



namespace Spaces {
    public class TreasureScript : MonoBehaviour {
        
        TreasureHuntScript huntScript;

        void Start() {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("treasure").ValueChanged += HandleValueChanged;
        }

        void HandleValueChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            DataSnapshot snapshot = args.Snapshot;
            if (!snapshot.Exists) {
                Destroy(gameObject);
            }
        }

        public void SetHuntManager(TreasureHuntScript ths) {
            huntScript = ths;
        }

        void OnTriggerEnter(Collider other) {
            CharacterScript player = other.GetComponent<CharacterScript>();
            if (player != null) {
                if (!player.otherPlayer) {
                    huntScript.ShowQuestion();
                }
            }
        }

        void OnTriggerExit(Collider other) {
            CharacterScript player = other.GetComponent<CharacterScript>();
            if (player != null) {
                if (!player.otherPlayer) {
                    huntScript.HideQuestion();
                }
            }
        }
    }
}
