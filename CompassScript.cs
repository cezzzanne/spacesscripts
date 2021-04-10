using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class CompassScript : MonoBehaviour {

        public Vector3 goingTo;

        public bool active = false;

        Transform compass;

        public Transform character;

        public Transform testTarget;

        void Start()  {
            compass = transform.GetChild(1).GetChild(0);
        }

        public void StartCompass(Vector3 to, Transform player) {
            goingTo = to;
            active = true;
            character = player;
        }

        public void StopCompass() {
            active = false;
        }

        // Update is called once per frame
        void Update() {
            if (active) {
                Vector3 targetDir = testTarget.position - character.position;
                Vector3 forward = character.forward;
                float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
                print("qqq angle " + angle);
                compass.eulerAngles = new Vector3(0 ,0, angle);
            }
        }
    }
}
