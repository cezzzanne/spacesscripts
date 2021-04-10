using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Photon.Pun;

namespace Spaces {
    public class InputHandler : MonoBehaviour {

        public GameObject inputField;

        public GameObject joystick, CameraButton, GoBackHomeButton;

        private GameObject toolbar;

        public TouchScreenKeyboard keyboard;

        Transform inputTransform;
        
        private bool keyboardActive = false;

        CharacterScript player;

        void Start() {
            TouchScreenKeyboard.hideInput = true;
            toolbar = Instantiate(inputField, inputField.transform.position, inputField.transform.rotation);
            toolbar.SetActive(false);
            toolbar.transform.SetParent(transform, false);
        }

        public void SetTarget(CharacterScript myPlayer) {
            player = myPlayer;
        }

        public void SendNewMessage(string message) {
            if (player != null) {
                player.SendNewMessage(message);
            }
        }

        void Update() {
            if (keyboard != null) {
                // comment this to test on editor 
                if (keyboard.active && TouchScreenKeyboard.visible) {
                    toolbar.SetActive(true);
                    toolbar.transform.position = new Vector3(toolbar.transform.position.x, TouchScreenKeyboard.area.height, toolbar.transform.position.z);
                    inputTransform = toolbar.transform.GetChild(0);
                    keyboardActive = true;
                    EventSystem.current.SetSelectedGameObject(inputTransform.gameObject, null);
                } else {
                    if (toolbar.activeSelf) {
                        CloseTyping();
                    }
                }
            }
        }

        public void ToggleKeyboard() {
            joystick.SetActive(false);
            CameraButton.SetActive(false);
            GoBackHomeButton.SetActive(false);
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);   
            // To test on editor -> comment out  
            // toolbar.SetActive(true);
            // toolbar.transform.position = new Vector3(toolbar.transform.position.x, 15, toolbar.transform.position.z);
            // inputTransform = toolbar.transform.GetChild(0);
            // keyboardActive = true;
            // EventSystem.current.SetSelectedGameObject(inputTransform.gameObject, null);
        }

        public void CloseTyping() {
            toolbar.SetActive(false);
            joystick.SetActive(true);
            CameraButton.SetActive(true);
            GoBackHomeButton.SetActive(true);
        }

    }
}
