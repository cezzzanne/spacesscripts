using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class DoorOpen : MonoBehaviour {
        // Start is called before the first frame update

        Vector3 prevScale;

        bool doorOpen = false;

        bool shouldDoorOpen = true;

        public void DisableDoorOpen() {
            shouldDoorOpen = false;
        }


        void OnTriggerEnter(Collider other) {
            if (!shouldDoorOpen) {
                return;
            }
            if (other.transform.GetComponent<CharacterScript>() != null && !doorOpen) {
                doorOpen = true;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                transform.localScale = new Vector3(1, 1, 4);
                StartCoroutine(CloseDoor());
            }
        }

        IEnumerator CloseDoor() {
            yield return new WaitForSeconds(2);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
            doorOpen = false;
        }
    }
}
