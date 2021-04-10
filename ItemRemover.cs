using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
public class ItemRemover : MonoBehaviour {
    // Start is called before the first frame update

    PlayerFollow MainCam;


    GameObject Character;

    public GameObject TrashCan;

    public float speed = 0.00001f;

    public GameObject UIManGameObject;
    
    UIManagerScript UIManager;

    bool isEditing = false;

    bool up;
    bool right;
    bool forward;

    bool reversed;

    void Start() {
        TrashCan = Instantiate(TrashCan);
        TrashCan.SetActive(false);
        UIManager = UIManGameObject.GetComponent<UIManagerScript>();
    }

    public void SetMainCam(PlayerFollow cam) {
        MainCam = cam;
    }

    void FixedUpdate() {
        if (isEditing) {
            MoveSideWays(right);
            MoveForwardOrBack(forward);
            MoveUpOrDown(up);
        }
    }

    public void StartDeleting() {
        Character = MainCam.GetCharacter();
        Vector3 pos = Character.transform.position;
        TrashCan.transform.position = pos;
        TrashCan.SetActive(true);
        Character.SetActive(false);
        MainCam.NowFollowing(TrashCan.transform, true);
        UIManager.StartDeleting();
        isEditing = true;
    }

    public void MoveSideWays(bool moving) {
        if (moving) {
            Vector3 direction = !reversed ? TrashCan.transform.right : -TrashCan.transform.right;
            TrashCan.transform.position +=  (direction * (speed * 0.1f));
        }
    }

    public void MoveUpOrDown(bool moving) {
        if (moving) {
            Vector3 direction = !reversed ? TrashCan.transform.up : -TrashCan.transform.up;
            TrashCan.transform.position +=  (direction * (speed * 0.1f));
        }
    }

    public void MoveForwardOrBack(bool moving) {
        if (moving) {
            Vector3 direction = reversed ? TrashCan.transform.forward : -TrashCan.transform.forward;
            TrashCan.transform.position +=  (direction * (speed * 0.1f));
        }
    }

    public void CancelDeleting() {
        UIManager.CancelDeleting();
        // add cancel editing trashan to remove the item as well as restore the color if it is actually touching an item
        TrashCan.GetComponent<DeleteTrashCanScript>().CancelEditing();
        TrashCan.SetActive(false);
        Character.SetActive(true);
        MainCam.NowFollowing(Character.transform, false);
        isEditing = false;
    }

    public void SetForwardInput(bool pressed, bool isForward, bool rev, bool isUpOrDown) {
        if (!pressed) {
            up = false;
            right = false;
            forward = false;
            reversed = false;
            return;
        }
        if (isUpOrDown) {
            up = true;
        } else {
            forward = isForward;
            right = !isForward;
        }
        reversed = rev;

    }

    public void DeleteItem() {
        TrashCan.GetComponent<DeleteTrashCanScript>().DeleteItem();
    }

}

}
