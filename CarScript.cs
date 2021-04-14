using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class CarScript : MonoBehaviour {
        
        private PathManagerScript pathManager;

        public bool stopped = false;

        public void SetPathManager(PathManagerScript pms) {
            pathManager = pms;
        }

        void OnTriggerEnter(Collider other) {
            if (other.GetComponent<CarScript>() != null && !other.GetComponent<CarScript>().stopped) {
                if (pathManager != null) {
                    pathManager.ToggleRoadIsClear(false);
                    stopped = true;
                }
            }
        }

        void OnTriggerExit(Collider other) {
            if (other.GetComponent<CarScript>() != null) {
                if (pathManager != null) {
                    StartCoroutine(StartAgain());
                }
            }
        }


        IEnumerator StartAgain() {
            yield return new WaitForSeconds(2);
            stopped = false;
            pathManager.ToggleRoadIsClear(true);
        }
    }
}
