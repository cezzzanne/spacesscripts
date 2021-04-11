using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class CompassScript : MonoBehaviour {

        public Vector3 goingTo;

        public bool active = false;

        Transform compass;

        Transform character;


        void Start()  {
            compass = transform.GetChild(1).GetChild(0);
        }

        public void SetPlayer(Transform player) {
            character = player;
        }

        public void StartCompass(Vector3 to) {
            goingTo = to;
            active = true;
        }

        public void StopCompass() {
            active = false;
        }

        // Update is called once per frame
        void Update() {
            if (active) {
                Vector3 targetDir = goingTo - character.position;
                Vector3 forward = character.forward;
                float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
                compass.eulerAngles = new Vector3(0 ,0, angle);
            }
        }
    }
}
