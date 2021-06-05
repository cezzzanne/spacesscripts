using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

        public GameObject DeliveryJobInstructions, ChoppingJobInstructions, ExitGame, InJobBanner, DeliveryJobFinished;

        public TMPro.TextMeshProUGUI DeliveryInfoText;

        public GameObject Compass, ExitJobSign;

        public GameObject PackageProgress;

        private bool inJob = false;

        public GameObject DeliveryWaitForHours;

        public TMPro.TextMeshProUGUI DeliveryWaitMessage;

        public GameObject TakeFlyerButton, LeaveFylerButton;

        public FlyerGameScript flyerGameScript;

        public GameObject SmileEmotion, TalkEmotion, SurpriseEmotion, ThoughtfulEmotion, AngyEmotion, EatingEmotion;

        public GameObject JoinRaceButton;

        public RacingManagerScript racingManagerScript;

        public GameObject RacingLobbyPanel;

        public GameObject RacingButtonPrefab, RacingManagerButtonPrefab;

        public GameObject StartRaceButton, RaceUnavailableButton;


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
            ToggleEmotions(true);
        }

        public void StandUp() {
            StandUpButton.SetActive(false);
            joystick.SetActive(true);
            SitDownCameraChangeB.SetActive(false);
            isSitting = false;
            ToggleEmotions(false);
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
            CoinsB.SetActive(!open);
            GoBackHomeB.SetActive(!open);
            ToggleEmotions(open);
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
            if (!inJob) {
                JobButton.SetActive(open);
            }
        }

        public void ShowDeliverJobLevel(int level) {
            if (level == 0) {
                // set the text to the current level
                DeliveryJobInstructions.SetActive(true);
                DeliveryInfoText.text = "Want to make some money? Go deliver to people the following item for $5 each!. \n Follow the compass to find the correct person (hint: they will be waving at you!)";
            } else {
                DeliveryJobInstructions.SetActive(true);
                DeliveryInfoText.text = "Ooof that was a tough one! There's still a bit more money to be made though if you want to continue! \n Follow the compass to find the correct person (hint: they will be waving at you!)";
            }
        }

        public void AddPlayerToOtherComponents(CharacterScript player) {
            Compass.GetComponent<CompassScript>().SetPlayer(player.transform);
            flyerGameScript.SetPlayer(player);
            racingManagerScript.SetPlayer(player);
        }

        public void CanDoDeliveryIn(TimeSpan time) {
            DeliveryWaitForHours.SetActive(true);
            DeliveryWaitMessage.text = "No work just yet! Come back in " + (5 - time.Hours) + " hours and " + (60 - time.Minutes) + " minutes and we'll have something for you!";
        }

        public void HideCanDoDelivery() {
            DeliveryWaitForHours.SetActive(false);
        }

        public void CloseJob() {
            DeliveryJobInstructions.SetActive(false);
            ToggleInitialState(true);
        }

        public void ToggleInitialState(bool initial) {
            TakeScreenShotB.SetActive(initial);
            GoBackHomeB.SetActive(initial);
            ExitJobSign.SetActive(!initial);
            Compass.SetActive(!initial);
            PackageProgress.SetActive(!initial);
            inJob = !initial;
        }

        public void StartJob() {
            DeliveryJobInstructions.SetActive(false);
            ToggleInitialState(false);
            JobButton.SetActive(false);
        }

        public void EndJob() {
            ToggleInitialState(true);
        }

        public void JobFinished(int type) {
            if (type == 0) {
                DeliveryJobFinished.SetActive(true);
            }
        }

        public void HideFinishedModal() {
            DeliveryJobFinished.SetActive(false);
            ToggleInitialState(true);
        }

        public void ToggleFylerButton(bool open) {
            TakeFlyerButton.SetActive(open);
        }

        public void TakeFlyer() {
            flyerGameScript.TakeFlyer();
            GoBackHomeB.SetActive(false);
            TakeFlyerButton.SetActive(false);
            LeaveFylerButton.SetActive(true);
        }

        public void LeaveFlyer() {
            TakeFlyerButton.SetActive(false);
            flyerGameScript.LeaveFlyer();
            LeaveFylerButton.SetActive(false);
            GoBackHomeB.SetActive(true);
        }

        public void ToggleEmotions(bool open) {
            SmileEmotion.SetActive(open);
            TalkEmotion.SetActive(open);
            SurpriseEmotion.SetActive(open);
            ThoughtfulEmotion.SetActive(open);
            AngyEmotion.SetActive(open);
            EatingEmotion.SetActive(open);
        }

        public void ToggleJoinRaceButton(bool open) {
            JoinRaceButton.SetActive(open);
        }

        public void ToggleRacingLobby(bool open, List<string> playersInLobby) {
            RacingLobbyPanel.SetActive(open);
            if (!open) {
                return;
            }
            foreach(string player in playersInLobby) {
                AddPlayerToRacingLobby(player);
            }
        }

        public void AddPlayerToRacingLobby(string name) {
            // TODO: 
            // instantiate a button (check friend manager)
            // set name
        }

        public void SetRacingLobbyManager(string name) {
            // TODO instantiate racingmanager button
        }

        public void ToggleStartRaceButton(bool open) {
            StartRaceButton.SetActive(open);
        }

        public void ToggleRaceUnavailable(bool open) {
            RaceUnavailableButton.SetActive(open);
        }
    }
}
