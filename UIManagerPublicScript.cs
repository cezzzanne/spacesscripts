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

        public GameObject ElevatorFloors, CancelElevatorButton, BuyHomeModal;

        public GameObject EditHomeB, DeleteItemB;

        private bool isTalking = false;

        private bool isSitting = false;
        public GameObject PhoneB, confirmItemB, ItemNameB, ItemSelectPanel, ItemSelectPanelBackdrop, nextItemB, prevItemB, quitSelectorB;

        public GameObject MovePanel, PlacePanel, ShadowMove, ShadowPlace;

        
        public GameObject FriendsB, ClosetB, EditButton;

        private bool tabsOpen = false;

        public GameObject FriendsPanel, TopPanel, closePanelButton;

        public GameObject CreateGroupB, CreateGroupForm, LoadingCreateGroupB, SuccessGroupCreationB, ErrorGroupCreationB; 

        public GameObject JoinGroupB, JoinGroupForm, LoadingJoinGroupB, SuccessJoinGroup, ErrorJoinGroup; 

        public GameObject SummonGroupForm, SummonGroupB, SuccessSummonGroup, SummonGroupSendButton;

        public GameObject InviteFriendB, LoadingFriendInvite, SuccessFriendInviteB, ErrorFriendInviteB;

        public GameObject GroupFriendsPanel, ParentPanel, RequestsPanel, BackToGroupsTopTab, InGroupsTopTab;

        public GameObject LoadingNewMembersB;

        public GameObject MainPanel;

        public GameObject TopPanelInRequests, TopPanelInFriends;

        private bool friendsPanelOpen = true;

        public GameObject AccessoryName, AccessoryNameShadow, BackToMainMenuCharacterSelect;
        
        private string specificType;

        public GameObject ScreenshotNameText;

        private string username;

        private Dictionary<string, GameObject> menuSelectors, CharacterPanels;

        public GameObject ArmPanel, TorsoPanel, FacePanel, ArmS, ShoulderS, HandsS, BackpackS, HolsterS, ExtraS, HariS, CapS;

        public GameObject CharacterChange;

        public GameObject NoAccesoriesButton;

        public GameObject ConfirmSkinB, CancelSkinB, MainMenuCharacterSelect, RotateCharacterB, MainMenuCharacterBack, PreviousSkinB, NextSkinB;

        public GameObject StoreB;

        public GameObject CoinsB;

        public GameObject OwnerOfAppDisplay;

        public GameObject ElevatorLoading;

        public GameObject JobButton;

        public JobManagerScript jobManager;


        void Start() {
            // SetInitialState();
            CharacterPanels = new Dictionary<string, GameObject>() {
            {"Arm", ArmPanel},
            {"Torso", TorsoPanel},
            {"Face", FacePanel},
            };
            menuSelectors = new Dictionary<string, GameObject>() {
                {"Arm", ArmS},
                {"Shoulder", ShoulderS},
                {"Hands", HandsS},
                {"Backpack", BackpackS},
                {"Holster", HolsterS},
                {"Extra", ExtraS},
                {"Hair", HariS},
                {"Cap", CapS},
                {"Skin", ArmS}
            }; 
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

        public void BrowseTypeOfAccessory(string type) {
            AccessoryName.SetActive(true);
            AccessoryNameShadow.SetActive(true);
            string mainType = type.Split('-')[0];
            specificType = type.Split('-')[1];
            BackToMainMenuCharacterSelect.SetActive(true);
            MainMenuCharacterSelect.SetActive(false);
            CancelSkinB.SetActive(false);
            ConfirmSkinB.SetActive(false);
            if (mainType != "Skin") {
                CharacterPanels[mainType].SetActive(true);
            } else {
                AccessoryName.SetActive(false);
                AccessoryNameShadow.SetActive(false);
                MainMenuCharacterBack.SetActive(false);
            }
            BrowseSpecificAccessory(specificType);
        }

        public void BackToCharacterselect() {
            NextSkinB.SetActive(false);
            PreviousSkinB.SetActive(false);
            ArmPanel.SetActive(false);
            TorsoPanel.SetActive(false);
            FacePanel.SetActive(false);
            MainMenuCharacterSelect.SetActive(true);
            MainMenuCharacterBack.SetActive(true);
            RotateCharacterB.SetActive(true);
            BackToMainMenuCharacterSelect.SetActive(false);
            CancelSkinB.SetActive(true);
            ConfirmSkinB.SetActive(true);
            AccessoryName.SetActive(false);
            AccessoryNameShadow.SetActive(false);
            NoAccesoriesButton.SetActive(false);
            menuSelectors[specificType].SetActive(false);
        }

        public void BrowseSpecificAccessory(string type) {
            menuSelectors[specificType].SetActive(false);
            menuSelectors[type].SetActive(true);
            NoAccesoriesButton.SetActive(false);
            specificType = type;
            NextSkinB.SetActive(true);
            PreviousSkinB.SetActive(true);
            CharacterChange.GetComponent<CharacterChange>().SetBrowsingType(type);
        }

        public void ToggleRequestAndFriendsPanel() {
            RequestsPanel.SetActive(friendsPanelOpen);
            TopPanelInRequests.SetActive(friendsPanelOpen);
            TopPanelInFriends.SetActive(!friendsPanelOpen);
            FriendsPanel.SetActive(!friendsPanelOpen);
            friendsPanelOpen = !friendsPanelOpen;
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

        public void ShowElevatorFloors() {
            GoBackHomeB.SetActive(false);
            TakeScreenShotB.SetActive(false);
            ElevatorFloors.SetActive(true);
            CancelElevatorButton.SetActive(true);
        }

        public void HideElevatorFloors() {
            ElevatorFloors.SetActive(false);
            CancelElevatorButton.SetActive(false);
            GoBackHomeB.SetActive(true);
            TakeScreenShotB.SetActive(true);
        }

        public void HideBuyHomeModal() {
            BuyHomeModal.SetActive(false);
        }

        public void ToggleEditMode(bool active) {
            EditButton.SetActive(active);
            // DeleteItemB.SetActive(active);
        } 

        public void PlacedItem() {
            MovePanel.SetActive(false);
            PlacePanel.SetActive(false);
            ShadowMove.SetActive(false);
            ShadowPlace.SetActive(false);
            ToggleEditing(false);
        }

        public void ToggleTabs() {
            tabsOpen = !tabsOpen;
            FriendsB.SetActive(tabsOpen);
            ClosetB.SetActive(tabsOpen);
            EditHomeB.SetActive(tabsOpen);
            CoinsB.SetActive(tabsOpen);
        }

        public void ToggleEditing(bool open) {
            if (!open) {
                PhoneB.SetActive(true);
                confirmItemB.SetActive(false);
                ItemNameB.SetActive(false);
                ItemSelectPanel.SetActive(false);
                ItemSelectPanelBackdrop.SetActive(false);
                nextItemB.SetActive(false);
                prevItemB.SetActive(false);
                quitSelectorB.SetActive(false);
                joystick.SetActive(true);
                // set conversation button active as well as camera button
                TakeScreenShotB.SetActive(true);
            } else {
                PhoneB.SetActive(false);
                TakeScreenShotB.SetActive(false);
                SitDownButton.SetActive(false);
                // set conversation button inactive
                ToggleTabs();
                confirmItemB.SetActive(true);
                ItemNameB.SetActive(true);
                nextItemB.SetActive(true);
                ItemSelectPanel.SetActive(true);
                ItemSelectPanelBackdrop.SetActive(true);
                prevItemB.SetActive(true);
                quitSelectorB.SetActive(true);
                joystick.SetActive(false);
            }
        }

        public void IsPlacingItem() {
            joystick.SetActive(false);
            MovePanel.SetActive(true);
            ShadowMove.SetActive(true);
            PlacePanel.SetActive(true);
            ShadowPlace.SetActive(true);
            PlacePanel.SetActive(true);
            confirmItemB.SetActive(false);
            ItemNameB.SetActive(false);
            ItemSelectPanel.SetActive(false);
            ItemSelectPanelBackdrop.SetActive(false);
            nextItemB.SetActive(false);
            prevItemB.SetActive(false);
            quitSelectorB.SetActive(false);
        }


        public void ToggleCharacterChange(bool open) {
            joystick.SetActive(!open);
            PhoneB.SetActive(!open);
            ToggleTabs();
            TakeScreenShotB.SetActive(!open);
            // consider current conversation hiding
            ConfirmSkinB.SetActive(open);
            CancelSkinB.SetActive(open);
            MainMenuCharacterSelect.SetActive(open);
            RotateCharacterB.SetActive(open);
            MainMenuCharacterBack.SetActive(open);
            PreviousSkinB.SetActive(false);
            NextSkinB.SetActive(false);
        }
        public void TogglePanel(bool open) {
            if (open) {
                MainPanel.SetActive(true);
                TopPanel.SetActive(true);
                closePanelButton.SetActive(true);
                PhoneB.SetActive(false);
                ClosetB.SetActive(false);
                CoinsB.SetActive(false);
                EditHomeB.SetActive(false);
                FriendsB.SetActive(false);
                SitDownButton.SetActive(false);
                TakeScreenShotB.SetActive(false);
            } else {
                MainPanel.SetActive(false);
                TopPanel.SetActive(false);
                TakeScreenShotB.SetActive(true); // TODO: IF in conversation, show again speaking panel
                closePanelButton.SetActive(false);
                PhoneB.SetActive(true);
            }
        }

        public void CreateGroup() {
            CreateGroupB.SetActive(false);
            CreateGroupForm.SetActive(true);
        }

        public void JoinGroup() {
            JoinGroupB.SetActive(false);
            JoinGroupForm.SetActive(true);
        }

        public void HideJoinGroupForm() {
            JoinGroupB.SetActive(true);
            JoinGroupForm.SetActive(false);
        }

        public void LoadingGroupCreation() {
            CreateGroupForm.SetActive(false);
            LoadingCreateGroupB.SetActive(true);
        }


        public void ResultGroupCreation(bool success) {
            LoadingCreateGroupB.SetActive(false);
            if (success) {
                SuccessGroupCreationB.SetActive(true);
            } else {
                ErrorGroupCreationB.SetActive(true);
            }
            StartCoroutine(RevertGroupCreation(success));
        }

        public void LoadingJoin() {
            JoinGroupForm.SetActive(false);
            LoadingJoinGroupB.SetActive(true);
        }

        // refactor and make more abstract all this code

        public void ResultGroupJoin(bool success) {
            LoadingJoinGroupB.SetActive(false);
            if (success) {
                SuccessJoinGroup.SetActive(true);
            } else {
                ErrorJoinGroup.SetActive(true);
            }
            StartCoroutine(RevertGroupJoin(success));
        }

        IEnumerator RevertGroupCreation(bool success) {
            yield return new WaitForSeconds(3);
            SuccessGroupCreationB.SetActive(false);
            ErrorGroupCreationB.SetActive(false);
            CreateGroupB.SetActive(true);
        }

        IEnumerator RevertGroupJoin(bool success) {
            yield return new WaitForSeconds(3);
            SuccessJoinGroup.SetActive(false);
            ErrorJoinGroup.SetActive(false);
            JoinGroupB.SetActive(true); 
        }

        public void ToggleSummonGroupForm(bool open) {
            SummonGroupForm.SetActive(open);
            SummonGroupB.SetActive(!open);
        }

        public void ToggleSummonGroupSendButton(string message) {
            SummonGroupSendButton.GetComponent<Text>().text = (message.Length > 0) ? "send" : "cancel";
        }

        public IEnumerator SuccessSummon() {
            SuccessSummonGroup.SetActive(true);
            SummonGroupForm.SetActive(false);
            yield return new WaitForSeconds(2.5f);
            SuccessSummonGroup.SetActive(false);
            SummonGroupB.SetActive(true);
        }


        public void LoadingInviteFriend() {
            InviteFriendB.SetActive(false);
            LoadingFriendInvite.SetActive(true);
        }

        public void ResultFriendInvite(bool success, string message) {
            InviteFriendB.SetActive(false);
            LoadingFriendInvite.SetActive(false);
            if (success) {
                SuccessFriendInviteB.SetActive(true);
            } else {
                ErrorFriendInviteB.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = message;
                ErrorFriendInviteB.SetActive(true);
            }
            StartCoroutine(RevertInvite(success));
        }

        IEnumerator RevertInvite(bool success) {
            yield return new WaitForSeconds(3);
            SuccessFriendInviteB.SetActive(false);
            ErrorFriendInviteB.SetActive(false);
            InviteFriendB.SetActive(true);  
        }

        public void OpenGroup() {
            GroupFriendsPanel.SetActive(true);
            ParentPanel.GetComponent<ScrollRect>().content = GroupFriendsPanel.GetComponent<RectTransform>();
            FriendsPanel.SetActive(false);
            BackToGroupsTopTab.SetActive(true);
            InGroupsTopTab.SetActive(false);
        }

        public void LoadingNewMemberToggle(bool active) {
            LoadingNewMembersB.SetActive(active);
        }

        public void BackToGroups() {
            BackToGroupsTopTab.SetActive(false);
            InGroupsTopTab.SetActive(true);
            GroupFriendsPanel.SetActive(false);
            FriendsPanel.SetActive(true);
            ParentPanel.GetComponent<ScrollRect>().content = FriendsPanel.GetComponent<RectTransform>();
            LoadingNewMemberToggle(false);
        }

        public void ToggleOwnerAppDisplay(bool open, string username) {
            OwnerOfAppDisplay.SetActive(open);
            OwnerOfAppDisplay.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "@" + username + " owns this home";
        }

        public void GoingToFloor() {
            joystick.SetActive(false);
        }

        public void ArrivedAtFloor() {
            joystick.SetActive(true);
        }

        public void ToggleJobButton(bool open, int jobType) {
            if (open) {
                jobManager.SetTypeOfJob(jobType);
            } 
            JobButton.SetActive(open);
        }
    }
}
