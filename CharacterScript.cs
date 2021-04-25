using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using Photon.Voice.PUN;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using AdvancedCustomizableSystem;

namespace Spaces {
    public class CharacterScript : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback {
        Joystick joystick;
        public Animator animator;
        public float inputDelay = 0.1f;
        public float forwardVel = 12;
        public float rotateVel = 100;
        Quaternion targetRotation;
        Rigidbody rBody;
        float forwardInput, turnInput = 0;

        public bool otherPlayer = false;

        public GameObject mainCam;


        public CharacterController characterController;

        Vector3 testPosition;

        Quaternion testRotation;

        public Material[] allSkins;

        private double lastUpdatetime = 0.0f;
        Vector3 prevPos;
        Quaternion prevRot;
        PhotonView PV;

        public GameObject glassedPrefab;
        private int inRoomState = -1;
        private int inPublicRoom;

        private GameObject ChatButton;

        private GameObject toolbar;

        public TouchScreenKeyboard keyboard;

        Transform inputTransform;
        
        private bool keyboardActive = false;

        private GameObject itemLoader;

        private bool myRoom;
        private bool wardrobeOpen = false;
        MapPlayerScript mapPlayerScript;

        private string username, roomID, myRoomID;

        public List<GameObject> previousAccesories;

        public Dictionary<int, List<GameObject>> friendsPreviousAccessories;

        public int publicWorldInterestGroup;
        public string currentlyTalkingTo;

        public bool isInConversation = false;

        PublicWorldChatManager ChatManager;

        private bool actiavteChat = false;

        private GameObject uiManager;

        private string currentGameType;

        private string currentGameCode;

        public bool inGame = false;

        private string charSetup;

        public Quaternion TargetRotation() {
            return transform.rotation;
        }
        void Awake() {
            inPublicRoom = PlayerPrefs.GetInt("isInPublicWorld");
            friendsPreviousAccessories = new Dictionary<int, List<GameObject>>();
            itemLoader = GameObject.FindGameObjectWithTag("ItemLoader");
            if (!photonView.IsMine) {
                otherPlayer = true;
                if (inPublicRoom == 1) {
                    ChatManager = (GameObject.FindGameObjectWithTag("ChatManager") as GameObject).GetComponent<PublicWorldChatManager>();
                }
            } else {
                charSetup = PlayerPrefs.GetString("myCharacter");
                SetUpCharacter(charSetup);
                mainCam = Resources.Load("Main Camera") as GameObject;
                mainCam = Instantiate(mainCam);
                PlayerFollow cameraScript = mainCam.GetComponent<PlayerFollow>();
                cameraScript.SetCameraTarget(characterController.transform, inPublicRoom);
                GameObject itemControllerObject = GameObject.FindGameObjectWithTag("ItemPlacementController") as GameObject;
                PV = transform.GetComponent<PhotonView>();
                username = PlayerPrefs.GetString("username");
                previousAccesories = new List<GameObject>();
                PV.RPC("RPC_ChangeCharacterName", RpcTarget.AllBuffered, username, PV.ViewID);
                PV.RPC("RPC_SetCharacterAccessories", RpcTarget.AllBuffered, charSetup, PV.ViewID);
                if (inPublicRoom == 1) {
                    ChatManager = (GameObject.FindGameObjectWithTag("ChatManager") as GameObject).GetComponent<PublicWorldChatManager>();
                    ChatManager.SetMainCharacter(this);
                    publicWorldInterestGroup = -1;
                    currentlyTalkingTo = "";
                    GameObject.FindGameObjectWithTag("Canvas").GetComponent<InputHandler>().SetTarget(this);
                    itemLoader.GetComponent<ItemLoaderStore>().SetMainCam(mainCam.GetComponent<PlayerFollow>());
                    uiManager = itemLoader.GetComponent<ItemLoaderStore>().ReturnUIManager();
                    uiManager.GetComponent<UIManagerPublicScript>().AddPlayerToCompass(transform);
                    // CHANGED Back
                    // itemLoader.GetComponent<ItemLoaderv2>().SetCamera(mainCam.GetComponent<PlayerFollow>());
                    // iManager = itemLoader.GetComponent<ItemLoaderv2>().ReturnUIManager();
                    // GameObject itemController = GameObject.FindGameObjectWithTag("ItemPlacementController");
                    // itemController.GetComponent<ItemPlacementControllerV2>().SetTarget(transform);
                    // CharacterChange charChange = GameObject.FindGameObjectWithTag("CharacterChange").GetComponent<CharacterChange>();
                    // charChange.SetTargetCharacter(this);
                    // charChange.UpdateAccessories(accessories);
                    // GameObject notificationManager = GameObject.FindGameObjectWithTag("NotificationManager");
                    // notificationManager.GetComponent<InnerNotifManagerScript>().SetCharacterTarget(transform, username, myRoomID);
                    // CHANGED Back
                    uiManager.GetComponent<UIManagerPublicScript>().SetSitDownListeners(SitDown, StandUp);
                    SetParticipateInConvoCollider();
                    StartCoroutine(ActivateChat());
                } else {
                    roomID = PlayerPrefs.GetString("currentRoomID");
                    myRoomID = PlayerPrefs.GetString("myRoomID");
                    myRoom = roomID == myRoomID;
                    itemLoader.GetComponent<ItemLoader>().SetCamera(mainCam.GetComponent<PlayerFollow>());
                    uiManager = itemLoader.GetComponent<ItemLoader>().ReturnUIManager();
                    uiManager.GetComponent<UIManagerScript>().SetSitDownListeners(SitDown, StandUp);
                    uiManager.GetComponent<UIManagerScript>().SetGamingManagerCharacter(this);
                    uiManager.GetComponent<UIManagerScript>().AddCameraToCustomizer(mainCam.GetComponent<PlayerFollow>());
                    SetParticipateInConvoCollider();
                    if (myRoom) {
                        GameObject itemController = GameObject.FindGameObjectWithTag("ItemPlacementController");
                        itemController.GetComponent<ItemPlacementController>().SetTarget(transform);
                        CharacterChange charChange = GameObject.FindGameObjectWithTag("CharacterChange").GetComponent<CharacterChange>();
                        charChange.SetTargetCharacter(this);
                        // charChange.UpdateAccessories(accessories);
                        GameObject notificationManager = GameObject.FindGameObjectWithTag("NotificationManager");
                        notificationManager.GetComponent<InnerNotifManagerScript>().SetCharacterTarget(transform, username, myRoomID);
                    } else {
                        GameObject notificationManager = GameObject.FindGameObjectWithTag("NotificationManager");
                        notificationManager.GetComponent<InnerNotifManagerScript>().SetCoinsforUser(username);
                    }
                }
            }
        }

