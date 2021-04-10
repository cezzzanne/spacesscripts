using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Spaces {
    public class PublicWorldChatManager : MonoBehaviour {
        // Start is called before the first frame update
        public GameObject RequestConvoB, RequestSentConvoB;
        public GameObject RejectConvoB;
        public GameObject InviteConvoB, CurrentGroupChat;
        private CharacterScript player;

        private string tempGroup;

        private int otherPVID;

        public GameObject VoiceManager;

        public GameObject CurrentGroupChatBubble;

        public GameObject LeaveGroupChatB;

        void Start() {
            
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void SetMainCharacter(CharacterScript character) {
            player = character;
        }

        public void ShowRequestConversation(bool open, bool morePeople, string username, string currentlyInConvo, int actorNumber, int interestGroup, int myActorNumber, int PVID) {
            if (open) {
                otherPVID = PVID;
                if (morePeople) {
                    tempGroup = interestGroup.ToString();
                    int playersInConvo = currentlyInConvo.Split(';').Length;
                    TMPro.TextMeshProUGUI mainText = RequestConvoB.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                    mainText.text = "Start conversation with " + username;
                } else {
                    tempGroup = string.Concat(myActorNumber.ToString(), actorNumber.ToString());
                    TMPro.TextMeshProUGUI mainText = RequestConvoB.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                    mainText.text = "Start conversation with " + username;
                }
                RequestConvoB.SetActive(true);
                RequestConvoB.GetComponent<Button>().onClick.RemoveAllListeners();
                RequestConvoB.GetComponent<Button>().onClick.AddListener(()=> {SendChatRequest(tempGroup, PVID);});
            } else {
                RequestConvoB.SetActive(false);
            }
        }

        public void SendChatRequest(string tempGroup, int otherPVID) {
            player.SendConversationRequest(tempGroup, otherPVID);
            RequestConvoB.SetActive(false);
            RequestSentConvoB.SetActive(true);
            Button cancelReqButton = RequestSentConvoB.transform.GetChild(1).GetChild(0).GetComponent<Button>();
            cancelReqButton.onClick.RemoveAllListeners();
            cancelReqButton.onClick.AddListener(()=> {CancelRequestSent(tempGroup, otherPVID);});
        }

        public void CancelRequestSent(string tempGroup, int otherPVID) {
            player.CancelRequestSent(tempGroup, otherPVID);
            RequestSentConvoB.SetActive(false);
        }

        // this function is when someone sends you a conversation
        public void ShowConversationRequest(string username, string group, int PVID) {
            RequestConvoB.SetActive(false);
            TMPro.TextMeshProUGUI mainText = InviteConvoB.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            mainText.text = "@" + username + " wants to join your conversation!";
            Button rejectButton = InviteConvoB.transform.GetChild(2).GetComponent<Button>();
            Button acceptButton = InviteConvoB.transform.GetChild(3).GetComponent<Button>();
            acceptButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(()=> {AcceptConversationRequest(username, group, PVID);});
            rejectButton.onClick.AddListener(()=> {RejectConversationRequest(username, group, PVID);});
            InviteConvoB.SetActive(true);
        }

        public void AcceptConversationRequest(string username, string group, int PVID) {
            Debug.Log("zzzthird pvid " + PVID);
            player.AcceptConversationRequest(username, group, PVID, false);
        }

        public void RejectConversationRequest(string username, string group, int PVID) {
            InviteConvoB.SetActive(false);
            player.RejectConversationRequest(username, group, PVID);
        }

        public void ShowCurrentConversation(string currentMembersInGroup) {
            VoiceManager.GetComponent<SoundManager>().StartSpeaking();
            InviteConvoB.SetActive(false);
            RequestConvoB.SetActive(false);
            RequestSentConvoB.SetActive(false);
            // string text = "";
            // if (currentMembersInGroup.IndexOf(';') == -1) {
            //     text = currentMembersInGroup;
            // } else {
            //     string names = "";
            //     foreach(string name in currentMembersInGroup.Split(';')) {
            //         names = "@" + name + ", " + names;
            //     }
            //     text = names;
            // }
            // CurrentGroupChat.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text =  "Currently in a group chat";
            CurrentGroupChatBubble.SetActive(true);
        }

        public void ShowRequestReject(string username) {
            TMPro.TextMeshProUGUI mainText = RejectConvoB.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            mainText.text = username + " can't join a conversation right now :/";
            RejectConvoB.SetActive(true);
            StartCoroutine(HideRejection());
        }

        IEnumerator HideRejection() {
            yield return new WaitForSeconds(2);
            RejectConvoB.SetActive(false);
            RequestSentConvoB.SetActive(false);
        }

        public void RemovePotentialConvo() {
            InviteConvoB.SetActive(false);
        }

        public void LeaveConversation() {
            CurrentGroupChat.SetActive(false);
            CurrentGroupChatBubble.SetActive(false);
            VoiceManager.GetComponent<SoundManager>().LeaveConversation();
            player.LeaveConversation();
            LeaveGroupChatB.SetActive(false);
        }

        public void ResetToStart() {
            LeaveConversation();
            RequestConvoB.SetActive(false);
            InviteConvoB.SetActive(false);
            RequestSentConvoB.SetActive(false);
        }

        public void ToggleLeaveGroupChatButton() {
            LeaveGroupChatB.SetActive(!LeaveGroupChatB.activeSelf);
        }

        public void DissapearConversation() {
            RequestConvoB.SetActive(false);
        }

    }
}
