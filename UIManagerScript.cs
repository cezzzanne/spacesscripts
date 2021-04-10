using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spaces {
    public class UIManagerScript : MonoBehaviour {
        public GameObject panel, closePanelButton;

        public GameObject editWorld,goBackHome, openPanelButton;
        public GameObject OpenTabsToggle;

        public GameObject SpeakingPanel;

        public GameObject EditOrGoHomePanel;

        string currentRoomID;

        private bool mapActivated;
        private bool inMyRoom;

        private bool tabsSpread;

        private bool mapIsToggled;

        public GameObject confirmItemB, nextItemB, prevItemB, placeItemB, clearItemB, quitSelectorB, rotateB, ItemNameB;

        private bool isEditing = false;
        public GameObject joystick;


        public GameObject NextSkinB, PreviousSkinB, CancelSkinB, ConfirmSkinB, WardrobeB;

        private bool changingCharacter = false;

        public GameObject TopPanel, FriendsPanel, RequestsPanel; 

        private bool friendsPanelOpen = true;

        public GameObject AddFriendB, AddFriendForm, LoadingFriendReqB, SuccessFriendReqB, ErrorFriendReqB;

        public GameObject InviteFriendB, InviteFriendForm, SuccessFriendInviteB, ErrorFriendInviteB, LoadingFriendInvite;

        public GameObject JoinGroupB, JoinGroupForm, SuccessJoinGroup, ErrorJoinGroup, LoadingJoinGroup;

        public GameObject GroupFriendsPanel;

        public GameObject InGroupsTopTab, InRequestsTopTab, BackToGroupsTopTab;

        public GameObject MovePanel, PlacePanel, ShadowMove, ShadowPlace;

        public GameObject MainMenuCharacterSelect, MainMenuCharacterBack, BackToMainMenuCharacterSelect, ArmPanel, TorsoPanel, FacePanel;

        public Dictionary<string, GameObject> CharacterPanels;

        public GameObject CharacterChange;

        public GameObject AccessoryName, AccessoryNameShadow;

        public Dictionary<string, GameObject> menuSelectors;

        public GameObject ArmS, ShoulderS, HandsS, BackpackS, HolsterS, ExtraS, HariS, CapS;

        public GameObject NoAccesoriesButton;

        string specificType;

        public GameObject RotateCharacterB;

        public GameObject CameraButton, CameraFlashButton, CancelScreenShotButton;

        public GameObject ScreenshotName, ScreenshotLogo, ScreenshotNameText, ScreenshotSuccessButton, ModalPanel, ShareScreenShotButton;

        public GameObject ModalPanelTitle, ModalPanelSubtitle, ModalPanelButtonText;

        public GameObject ItemSelectPanel, ItemSelectPanelBackdrop;
        private string username;

        public GameObject LoadingNewMembersB;

        public GameObject DeleteTopPanel, DeleteTopPanelBack, DeleteBottomPanel, DeleteBottomPanelBack;
        
        public GameObject ParentPanel;

        public GameObject SitDownButton, StandUpButton, SitDownCameraChange;

        private bool isSitting = false;

        public GameObject JoinGameB, JoinGameBText;

        public GameObject GamingManager, GamePanel, FullGamePanel;
        
        private bool isGamePanelOpen = false;

        public GameObject PublicWorldsB, PublicWorldsPanel;

        public GameObject PublicWorldsAns, CoinsAns, GroupsAns, GamesAns;
        
        private GameObject CurrentOpenAns;

        public GameObject SummonGroupB, SummonGroupForm, SummonGroupSendButton, SuccessSummonGroup;

        void Start() {
            inMyRoom = PlayerPrefs.GetString("currentRoomID") == PlayerPrefs.GetString("myRoomID");
            SetInitialState();
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

        public void SetInitialState() {
            panel.SetActive(false);
            TopPanel.SetActive(false);
            FriendsPanel.SetActive(true);
            ParentPanel.GetComponent<ScrollRect>().content = FriendsPanel.GetComponent<RectTransform>();
            RequestsPanel.SetActive(false);
            tabsSpread = false;
            mapIsToggled = false;
            openPanelButton.SetActive(false);
            StandUpButton.SetActive(false);
            SitDownCameraChange.SetActive(false);
            SitDownButton.SetActive(false);
            closePanelButton.SetActive(false);
            FullGamePanel.SetActive(false);
            SpeakingPanel.SetActive(true);
            OpenTabsToggle.SetActive(true);
            EditOrGoHomePanel.SetActive(false);
            GamePanel.SetActive(false);
            CameraButton.SetActive(true);
            JoinGameB.SetActive(false);
            SitDownCameraChange.SetActive(false);
            if (inMyRoom) {
                goBackHome.SetActive(false);
            } else {
                editWorld.SetActive(false);
                goBackHome.SetActive(true);
            }        
        }

        public void ActivateEditing() {
            editWorld.SetActive(true);
        }

        public void TogglePanel(bool open) {
            if (open) {
                panel.SetActive(true);
                TopPanel.SetActive(true);
                closePanelButton.SetActive(true);
                OpenTabsToggle.SetActive(false);
                OpenTabsToggle.SetActive(false);
                EditOrGoHomePanel.SetActive(false);
                GamePanel.SetActive(false);
                SitDownButton.SetActive(false);
                JoinGameB.SetActive(false);
                CameraButton.SetActive(false);
                SpeakingPanel.SetActive(false);
                openPanelButton.SetActive(false);
            } else {
                panel.SetActive(false);
                TopPanel.SetActive(false);
                CameraButton.SetActive(true);
                SpeakingPanel.SetActive(true);
                closePanelButton.SetActive(false);
                OpenTabsToggle.SetActive(true);
            }
            if (isGamePanelOpen && !open) { 
                CloseGamePanel();
            } else if (isGamePanelOpen && open) {
                TopPanel.SetActive(false);
            }
        }

        public void CloseGamePanel() {
            FriendsPanel.SetActive(true);
            FullGamePanel.SetActive(false);
            isGamePanelOpen = false;
            GoToGroups();
            BackToGroups();
        }

        public void OpenGamesPanel() {
            isGamePanelOpen = true;
            FriendsPanel.SetActive(false);
            RequestsPanel.SetActive(false);
            GroupFriendsPanel.SetActive(false);
            FullGamePanel.SetActive(true);
            TogglePanel(true);
        }

        public void GoToFriendsRoom() {
            inMyRoom = false;
            SetInitialState();
            BackToGroups();
        }

        public void GoHomeCallback() {
            inMyRoom = true;
            SetInitialState();
            editWorld.SetActive(true);
        }

        public void SpreadTabs() {
            if (tabsSpread) {
                EditOrGoHomePanel.SetActive(false);
                GamePanel.SetActive(false);
                openPanelButton.SetActive(false);
                tabsSpread = false;
            } else {
                EditOrGoHomePanel.SetActive(true);
                openPanelButton.SetActive(true);
                GamePanel.SetActive(true);
                tabsSpread = true;
            }
        }

  

        public void ToggleEditing() {
            if (isEditing) {
                OpenTabsToggle.SetActive(true);
                confirmItemB.SetActive(false);
                ItemNameB.SetActive(false);
                ItemSelectPanel.SetActive(false);
                ItemSelectPanelBackdrop.SetActive(false);
                rotateB.SetActive(false);
                nextItemB.SetActive(false);
                prevItemB.SetActive(false);
                quitSelectorB.SetActive(false);
                joystick.SetActive(true);
                SpeakingPanel.SetActive(true);
                CameraButton.SetActive(true);
            } else {
                OpenTabsToggle.SetActive(false);
                CameraButton.SetActive(false);
                SitDownButton.SetActive(false);
                JoinGameB.SetActive(false);
                SpeakingPanel.SetActive(false);
                if (tabsSpread) {
                    SpreadTabs();
                }
                confirmItemB.SetActive(true);
                ItemNameB.SetActive(true);
                nextItemB.SetActive(true);
                ItemSelectPanel.SetActive(true);
                ItemSelectPanelBackdrop.SetActive(true);
                prevItemB.SetActive(true);
                quitSelectorB.SetActive(true);
                joystick.SetActive(false);
            }
            isEditing = !isEditing;
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

        public void PlacedItem() {
            MovePanel.SetActive(false);
            PlacePanel.SetActive(false);
            ShadowMove.SetActive(false);
            ShadowPlace.SetActive(false);
            placeItemB.SetActive(false);
            clearItemB.SetActive(false);
            rotateB.SetActive(false);
            ToggleEditing();
        }

        public void FinishedEditing() {
            //
        }

        public void ToggleWardrobe(bool open) {
            WardrobeB.SetActive(open);
        }

        public void ToggleCharacterChange() {
            joystick.SetActive(changingCharacter);
            OpenTabsToggle.SetActive(changingCharacter);
            CameraButton.SetActive(changingCharacter);
            SpeakingPanel.SetActive(changingCharacter);
            WardrobeB.SetActive(changingCharacter);
            if (tabsSpread && !changingCharacter) {
                    SpreadTabs();
            }
            ConfirmSkinB.SetActive(!changingCharacter);
            CancelSkinB.SetActive(!changingCharacter);
            MainMenuCharacterSelect.SetActive(!changingCharacter);
            RotateCharacterB.SetActive(!changingCharacter);
            MainMenuCharacterBack.SetActive(!changingCharacter);
            PreviousSkinB.SetActive(false);
            NextSkinB.SetActive(false);
            if (changingCharacter) {
                SetInitialState();
            }
            changingCharacter = !changingCharacter;
        }

        public void ToggleRequestAndFriendsPanel() {
            RequestsPanel.SetActive(friendsPanelOpen);
            FriendsPanel.SetActive(!friendsPanelOpen);
            friendsPanelOpen = !friendsPanelOpen;
        }

        public void AddGroupUsername() {
            AddFriendB.SetActive(false);
            AddFriendForm.SetActive(true);
        }

        public void JoinGroup() {
            JoinGroupB.SetActive(false);
            JoinGroupForm.SetActive(true);
        }

        public void HideJoinGroupForm() {
            JoinGroupB.SetActive(true);
            JoinGroupForm.SetActive(false);
        }

        public void HideInviteFriendForm() {
            InviteFriendB.SetActive(true);
            InviteFriendForm.SetActive(false);
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
        public void LoadingGroupCreation() {
            AddFriendForm.SetActive(false);
            LoadingFriendReqB.SetActive(true);
        }

        public void ResultGroupCreation(bool success) {
            LoadingFriendReqB.SetActive(false);
            if (success) {
                SuccessFriendReqB.SetActive(true);
            } else {
                ErrorFriendReqB.SetActive(true);
            }
            StartCoroutine(RevertGroupCreation(success));
        }

        public void LoadingJoin() {
            JoinGroupForm.SetActive(false);
            LoadingJoinGroup.SetActive(true);
        }

        // refactor and make more abstract all this code


        public void ResultGroupJoin(bool success) {
            LoadingJoinGroup.SetActive(false);
            if (success) {
                SuccessJoinGroup.SetActive(true);
            } else {
                ErrorJoinGroup.SetActive(true);
            }
            StartCoroutine(RevertGroupJoin(success));
        }

        IEnumerator RevertGroupCreation(bool success) {
            yield return new WaitForSeconds(3);
            SuccessFriendReqB.SetActive(false);
            ErrorFriendReqB.SetActive(false);
            AddFriendB.SetActive(true);
            if (success) {
                GoToGroups();
            }
        }

        IEnumerator RevertGroupJoin(bool success) {
            yield return new WaitForSeconds(3);
            SuccessJoinGroup.SetActive(false);
            ErrorJoinGroup.SetActive(false);
            JoinGroupB.SetActive(true);
            if (success) {
                GoToGroups();
            }   
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
            RequestsPanel.SetActive(false); // only for precaution
            BackToGroupsTopTab.SetActive(true);
            InGroupsTopTab.SetActive(false);
        }

        public void GoToRequests() {
            InGroupsTopTab.SetActive(false);
            InRequestsTopTab.SetActive(true);
            RequestsPanel.SetActive(true);
            ParentPanel.GetComponent<ScrollRect>().content = RequestsPanel.GetComponent<RectTransform>();
            FriendsPanel.SetActive(false);
        }

        public void GoToGroups() {
            FriendsPanel.SetActive(true);
            ParentPanel.GetComponent<ScrollRect>().content = FriendsPanel.GetComponent<RectTransform>();
            RequestsPanel.SetActive(false);
            InRequestsTopTab.SetActive(false);
            InGroupsTopTab.SetActive(true);

        }

        public void BackToGroups() {
            BackToGroupsTopTab.SetActive(false);
            InGroupsTopTab.SetActive(true);
            GroupFriendsPanel.SetActive(false);
            FriendsPanel.SetActive(true);
            PublicWorldsPanel.SetActive(false);
            ParentPanel.GetComponent<ScrollRect>().content = FriendsPanel.GetComponent<RectTransform>();
            LoadingNewMemberToggle(false);
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

        public void BrowseSpecificAccessory(string type) {
            menuSelectors[specificType].SetActive(false);
            menuSelectors[type].SetActive(true);
            NoAccesoriesButton.SetActive(false);
            specificType = type;
            NextSkinB.SetActive(true);
            PreviousSkinB.SetActive(true);
            CharacterChange.GetComponent<CharacterChange>().SetBrowsingType(type);
        }

        public void TakeScreenshot(bool open) {
            if (tabsSpread && open) {
                SpreadTabs();
            }
            joystick.SetActive(!open);
            OpenTabsToggle.SetActive(!open);
            CameraButton.SetActive(!open);
            SitDownButton.SetActive(false);
            JoinGameB.SetActive(false);
            StandUpButton.SetActive(!open && isSitting);
            SitDownCameraChange.SetActive(!open && isSitting);
            CameraFlashButton.SetActive(open);
            CancelScreenShotButton.SetActive(open);
        }

        public void ToggleCaputreScreenshot(bool isTakingShot) {
            CameraFlashButton.SetActive(!isTakingShot);
            CancelScreenShotButton.SetActive(!isTakingShot);
            SpeakingPanel.SetActive(!isTakingShot);
            ScreenshotLogo.SetActive(isTakingShot);
            ScreenshotName.SetActive(isTakingShot);
        }

        public void CaputreScreenshotResult(bool success) {
            if (success) {
                ScreenshotName.SetActive(false);
                ScreenshotLogo.SetActive(false);
                ScreenshotSuccessButton.SetActive(true);
                ShareScreenShotButton.SetActive(true);
                // 
            } else {
                ToggleCaputreScreenshot(false);
                SetUpPanel("There was an error taking your screenshot", "In settings, make sure you have permissions enabled for spaces to take screenshots", "try again");
            }
        }

        public void CloseShareScreenShot() {
            ScreenshotSuccessButton.SetActive(false);
            ShareScreenShotButton.SetActive(false);
            ToggleCaputreScreenshot(false);
        }

        public void CloseModalPanel() {
            ModalPanel.SetActive(false);
        }

        public void SetUpPanel(string title, string text, string buttonText) {
            ModalPanelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = title;
            ModalPanelSubtitle.GetComponent<TMPro.TextMeshProUGUI>().text = text;
            ModalPanelButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = buttonText;
            ModalPanel.SetActive(true);
        }

        public void LoadingNewMemberToggle(bool active) {
            LoadingNewMembersB.SetActive(active);
        }

        public void StartDeleting() {
            if (tabsSpread) {
                SpreadTabs();
            }
            OpenTabsToggle.SetActive(false);
            joystick.SetActive(false);
            CameraButton.SetActive(false);
            SpeakingPanel.SetActive(false);
            DeleteBottomPanel.SetActive(true);
            DeleteTopPanel.SetActive(true);
            DeleteBottomPanelBack.SetActive(true);
            DeleteTopPanelBack.SetActive(true);
        }

        public void CancelDeleting() {
            DeleteBottomPanel.SetActive(false);
            DeleteTopPanel.SetActive(false);
            DeleteBottomPanelBack.SetActive(false);
            DeleteTopPanelBack.SetActive(false);
            joystick.SetActive(true);
            SetInitialState();
        }

        public void ToggleSittingButton(bool open) {
            SitDownButton.SetActive(open);
        }

        public void Sit() {
            isSitting = true;
            SitDownButton.SetActive(false);
            StandUpButton.SetActive(true);
            SitDownCameraChange.SetActive(true);
            joystick.SetActive(false);
        }

        public void StandUp() {
            StandUpButton.SetActive(false);
            joystick.SetActive(true);
            SitDownCameraChange.SetActive(false);
            isSitting = false;
        }

        public void SetSitDownListeners(System.Action SitDown, System.Action StandUp) {
            Button sitDown = SitDownButton.GetComponent<Button>();
            Button standUp = StandUpButton.GetComponent<Button>();
            standUp.onClick.RemoveAllListeners();
            sitDown.onClick.RemoveAllListeners();
            sitDown.onClick.AddListener(()=> {SitDown();});
            standUp.onClick.AddListener(()=> {StandUp();});
        }

        public void ToggleZoomSitDown(bool zoomed) {
            joystick.SetActive(zoomed);
            StandUpButton.SetActive(!zoomed);
            CameraButton.SetActive(!zoomed);
        }

        public void ShowJoinGame(string gameName, string username, string gameCode) {
            JoinGameBText.GetComponent<TMPro.TextMeshProUGUI>().text = "Join @" + username + " in " + gameName;
            GamingManager.GetComponent<GamingManager>().SetPotentialGame(gameCode);
            JoinGameB.SetActive(true);
        }
        
        // far from optimal but I don't want to Find() more stuff in character script and I already have uimanager
        public void SetGamingManagerCharacter(CharacterScript cs) {
            GamingManager.GetComponent<GamingManager>().SetCharacterScript(cs);
        }

        public void HideJoinGame() {
            JoinGameB.SetActive(false);
        }

        public void OpenPublicWorldsPanel() {
            FriendsPanel.SetActive(false);
            PublicWorldsPanel.SetActive(true);
            BackToGroupsTopTab.SetActive(true);
        }

        public void ToggleAnswer(string question) {
            GameObject newOpenAnswer = null;
            if (question == "publicWorlds") {
                PublicWorldsAns.SetActive(!PublicWorldsAns.activeSelf);
                newOpenAnswer = PublicWorldsAns;
            } else if (question == "coins") {
                CoinsAns.SetActive(!CoinsAns.activeSelf);
                newOpenAnswer = CoinsAns;
            } else if (question == "groups") {
                GroupsAns.SetActive(!GroupsAns.activeSelf);
                newOpenAnswer = GroupsAns;
            } else if (question == "games") {
                GamesAns.SetActive(!GamesAns.activeSelf);
                newOpenAnswer = GamesAns;
            }
            if (CurrentOpenAns != newOpenAnswer && CurrentOpenAns != null) {
                CurrentOpenAns.SetActive(false);
                CurrentOpenAns = newOpenAnswer;
            } else {
                CurrentOpenAns = newOpenAnswer;
            }
        }

        public void OpenSummonGroupForm(){
            SummonGroupB.SetActive(false);
            SummonGroupForm.SetActive(true);
        }

        public void HideSummonGroupForm() {
            SummonGroupForm.SetActive(false);
            SummonGroupB.SetActive(true);
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

    }
}
