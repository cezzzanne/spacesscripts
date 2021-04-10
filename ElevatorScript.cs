using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class ElevatorScript : MonoBehaviour {

        // public GameObject UIManagerScript;

        // private UIManagerPublicScript UIScript;

        // private bool elevatorIsMoving = false;

        // public GameObject HideElevatorFloorsButton;

        // private CharacterScript character;

        // public GameObject OffloadingPlatform;


        // void Start() {
        //     UIScript = UIManagerScript.GetComponent<UIManagerPublicScript>();
        // }

        // private void OnTriggerEnter(Collider other) {
        //     CharacterScript player = other.transform.GetComponent<CharacterScript>();
        //     if (player != null) {
        //         if (!player.otherPlayer && !elevatorIsMoving) {
        //             UIScript.ShowElevatorFloors();
        //             character = player;
        //         }
        //     }
        // }

        // private void OnTriggerExit(Collider other) {
        //     CharacterScript player = other.transform.GetComponent<CharacterScript>();
        //     if (player != null) {
        //         if (!player.otherPlayer && !elevatorIsMoving) {
        //             UIScript.HideElevatorFloors();
        //         }
        //     }
        // }

        // public void GoToFloor(int floorNumber) {
        //     float elevatorY = (floorNumber * 6.15f) + 2;
        //     elevatorIsMoving = true;
        //     UIScript.HideElevatorFloors();
        //     UIScript.GoingToFloor();
        //     character.mainCam.GetComponent<PlayerFollow>().StartElevator();
        //     OffloadingPlatform.SetActive(false);
        //     StartCoroutine(MoveElevatorToFloor(elevatorY));
        // }

        // IEnumerator MoveElevatorToFloor(float y) {
        //     Vector3 des = new Vector3(transform.position.x, y, transform.position.z);
        //     while ((transform.position - des).magnitude > 0.2) {
        //         yield return new WaitForSeconds(0.003f);
        //         transform.position = Vector3.Lerp(transform.position, des, Time.deltaTime / 1f);
        //     }
        //     elevatorIsMoving = false;
        //     character.mainCam.GetComponent<PlayerFollow>().StopElevator();
        //     UIScript.ArrivedAtFloor();
        //     OffloadingPlatform.SetActive(true);
        // }
    }
}
