using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

namespace Spaces {
    public class ItemPlacementControllerV2 : MonoBehaviourPun {
        // Start is called before the first frame update

        private GameObject currentPlaceableObject;


        private float currRotation = 0;

        Transform target;

        public GameObject terrain;

        public SaveSystem saveSystem;

        private string myRoomID;

        public GameObject uiManager;

        private UIManagerPublicScript uiManagerScript;

        private float forwardInput, rightInput = 0;

        private bool beingPressed, isForward, isReversed = false;

        public float speed = 0.3f;

        public GameObject SpeedIndicator;

        public float minApartmentY = 0.0f;

        private Vector3 pastPosition;

        

        void Start() {
            myRoomID = PlayerPrefs.GetString("myRoomID");
            uiManagerScript = uiManager.GetComponent<UIManagerPublicScript>();
        }

        public void SetTarget(Transform character) {
            target = character;
        }

        public void HandleNewObj(string item) {
            GameObject prefab;
            try {
                prefab = Resources.Load<GameObject>("TownPrefabs/" + item);
                prefab = Instantiate(prefab) as GameObject;
            } catch {
                prefab = Resources.Load<GameObject>("StoreItems/" + item);
                prefab = Instantiate(prefab) as GameObject;
            }
            HandleNewObjectHotKey(prefab);
        }


        void FixedUpdate() {
            if (currentPlaceableObject != null) {
                forwardInput = 0;
                rightInput = 0;
                if (beingPressed) {
                    if (isForward) {
                        forwardInput = speed;
                        if (isReversed) {
                            forwardInput = -speed;
                        }
                    } else {
                        rightInput = speed;
                        if (isReversed) {
                            rightInput = -speed;
                        }
                    }
                }
                MoveCurrentPlaceableObject();
            }
        }

        public void SetCameraDistance(int zoomIn) {
            target.GetComponent<CharacterScript>().mainCam.GetComponent<PlayerFollow>().SetCameraDistance(zoomIn);
        }

