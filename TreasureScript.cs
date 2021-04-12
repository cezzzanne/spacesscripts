using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class TreasureScript : MonoBehaviour {
        
        TreasureHuntScript huntScript;

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
