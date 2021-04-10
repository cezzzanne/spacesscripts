using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spaces {
    public class UIManagerPublicScript : MonoBehaviour {

        public GameObject SitDownButton;
        
        public GameObject StandUpButton;

        public GameObject GoBackHomeB, TakeScreenShotB, CaptureScreenShotB, CancelScreenShotB;

        public GameObject joystick;

        public GameObject microhponeButton, SitDownCameraChangeB;

        public GameObject LeaveChatB;

        private bool isTalking = false;

        private bool isSitting = false;

        public GameObject ScreenshotNameText;

        private string username;


        public GameObject StoreB;

        public GameObject CoinsB;

        public GameObject JobButton;

        public JobManagerScript jobManager;

        public GameObject DeliveryJobInstructions, ChoppingJobInstructions, ExitGame, InJobBanner;

        public TMPro.TextMeshProUGUI DeliveryInfoText;


        void Start() {
            // SetInitialState();
            username = PlayerPrefs.GetString("username");
            ScreenshotNameText.GetComponent<TMPro.TextMeshProUGUI>().text = "@" + username.ToLower();
        }

        public void ActivateStore(bool open) {
            SitDownButton.SetActive(false);
            StoreB.SetActive(open);
        }

        public void ToggleSittingButton(bool open) {
            SitDownButton.SetActive(open);
        }

 
        public void Sit() {
            isSitting = true;
            SitDownButton.SetActive(false);
            joystick.SetActive(false);
            StandUpButton.SetActive(true);
            SitDownCameraChangeB.SetActive(true);
        }

        public void StandUp() {
            StandUpButton.SetActive(false);
            joystick.SetActive(true);
            SitDownCameraChangeB.SetActive(false);
            isSitting = false;
        }

        public void SetSitDownListeners(System.Action SitDown, System.Action StandUp) {
            Button sitDown = SitDownButton.GetComponent<Button>();
            Button standUp = StandUpButton.GetComponent<Button>();
            sitDown.onClick.RemoveAllListeners();
            standUp.onClick.RemoveAllListeners();
            sitDown.onClick.AddListener(()=> {SitDown();});
            standUp.onClick.AddListener(()=> {StandUp();});
        }

        public void ToggleScreenshot(bool open) {
            GoBackHomeB.SetActive(!open);
            StandUpButton.SetActive(!open && isSitting);
            SitDownButton.SetActive(false);
            SitDownCameraChangeB.SetActive(!open && isSitting);
            TakeScreenShotB.SetActive(!open);
            if (!open) {
                microhponeButton.SetActive(isTalking);
            } else {
                isTalking = microhponeButton.activeSelf;
                microhponeButton.SetActive(false);
            }
            LeaveChatB.SetActive(false);
            CaptureScreenShotB.SetActive(open);
            CancelScreenShotB.SetActive(open);
            joystick.SetActive(!open);
        }

        public void ToggleZoomSitDown(bool zoomed) {
            joystick.SetActive(zoomed);
            StandUpButton.SetActive(!zoomed);
            TakeScreenShotB.SetActive(!zoomed);
        }



        public void ToggleJobButton(bool open, int jobType) {
            if (open) {
                jobManager.SetTypeOfJob(jobType);
            } 
            JobButton.SetActive(open);
        }

        public void ShowDeliverJobLevel(int level) {
            if (level == 0) {
                // set the text to the current level
                DeliveryJobInstructions.SetActive(true);
                DeliveryInfoText.text = "Want to make some money? Go deliver to people the following item. \n Follow the compass to find the correct person (hint: they will be waving at you!)";
            }
        }

        public void CloseJob() {
            DeliveryJobInstructions.SetActive(false);
        }
    }
}
