using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

namespace Spaces {
    public class FlyerGameScript : MonoBehaviour {

        private CharacterScript player;

        float currentAltitude = 0;

        int activeFlyers = -1;

        public void SetPlayer(CharacterScript cs) {
            player = cs;
        }

        public void TakeFlyer() {
            player.TakeFlyer();
        }

        public void LeaveFlyer() {
            player.LeaveFlyer();
        }

        void Start() {
            StartCoroutine(CheckFlyers());
            StartCoroutine(ActivateFlyers());
        }


        IEnumerator CheckFlyers() {
            yield return new WaitForSeconds(5);
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("activeFlyers").GetValueAsync().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                string val = snapshot.Value.ToString();
                activeFlyers = val == "yes" ? 1 : 0;
                
            });
        }
        IEnumerator ActivateFlyers() {
            while (activeFlyers == -1) {
                yield return null;
            }
            if (activeFlyers == 1) {
                for(int i = 0; i < transform.childCount; i++) {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }
}
