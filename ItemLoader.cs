using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Spaces {
    public class ItemLoader : MonoBehaviour {
        // Start is called before the first frame update
        PlayerFollow mainCam;

        List<GameObject> prefabList;
        List<string> itemNames;

        private Transform ObjectHolder;

        private int currIndex = 0;

        public GameObject nextItemButton;
        public GameObject confirmItemButton;
        private bool inEditing = false;
        public GameObject itemController;

        public GameObject uiManager;

        private UIManagerScript uiManagerScript;

        public GameObject EditWorldButton;

        private List<StoreItem> storeDataObjects;
        private List<StoreItem> storeDataSkins;
        private bool browsingPurchased = false;

        private int purchasedItemsIndex = 0;


        private GameObject currentItem;

        public GameObject CharacterChange;

        private CharacterChange CharacterChangeScript;

        public GameObject itemName;
        public GameObject PhotoManager;

        private Dictionary<string, List<GameObject>> items;

        private string currentBrowsingType = "Furniture";

        public GameObject NoItemsBanner, NextItemB, PrevItemB;

        public GameObject FurnitureBackdrop, ElectronicsBackdrop, PlantsBackdrop, ExtraBackdrop;

        private Dictionary<string, GameObject> ItemButtonBackdrops;
        List<GameObject> fullItemList;
        
        public GameObject ItemRemover;

        public MaleCustomizerScript CharacterCustomizer;

        void Start() {
            fullItemList = new List<GameObject>();
            ObjectHolder = transform.GetChild(0);
            uiManagerScript = uiManager.GetComponent<UIManagerScript>();
            CharacterChangeScript = CharacterChange.GetComponent<CharacterChange>();
            string roomID = PlayerPrefs.GetString("myRoomID");
            items = new Dictionary<string, List<GameObject>>() {
                {"Furniture", new List<GameObject>() {}},
                {"Electronics", new List<GameObject>(){}},
                {"Plants", new List<GameObject>() {}},
                {"Extra", new List<GameObject>() {}}
            };
            ItemButtonBackdrops = new Dictionary<string, GameObject>() {
                {"Furniture", FurnitureBackdrop},
                {"Electronics", ElectronicsBackdrop},
                {"Plants", PlantsBackdrop},
                {"Extra", ExtraBackdrop}
            };
            if (PlayerPrefs.GetString("currentRoomID") == roomID) {
                StartCoroutine(LoadPurchasedItems(roomID));
            }
        }

     
        public IEnumerator LoadPurchasedItems(string roomID) {
            if (storeDataObjects == null) {
                WWWForm form = new WWWForm();
                form.AddField("userID", roomID);
                form.AddField("storeType", 0);
                UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-purchased-items", form);
                yield return www.SendWebRequest();
                if(www.isNetworkError || www.isHttpError) {
                    Debug.Log(www.error);
                }
                else {
                    string response = www.downloadHandler.text;
                    yield return response;
                    storeDataObjects = new List<StoreItem>();
                    storeDataSkins = new List<StoreItem>();
                    StoreResponse fullData = JsonUtility.FromJson<StoreResponse>(response);
                    foreach(StoreItem item in fullData.data) {
                        if (item.type == "skin") {
                            // CharacterChangeScript.AddToCharacterObjects(item.type == "skin", item);
                            CharacterCustomizer.AddAvailableItems(item);
                        } else if (item.type == "object") {
                            GameObject currentAsset = Resources.Load<GameObject>("StoreItems/" + item.location) as GameObject;
                            fullItemList.Add(currentAsset);
                        }
                    }
                    GameObject[] prevItems = Resources.LoadAll<GameObject>("TownPrefabs") as GameObject[];
                    foreach(GameObject prevItem in prevItems) {
                        fullItemList.Add(prevItem);
                    }
                    uiManagerScript.ActivateEditing();
                }
            }
        }


    public void LoadItem() {
        StoreItem item = storeDataObjects[purchasedItemsIndex];
        GameObject currentAsset = Resources.Load<GameObject>("StoreItems/" + item.location) as GameObject;
        GameObject instPrefab = Instantiate(currentAsset);
        instPrefab.transform.SetParent(ObjectHolder);
        instPrefab.transform.localPosition = new Vector3(0, 0, 0);
        float width = instPrefab.GetComponent<BoxCollider>().size.x * Screen.width/ Screen.height; // basically height * screen aspect ratio
        // instPrefab.transform.localScale = Vector3.one * width / 4f;
        // instPrefab.transform.localScale = instPrefab.transform.localScale * (1f / instPrefab.GetComponent<BoxCollider>().size.x);
        instPrefab.transform.Rotate(new Vector3(-20, 0, 0), Space.Self);
        instPrefab.SetActive(true);
        currentItem = instPrefab;
        itemName.GetComponent<TMPro.TextMeshProUGUI>().text = storeDataObjects[purchasedItemsIndex].name;
        FitCamera();
    }

    public void GoToItemSelection() {
            if (mainCam == null) {
                return;
            }
            mainCam.ToggleItemLoader();
            if (prefabList == null) {
                LoadItems(fullItemList);
            } else {
                if (!inEditing) {
                    FitCamera();
                }
            }
            inEditing = !inEditing;
            uiManagerScript.ToggleEditing();
        }

        public void SetCamera(PlayerFollow cam) {
            mainCam = cam;
            PhotoManager.GetComponent<PhotoManagerScript>().SetMainCam(cam);
            ItemRemover.GetComponent<ItemRemover>().SetMainCam(cam);
        }

        public void ToggleWardrobe(bool open) {
            uiManagerScript.ToggleWardrobe(open);
        }

        public void ToggleCharacterChange() {
            if (mainCam == null) {
                return;
            }
            mainCam.ToggleCharacterChange();
            inEditing = !inEditing;
            uiManagerScript.ToggleCharacterChange();
        }

        public void ConfirmCharacterChange() {
            CharacterChangeScript.ConfirmCharacterChanges();
            ToggleCharacterChange();
        }


        void LoadItems(List<GameObject> assetsList) {
            // prefabList = new List<GameObject>();
            // itemNames = new List<string>();
            int maxObjects = assetsList.Count;
            for(int i = 0; i < maxObjects; i++) {
                GameObject currentAsset = assetsList[i] as GameObject;
                GameObject instPrefab = Instantiate(currentAsset);
                string nameWithoutClone = instPrefab.name.Substring(0, instPrefab.name.Length - 7);
                string type = nameWithoutClone.Split(';')[1];
                string name = nameWithoutClone.Split(';')[0];
                int x = i;
                items[type].Add(instPrefab);
                // prefabList.Insert(x, instPrefab);
                // itemNames.Insert(x, instPrefab.name.Substring(0, instPrefab.name.Length - 7));
                instPrefab.SetActive(false);
                instPrefab.transform.SetParent(ObjectHolder);
                instPrefab.transform.localPosition = new Vector3(0, 0, 0);
                float width = instPrefab.GetComponent<BoxCollider>().size.x * Screen.width/ Screen.height; // basically height * screen aspect ratio
                instPrefab.transform.localScale = Vector3.one * width / 4f;
                instPrefab.transform.localScale = instPrefab.transform.localScale * (1f / instPrefab.GetComponent<BoxCollider>().size.x);
                instPrefab.transform.Rotate(new Vector3(-20, 0, 0), Space.Self);
            }
            // prefabList[currIndex].SetActive(true);
            // currentItem = prefabList[currIndex];
            items[currentBrowsingType][currIndex].SetActive(true);
            ItemButtonBackdrops[currentBrowsingType].SetActive(true);
            currentItem = items[currentBrowsingType][currIndex];
            string fullName = currentItem.name;
            fullName = fullName.Substring(0, fullName.Length - 7);
            itemName.GetComponent<TMPro.TextMeshProUGUI>().text = fullName.Split(';')[0].Replace("-", " ");
            FitCamera();
        }

        void FitCamera() {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Bounds itemBounds = currentItem.GetComponent<BoxCollider>().bounds;
            float cameraDistance = browsingPurchased ?  6.0f : 5f; // Constant factor
            Vector3 objectSizes = itemBounds.max - itemBounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCam.gameObject.GetComponent<Camera>().fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
            if (browsingPurchased) {
                mainCam.gameObject.transform.position = currentItem.transform.position - (distance * 0.5f) * mainCam.transform.forward;
            } else {
                mainCam.gameObject.transform.position = itemBounds.center - distance * mainCam.transform.forward;
            }
        }

        public void NextItem() {
            int temp = currIndex + 1;
            if (temp >  (items[currentBrowsingType].Count - 1)) {
                temp = 0;
            }
            items[currentBrowsingType][currIndex].SetActive(false);
            currIndex = temp;
            items[currentBrowsingType][currIndex].SetActive(true);
            currentItem = items[currentBrowsingType][currIndex];
            string fullName = currentItem.name;
            fullName = fullName.Substring(0, fullName.Length - 7);
            itemName.GetComponent<TMPro.TextMeshProUGUI>().text = fullName.Split(';')[0].Replace("-", " ");
            FitCamera();
        }

        public void SetBrowsingType(string type) {
            if (items[currentBrowsingType].Count > 0) {
                items[currentBrowsingType][currIndex].SetActive(false); // set preivous item to not active
            }
            ItemButtonBackdrops[currentBrowsingType].SetActive(false); // remove this ui piece to main ui manager
            currentBrowsingType = type;
            ItemButtonBackdrops[currentBrowsingType].SetActive(true);
            currIndex = 0;
            if (items[currentBrowsingType].Count == 0) {
                NoItemsBanner.SetActive(true);
                NextItemB.SetActive(false);
                PrevItemB.SetActive(false);
                itemName.transform.parent.gameObject.SetActive(false);
                return;
            } else {
                NoItemsBanner.SetActive(false);
                NextItemB.SetActive(true);
                PrevItemB.SetActive(true);
                itemName.transform.parent.gameObject.SetActive(true);
            }
            items[currentBrowsingType][currIndex].SetActive(true); // activate current item
            currentItem = items[currentBrowsingType][currIndex];
            string fullName = currentItem.name;
            fullName = fullName.Substring(0, fullName.Length - 7);
            itemName.GetComponent<TMPro.TextMeshProUGUI>().text = fullName.Split(';')[0].Replace("-", " ");
            FitCamera();

        }

        public void PrevItem() {
            int temp = currIndex - 1;
            if (temp < 0) {
                temp = items[currentBrowsingType].Count - 1;
            }
            items[currentBrowsingType][currIndex].SetActive(false);
            currIndex = temp;
            items[currentBrowsingType][currIndex].SetActive(true);
            currentItem = items[currentBrowsingType][currIndex];
            string fullName = currentItem.name;
            fullName = fullName.Substring(0, fullName.Length - 7);
            itemName.GetComponent<TMPro.TextMeshProUGUI>().text = fullName.Split(';')[0].Replace("-", " ");
            FitCamera();
        }

        public void ConfirmItem() {
            inEditing = false;
            mainCam.ToggleItemLoader();
            uiManagerScript.IsPlacingItem();
            string itemName = items[currentBrowsingType][currIndex].name;
            itemName = itemName.Substring(0, itemName.Length - 7);
            itemController.GetComponent<ItemPlacementController>().HandleNewObj(itemName);
        }

        public void CancelEditing() {
            inEditing = false;
            NoItemsBanner.SetActive(false);
            uiManagerScript.ToggleEditing();
            mainCam.ToggleItemLoader();
        }

        public GameObject ReturnUIManager() {
            return uiManager;
        }

        // temporal function

        public void SitDownToggleCamera() {
            mainCam.GetComponent<PlayerFollow>().SitCameraToogle();
        }

    }
}