        public void SetSpeed() {
            if (speed == 0.3f) {
                speed = 1f;
                SpeedIndicator.transform.GetChild(0).gameObject.SetActive(false);
                SpeedIndicator.transform.GetChild(1).gameObject.SetActive(true);
            } else if (speed == 1f) {
                speed = 2f;
                SpeedIndicator.transform.GetChild(1).gameObject.SetActive(false);
                SpeedIndicator.transform.GetChild(2).gameObject.SetActive(true);
            } else {
                speed = 0.3f;
                SpeedIndicator.transform.GetChild(2).gameObject.SetActive(false);
                SpeedIndicator.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void ReleaseIfClicked() {
            currentPlaceableObject.GetComponent<Rigidbody>().isKinematic = true;
            currentPlaceableObject.GetComponent<Rigidbody>().freezeRotation = true;
            currentPlaceableObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            currentPlaceableObject.transform.SetParent(terrain.transform);
            float xPos = currentPlaceableObject.transform.position.x;
            float yPos = currentPlaceableObject.transform.position.y;
            float zPos= currentPlaceableObject.transform.position.z;
            float yRot = currentPlaceableObject.transform.eulerAngles.y;
            Dictionary<string, object> objectData = new Dictionary<string, object> {
                {"xPos", xPos},
                {"yPos", yPos},
                {"zPos", zPos},
                {"yRot", yRot},
                {"name", currentPlaceableObject.name}
            };
            target.gameObject.SetActive(true);
            target.GetComponent<CharacterScript>().SetMainCamEditing(target, false);
            currentPlaceableObject = null;
            uiManagerScript.PlacedItem();
            saveSystem.SaveSpace(objectData, target, int.Parse(myRoomID));
        }

        public void Rotate(int side) {
            Debug.Log("rotating");
            currentPlaceableObject.transform.rotation = Quaternion.Euler(currentPlaceableObject.transform.eulerAngles.x, currentPlaceableObject.transform.eulerAngles.y + side, currentPlaceableObject.transform.eulerAngles.z);
        }

        private void MoveCurrentPlaceableObject() {
            Collider[] colliders = Physics.OverlapBox(currentPlaceableObject.transform.position, currentPlaceableObject.transform.localScale / 2);
            bool contactWithCollider = false;
            if (colliders.Length > 0) {
                Collider collider = null;
                for (int i = 0; i < colliders.Length; i++) {
                    if (colliders[i].gameObject != currentPlaceableObject.gameObject && colliders[i].gameObject.name != "SitDown") {
                        contactWithCollider = true;
                        if (collider != null) {
                            Collider temp = colliders[i];
                            collider = (collider.bounds.center.y + collider.bounds.extents.y + collider.transform.position.y) >= (temp.bounds.center.y + temp.bounds.extents.y + temp.transform.position.y) ? collider : temp;
                        } else {
                            collider = colliders[i];
                        }
                    }
                }
              
                if (contactWithCollider && !collider.isTrigger) {
                    Vector3 pos = currentPlaceableObject.transform.position;
                    float length = collider.transform.localScale.x * ((BoxCollider)collider).size.x;
                    float width = collider.transform.localScale.z * ((BoxCollider)collider).size.z;
                    float height = collider.transform.localScale.y * ((BoxCollider)collider).size.y;
                    Vector3 dimensions = new Vector3(length, height, width);

                    //now to know the world position of top most level of the wall:
                    float topMost = collider.transform.position.y + dimensions.y ;
                    pos.y = topMost;
                    if (collider.gameObject.name.Substring(0, 4) == "wall") {
                        currentPlaceableObject.transform.position = pastPosition;
                    } else {
                        pastPosition = currentPlaceableObject.transform.position;
                        currentPlaceableObject.transform.position = pos;
                        currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.forward * 0.1f * forwardInput);
                        currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.right * 0.1f * rightInput);
                    }
                } else {
                    Vector3 pos = currentPlaceableObject.transform.position;
                    pos.y = minApartmentY; // added this;
                    currentPlaceableObject.transform.position = pos;
                    currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.forward * 0.1f * forwardInput);
                    currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.right * 0.1f * rightInput);
                    pastPosition = currentPlaceableObject.transform.position;
                }
            } else {
                Vector3 pos = currentPlaceableObject.transform.position;
                pos.y = minApartmentY;
                currentPlaceableObject.transform.position = pos;
                currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.forward * 0.1f * forwardInput);
                currentPlaceableObject.transform.position = currentPlaceableObject.transform.position + (currentPlaceableObject.transform.right * 0.1f * rightInput);
                pastPosition = pos;
            }
        }

        public void SetForwardInput(bool pressed, bool forward, bool reversed) {
            beingPressed = pressed;
            isForward = forward;
            isReversed = reversed;
        }

        public void HandleNewObjectHotKey(GameObject prefab) {
            // when a button presses I can hardcode what number each item is (find more elegant way) 
            if (currentPlaceableObject == null) {
                    currentPlaceableObject = prefab;
                    Rigidbody rBody = currentPlaceableObject.GetComponent<Rigidbody>();
                    rBody.useGravity = false;
                    rBody.isKinematic = true;
                    BoxCollider bCollider = currentPlaceableObject.GetComponent<BoxCollider>();
                    MeshRenderer meshRenderer = currentPlaceableObject.GetComponentInChildren<MeshRenderer>();
                    Transform tr = currentPlaceableObject.transform;
                    Vector3 tempPos = target.position;
                    minApartmentY = target.position.y;
                    pastPosition = target.position;
                    target.gameObject.SetActive(false);
                    target.GetComponent<CharacterScript>().SetMainCamEditing(currentPlaceableObject.transform, true); //  new
                    currentPlaceableObject.transform.position = tempPos; //+ (target.forward * ((maxWidthObject * 1.2f)  + characterWidth + 0.5f)); //target.position + (target.forward * ((maxWidthObject * 1.2f)  + characterWidth + 0.5f));

                } else {
            }
        }

        public void RemoveCurrentPlaceableObject() {
            Destroy(currentPlaceableObject);
            uiManagerScript.PlacedItem();
            target.gameObject.SetActive(true);
            target.GetComponent<CharacterScript>().SetMainCamEditing(target, false);
            // PlayerPrefs.DeleteKey("CurrentItem");
            // ShowListOfObjects();
        }
    }
}
