using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

namespace Spaces {
    public class RacingManagerScript : MonoBehaviour {
        // Start is called before the first frame update

        UIManagerPublicScript uIManager;

        CharacterScript player;

        public BoxCollider outerBounds;

        private bool isRacingManager;

        private string username = null;

        public void SetPlayer(CharacterScript character) {
            player = character;
        }

        void OnTriggerEnter(Collider other) {
            CharacterScript player = other.gameObject.GetComponent<CharacterScript>();
            if (player && !player.otherPlayer) {
                // check firebase if race is open
                // if so 
                uIManager.ToggleJoinRaceButton(true);
                // else show 
                uIManager.ToggleRaceUnavailable(true);
            }
        }

        void OnTriggerExit(Collider other) {
            CharacterScript player = other.gameObject.GetComponent<CharacterScript>();
            if (player && !player.otherPlayer) {
                uIManager.ToggleJoinRaceButton(false);
                uIManager.ToggleRaceUnavailable(false);
            }
        }

        public void JoinRace() {
            // add name to firebase
            player.LeaveFlyer();
            player.TakeRacingFlyer();
            outerBounds.enabled = true;
            username = (username == null) ? PlayerPrefs.GetString("username") : username;
            List<string> players = JoinFirebaseLobby();
            uIManager.ToggleRacingLobby(true, null); // TODO: pass the current names in the racing lobby
        }

        public void NewPlayerInLobby(string name) {
            uIManager.AddPlayerToRacingLobby(name);
        }

        // firebase layout
        // players 
        //    username: string
        // owner : string
        // open: bool
        // winner: string


        public List<string> JoinFirebaseLobby() {
            return null;
            // TODO 
            // if racing lobby is empty, add as player (will create lobby), and show "start race" button, and set owner as @name, and set isRacingManageer to true
            // otherwise start listening to the lobby (for new players) which will return all players currently in the game
            // set isRacingManager to false, get the racingmanager name, update in uimanager and return all names in lobby
        }

        public void StartRace() {
            if (isRacingManager) {
                // update firebase to race started
            }
        }
    }
}
