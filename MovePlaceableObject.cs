using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
 

namespace Spaces{
    public class MovePlaceableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    public bool buttonPressed;

    public GameObject PlacementController;

    public bool isPlacementController;

    public GameObject ItemRemover;

    public bool upOrDown;
    
    public void OnPointerDown(PointerEventData eventData){
        bool isForward = gameObject.name.ToCharArray()[0] == '1';
        bool isReversed = gameObject.name.ToCharArray()[1] != '1';
        if (isPlacementController) {
            PlacementController.GetComponent<ItemPlacementController>().SetForwardInput(true, isForward, isReversed);
        } else {
            ItemRemover.GetComponent<ItemRemover>().SetForwardInput(true, !isForward, isReversed, upOrDown);
        }
        buttonPressed = true;
    }
    
    public void OnPointerUp(PointerEventData eventData){
        if (isPlacementController) {
            PlacementController.GetComponent<ItemPlacementController>().SetForwardInput(false, false, false);
        } else {
            ItemRemover.GetComponent<ItemRemover>().SetForwardInput(false, false, false, false);
        }
        buttonPressed = false;
    }
    }
}