        void SetParticipateInConvoCollider() {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(0, 3, 0);
            collider.size = collider.size * 2;
        }

        IEnumerator ActivateChat() {
            yield return new WaitForSeconds(3);
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("withMatter").GetValueAsync().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                string val = snapshot.Value.ToString();
                Debug.Log("zzzval " + val);
                actiavteChat = (val == "yes");
            });
        }

        void SetCharacterAccessories(string accss, List<GameObject> prevAccss, Transform character) {
            string accessories = accss;
            // using same names as local variables - ignore that
            if (!accessories.Contains("$")) {
                if (accessories != "") {
                    // one accessory
                    string[] accessoryAttribute = accessories.Split('-');
                    string location = accessoryAttribute[3];
                    string bodyLocation = accessoryAttribute[0];
                    GameObject acc = Resources.Load<GameObject>("Characters/Accessories/" + location);
                    acc = Instantiate(acc);
                    prevAccss.Add(acc);
                    Transform parent = character.Find(bodyLocation);
                    AllocateAccessory(acc.transform, parent);
                } else {
                    // no accessories
                    return;
                }
            } else {
                // multiple accessories
                string[] allAccessories = accessories.Split('$');
                foreach(string accessory in allAccessories) {
                    string[] accessoryAttribute = accessory.Split('-');
                    string location = accessoryAttribute[3];
                    string bodyLocation = accessoryAttribute[0];
                    GameObject acc = Resources.Load<GameObject>("Characters/Accessories/" + location);
                    acc = Instantiate(acc);
                    prevAccss.Add(acc);
                    Transform parent = character.Find(bodyLocation);
                    AllocateAccessory(acc.transform, parent);
                }
            }
            // Move store to first tent
        }

        public void UpdateCharacter(string json) {
            SetUpCharacter(json);
            PV.RPC("RPC_UpdateCharacterAccessories", RpcTarget.AllBuffered, json, PV.ViewID);
        }

        // Below not being used anymore
        public void UpdateMyAccessories(string newAccessories) {
            PV.RPC("RPC_UpdateCharacterAccessories", RpcTarget.AllBuffered, newAccessories, PV.ViewID);
        }

        public void UpdateCharacterAccessories(string newAccessories, List<GameObject> prevAcc, Transform character) {
            DestroyPreviousAccessories(prevAcc);
            if (!newAccessories.Contains("$")) {
                if (newAccessories != "") {
                    // one accessory
                    // 0 is the Roots/... -- 1 is the Name -- 2 is the Type (Cap) -- 3 is the GameObject name (astroBackpack)
                    string[] accessoryAttribute = newAccessories.Split('-');
                    string location = accessoryAttribute[3];
                    string bodyLocation = accessoryAttribute[0];
                    GameObject acc = Resources.Load<GameObject>("Characters/Accessories/" + location);
                    acc = Instantiate(acc);
                    prevAcc.Add(acc);
                    Transform parent = character.Find(bodyLocation);
                    AllocateAccessory(acc.transform, parent);
                } else {
                    return;
                }
            } else {
                string[] allAccessories = newAccessories.Split('$');
                foreach(string accessory in allAccessories) {
                    string[] accessoryAttribute = accessory.Split('-');
                    string location = accessoryAttribute[3];
                    string bodyLocation = accessoryAttribute[0];
                    GameObject acc = Resources.Load<GameObject>("Characters/Accessories/" + location);
                    acc = Instantiate(acc);
                    prevAcc.Add(acc);
                    Transform parent = character.Find(bodyLocation);
                    AllocateAccessory(acc.transform, parent);
                }
            }
        }

        void DestroyPreviousAccessories(List<GameObject> prevAccess) {
            foreach(GameObject accessory in prevAccess) {
                RemoveAccessory(accessory);
            }
        }

        public void RemoveAccessory(GameObject accessory) {
            Destroy(accessory);
        }

        public void AllocateAccessory(Transform child, Transform parent) {
            Vector3 pos = child.position;
            Quaternion rot = child.rotation;
            Vector3 scale = child.localScale;
            child.parent = parent;
            child.localPosition = pos;
            child.localRotation = rot;
            child.localScale = scale;
        }

        public void SendNewMessage(string message) {
            PV.RPC("RPC_ReceiveMessage", RpcTarget.All, message, PV.ViewID);
        }



        public bool OtherPlayer() {
            return otherPlayer;
        }

       

        void Start() {
            joystick = FindObjectOfType<Joystick>();
            targetRotation = transform.rotation;
            //rBody = GetComponent<Rigidbody>();
            forwardInput = turnInput = 0;
            // characterController = transform.GetComponent<CharacterController>();
            if (otherPlayer) {
                StartCoroutine(CheckSittingPosition());
            }
            // StartCoroutine(Liftoff());
        }

        IEnumerator Liftoff() {
            yield return new WaitForSeconds(5);
            sitting = true;
            float desiredY = transform.position.y + 10;
            GetComponent<CharacterController>().enabled = false;
            while (Mathf.Abs(transform.position.y - desiredY) > 1) {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                yield return new WaitForSeconds(0.012f);
            }
            sitting = false;
            GetComponent<CharacterController>().enabled = true;
        }


        void GetInput() {
            if (otherPlayer) {
                // do something with photon
            } else {
                if (joystick == null) {
                    return;
                }
                forwardInput =  joystick.Vertical;
                turnInput = joystick.Horizontal; 
            }
        }

        void Update() {
            if (otherPlayer) {
                return;
            }
            if (sitting) {
                return;
            }
            GetInput();
            Turn();
        }

        void FixedUpdate() {
            if (otherPlayer) {
                return;
            }
            if (sitting) {
                return;
            }
            Run();
        }
        


        private bool sitting = false;
        private Transform sittingOn;
        private AllowSitDownScript ChairScript;

        public void ShowSitButton(Transform target) {
            if (otherPlayer) {
                sittingOn = target;
                ChairScript = sittingOn.GetComponent<AllowSitDownScript>();
                return;
            }
            if (!sitting) {
                print("zzzzz ui manager : " + uiManager);
                if (inPublicRoom == 1) {
                    uiManager.GetComponent<UIManagerPublicScript>().ToggleSittingButton(true);
                } else {
                    uiManager.GetComponent<UIManagerScript>().ToggleSittingButton(true);
                }
            }
            sittingOn = target;
            ChairScript = sittingOn.GetComponent<AllowSitDownScript>();
        }

        private void SitDown() {
            animator.SetTrigger("isSitting");
            PV.RPC("RPC_SitDown", RpcTarget.AllBuffered, PV.ViewID);
            sitting = true;
            ChairScript.SitCharacter(this);
            GetComponent<CharacterController>().enabled = false;
            if (inPublicRoom == 1) {
                uiManager.GetComponent<UIManagerPublicScript>().Sit();
            } else {
                uiManager.GetComponent<UIManagerScript>().Sit();
            }
            mainCam.GetComponent<PlayerFollow>().SetSitting(true);
        }

        [PunRPC]
        public void RPC_SitDown(int PVID) {
            if (!otherPlayer) {
                return;
            }
            GetComponent<CharacterController>().enabled = false;
            sitting = true;
            if (ChairScript) {
                ChairScript.OccupyChair(true);
            }
            animator.SetTrigger("isSitting");
        }

        [PunRPC]
        public void RPC_StandUp(int PVID) {
            if (!otherPlayer) {
                return;
            }
            GetComponent<CharacterController>().enabled = true;
            sitting = false;
            if (ChairScript) {
                ChairScript.OccupyChair(false);
            } 
            animator.SetTrigger("isStanding");
        }

        IEnumerator CheckSittingPosition() {
            yield return new WaitForSeconds(2);
            if (sitting) {
                animator.SetTrigger("isSitting");
            }
        }

        public void ToggleZoomViewSitDown(bool zoomed) {
            if (inPublicRoom == 1) {
                uiManager.GetComponent<UIManagerPublicScript>().ToggleZoomSitDown(zoomed);
            } else {
                uiManager.GetComponent<UIManagerScript>().ToggleZoomSitDown(zoomed);
            }
        }

        public void HideSitDownButton() {
            if (otherPlayer) {
                return;
            }
            if (inPublicRoom == 1) {
                uiManager.GetComponent<UIManagerPublicScript>().ToggleSittingButton(false);
            } else {
                uiManager.GetComponent<UIManagerScript>().ToggleSittingButton(false);
            }
        }

        public void StandUp() {
            sitting = false;
            transform.Translate(new Vector3(0, 0, -1), sittingOn);
            GetComponent<CharacterController>().enabled = true;
            animator.SetTrigger("isStanding");
            if (inPublicRoom == 1) {
                uiManager.GetComponent<UIManagerPublicScript>().StandUp();
            } else {
                uiManager.GetComponent<UIManagerScript>().StandUp();
            }
            mainCam.GetComponent<PlayerFollow>().SetInitialCameraState();
            PV.RPC("RPC_StandUp", RpcTarget.AllBuffered, PV.ViewID);
        }

        void Run() {
            Vector3 newPos = transform.position + (transform.forward * forwardInput * forwardVel * Time.deltaTime * 55);
            // transform.position = newPos; //Vector3.Lerp(transform.position, newPos, Time.deltaTime * 10f);
            characterController.SimpleMove(transform.forward * forwardInput * forwardVel * 40);
            animator.SetFloat("Speed", forwardInput);
        }

        void Turn() {
            targetRotation *= Quaternion.AngleAxis(rotateVel * turnInput * Time.deltaTime, Vector3.up);
            animator.SetFloat("TurnDirection", turnInput);
            transform.rotation = targetRotation;//Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            // if (transform.position.y > 0.3) {
            //     Vector3 pos = transform.position;
            //     pos.y -= 0.3f;
            //     transform.position = pos;
            // }
        }


          public static void RefreshInstance(ref CharacterScript player, CharacterScript prefab ) {
              Vector3 pos;
              Debug.Log("zzzz refreshed instance ");
              int publicWorld = PlayerPrefs.GetInt("isInPublicWorld");
              if (publicWorld == 1) {
                  string worldType = PlayerPrefs.GetString("currentPublicWorld");
                  if (worldType == "RacingWorld") {
                    pos = new Vector3(447.5852f, 0, 335.4253f);
                  } else if (worldType == "HolidayWorld")  {
                    pos = new Vector3(22, 1, 18); 
                  } else {
                    pos = new Vector3(2, 1, 4);
                  }
              } else {
                pos = new Vector3(2, 0, 2);
              }
            Quaternion rot = Quaternion.identity;
            if (player != null) {
                pos = player.transform.position;
                rot = player.transform.rotation;
                PhotonNetwork.Destroy(player.gameObject);
            }
            player = PhotonNetwork.Instantiate("Characters/" + prefab.gameObject.name, pos, rot).GetComponent<CharacterScript>();
        }
      


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(forwardInput);
                stream.SendNext(turnInput);
            } else if (stream.IsReading) {
                Vector3 pos = (Vector3) stream.ReceiveNext();
                Quaternion rot = (Quaternion) stream.ReceiveNext();
                float forwardInp = (float) stream.ReceiveNext();
                float turnInp = (float) stream.ReceiveNext();
                if (prevPos == null) {
                    transform.position = pos;
                    transform.rotation = rot;
                    prevPos = pos;
                    prevRot = rot;
                } else {
                    transform.position = Vector3.Lerp(prevPos, pos, Time.deltaTime * (float)lastUpdatetime);
                    transform.rotation = Quaternion.Lerp(prevRot, rot, Time.deltaTime * (float)lastUpdatetime);
                    lastUpdatetime = info.SentServerTime;
                }
                if (animator) {
                    animator.SetFloat("Speed", forwardInp);
                    animator.SetFloat("TurnDirection", turnInp);
                }
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info) {
            info.Sender.TagObject = transform.gameObject;
        }
        private void OnTriggerEnter(Collider other) {
            // -1 is not assigned ; 0 is no and ; 1 is in room
            if (otherPlayer) {
                return;
            }
            if (inRoomState == -1 && other.gameObject.name == "door") {
                mainCam.GetComponent<PlayerFollow>().ChangeCameraViewpoint(true);
                inRoomState = 1;
            } else if (inRoomState == 1 && other.gameObject.name == "door") {
                mainCam.GetComponent<PlayerFollow>().ChangeCameraViewpoint(false);
                inRoomState = -1;
            } else if (other.gameObject.name == "store") {
                // itemLoader.GetComponent<ItemLoaderStore>().ActivateStore(true);
                uiManager.GetComponent<UIManagerPublicScript>().ActivateStore(true);
            } else if (other.gameObject.name == "Wardrobe" && myRoom) {
                itemLoader.GetComponent<ItemLoader>().ToggleWardrobe(true);
            } else if (other.gameObject.GetComponent<CharacterScript>() != null && inPublicRoom == 1 && actiavteChat) {
                CharacterScript otherCharScript = other.gameObject.GetComponent<CharacterScript>();
                if (otherCharScript != this) {// ie is not me 
                    Photon.Realtime.Player CurrPlayer = other.transform.GetComponent<PhotonView>().Controller;
                    PV.RPC("RPC_SendPotentialConvo", CurrPlayer, username, PhotonNetwork.LocalPlayer.ActorNumber, publicWorldInterestGroup, currentlyTalkingTo, PV.ViewID);
                }
            } else if (other.gameObject.GetComponent<CharacterScript>() != null && inPublicRoom == 0) {
                CharacterScript otherPlayer = other.gameObject.GetComponent<CharacterScript>();
                if (otherPlayer.inGame) {
                    uiManager.GetComponent<UIManagerScript>().ShowJoinGame(otherPlayer.currentGameType, otherPlayer.username, otherPlayer.currentGameCode);
                }
            } else if (other.gameObject.name == "job") {
                int jobType = int.Parse(other.transform.GetChild(0).gameObject.name);
                uiManager.GetComponent<UIManagerPublicScript>().ToggleJobButton(true, jobType);
            }
        }


        public void ChangeSkin(Material newMat) {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = newMat;
            PV.RPC("RPC_SkinChange", RpcTarget.AllBuffered, newMat.name, PV.ViewID);
        }

        private void OnTriggerExit(Collider other) {
            if (otherPlayer) {
                return;
            }
            if (other.gameObject.name == "store") {
                // itemLoader.GetComponent<ItemLoaderStore>().ActivateStore(false);
                uiManager.GetComponent<UIManagerPublicScript>().ActivateStore(false);
            } else if (other.gameObject.name == "Wardrobe" && myRoom) {
                itemLoader.GetComponent<ItemLoader>().ToggleWardrobe(false);
            } else if (other.gameObject.GetComponent<CharacterScript>() != null && inPublicRoom == 1) {
                CharacterScript otherCharScript = other.gameObject.GetComponent<CharacterScript>();
                Debug.Log(other.transform.name);
                if (otherCharScript != this) {// ie is not me 
                    Photon.Realtime.Player CurrPlayer = other.transform.GetComponent<PhotonView>().Controller;
                    PV.RPC("RPC_DissappearConversation", CurrPlayer);
                
                }
            } else if (other.gameObject.GetComponent<CharacterScript>() != null && inPublicRoom == 0) {
                CharacterScript otherPlayer = other.gameObject.GetComponent<CharacterScript>();
                if (otherPlayer.inGame) {
                    uiManager.GetComponent<UIManagerScript>().HideJoinGame();
                }
            } else if (other.gameObject.name == "job") {
                uiManager.GetComponent<UIManagerPublicScript>().ToggleJobButton(false, 0);
            }
        }

        public void DestroyCamera() {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            Destroy(camera);
        }

        public void SetMainCamEditing(Transform placeableObject, bool isPlacingItem) {
            mainCam.GetComponent<PlayerFollow>().NowFollowing(placeableObject, isPlacingItem);
        }

        public void JoinedGame() {
            uiManager.GetComponent<UIManagerScript>().HideJoinGame();
        }


        public void SetGame(string name, string code) {
            currentGameCode = code;
            currentGameType = name;
            inGame = true;
            PV.RPC("RPC_SetCharacterGame", RpcTarget.AllBuffered, PV.ViewID, currentGameType, currentGameCode);
        }

        public void LeftGame() {
            currentGameCode = "";
            currentGameType = "";
            inGame = false;
            PV.RPC("RPC_CharacterLeftGame", RpcTarget.AllBuffered);
        }

        [PunRPC]
        void RPC_CharacterLeftGame() {
            currentGameCode = "";
            currentGameType = "";
            inGame = false;
        }


        [PunRPC]
        void RPC_SetCharacterGame(int pvID, string gameType, string gameCode) {
            currentGameCode = gameCode;
            currentGameType = gameType;
            inGame = true;
        }


        [PunRPC]
        void RPC_ChangeCharacterName(string name, int pvID) {
            // 0 = private; 1 = public
            if (photonView.IsMine) {
                return;
            }
            GameObject nameCanvas;
            nameCanvas = PhotonView.Find(pvID).transform.GetChild(2).gameObject;
            nameCanvas.SetActive(true);
            TMPro.TextMeshProUGUI playerName = nameCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            playerName.text = "@" + name;
            username = name;
        }

        [PunRPC]
        void RPC_ReceiveMessage(string message, int pvID) {
            // 0 = private; 1 = public
            if (inPublicRoom == 0) {
                return;
            }
            TMPro.TextMeshProUGUI chatCanvas = PhotonView.Find(pvID).transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            chatCanvas.text = message;
        }

        [PunRPC]
        void RPC_SkinChange(string skinName, int pvID) {
            Material material = Resources.Load<Material>("Characters/Materials/" + skinName) as Material;
            PhotonView.Find(pvID).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = material;
        }


        [PunRPC]
        void RPC_SetCharacterAccessories(string baseAcc, int pvID) {
            // List<GameObject> previousAccesories = new List<GameObject>();
            // friendsPreviousAccessories.Add(pvID, previousAccesories);
            if (!photonView.IsMine) {
                SetUpCharacter(baseAcc);
            }
            // SetCharacterAccessories(baseAcc, previousAccesories, PhotonView.Find(pvID).transform);
        }

        void SetUpCharacter(string acc) {
            CharacterCustomizationSetup setup = CharacterCustomizationSetup.DeserializeFromJson(acc);
            GetComponent<CharacterCustomization>().SetCharacterSetup(setup);
        }

        [PunRPC]
        public void RPC_UpdateCharacterAccessories(string newAccessories, int pvID) {
            if (!photonView.IsMine) {
                SetUpCharacter(newAccessories);
            }
            // List<GameObject> prevAcc = friendsPreviousAccessories[pvID];
            // UpdateCharacterAccessories(newAccessories, prevAcc, PhotonView.Find(pvID).transform);
        }

        [PunRPC]
        public void RPC_SendPotentialConvo(string username, int ActorNumber, int interestGroup, string currentlyTalkingTo, int PVID) {
            // when I collide with another player I send that other player the pop up "Want to chat" -> as the other player receives it, if I am already in convo 
            // I don't get the pop up -> but the popup still appears to me if I am not in conversation (i.e, popups are always sent but not always displayed)
            CharacterScript mainPlayerScript = (PhotonNetwork.LocalPlayer.TagObject as GameObject).GetComponent<CharacterScript>();
            int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            // remember that when I send an RPC to another player it gets passed to MY player in their instance so I tell my player to get their script to see if they're in convo
            if (!mainPlayerScript.isInConversation) {
                // i am not in conversation and therefore should be prompted invitations to other conversations to join
                if (interestGroup == -1) {
                    ChatManager.ShowRequestConversation(true, false, username, currentlyTalkingTo, ActorNumber, interestGroup, myActorNumber, PVID);
                } else {
                    ChatManager.ShowRequestConversation(true, true, username, currentlyTalkingTo, ActorNumber, interestGroup, myActorNumber, PVID);
                }
            }
        }

        public void SendConversationRequest(string tempGroup, int OtherPlayerPVID) {
            Photon.Realtime.Player CurrPlayer = PhotonView.Find(OtherPlayerPVID).Owner;
            PV.RPC("RPC_ConversationRequest", CurrPlayer, username, tempGroup, PV.ViewID);
        }


        public void CancelRequestSent(string tempGroup, int OtherPlayerPVID) {
            Photon.Realtime.Player CurrPlayer = PhotonView.Find(OtherPlayerPVID).Owner;
            PV.RPC("RPC_CancelRequestSent", CurrPlayer);
        }

        [PunRPC]
        public void RPC_CancelRequestSent() {
            ChatManager.RemovePotentialConvo();
        }

        [PunRPC]
        public void RPC_ConversationRequest(string username, string tempGroup, int PVID) {
            ChatManager.ShowConversationRequest(username, tempGroup, PVID);
        }

        [PunRPC]
        public void RPC_DissappearConversation() {
            ChatManager.DissapearConversation();
        }

        public void LeaveConversation() {
            // send to other players that I have left conversation
            isInConversation = false;
            currentlyTalkingTo = ""; 
            publicWorldInterestGroup = -1;
            PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = (byte) 0;
        }

        public void AcceptConversationRequest(string otherUsername, string group, int PVID, bool iSentIt) {
            if (isInConversation) {
                currentlyTalkingTo = currentlyTalkingTo + ";" + otherUsername;
            } else {
                currentlyTalkingTo = otherUsername;
            }
            isInConversation = true;
            publicWorldInterestGroup = int.Parse(group);
            PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = (byte) publicWorldInterestGroup;
            ChatManager.ShowCurrentConversation(currentlyTalkingTo);
            if (!iSentIt) {
                Photon.Realtime.Player CurrPlayer = PhotonView.Find(PVID).Owner;
                PV.RPC("RPC_AcceptConversation", CurrPlayer, username, group, PV.ViewID);
            }
        }

        public void RejectConversationRequest(string username, string group, int PVID) {
            Photon.Realtime.Player CurrPlayer = PhotonView.Find(PVID).Owner;
            PV.RPC("RPC_RejectConversation", CurrPlayer, username);
        }

        [PunRPC]
        public void RPC_AcceptConversation(string username, string group, int PVID) {
            CharacterScript mainPlayerScript = (PhotonNetwork.LocalPlayer.TagObject as GameObject).GetComponent<CharacterScript>();
            mainPlayerScript.AcceptConversationRequest(username, group, 3, true);
        }

        [PunRPC]
        public void RPC_RejectConversation(string username) {
            ChatManager.ShowRequestReject(username);
        }
    }
}
