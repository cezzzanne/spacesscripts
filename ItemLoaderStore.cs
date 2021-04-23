using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

namespace Spaces {


   [System.Serializable]
    public  struct StoreResponse {
        public string success;
        public List<StoreItem> data;
        public int coins;
    }

    [System.Serializable]
    public  struct StoreItem {
        public string name, type, location;
        public string price;
        
    }


    [System.Serializable]
    public  struct PurchaseResponse {
        public string success;        
    }



    public class ItemLoaderStore : MonoBehaviour {
        // Start is called before the first frame update
        PlayerFollow mainCam;

        List<GameObject> prefabList;
        List<string> itemNames;

        private Transform ObjectHolder;

        private int currIndex = 0;

        private bool inEditing = false;

        public GameObject BuyItemB, ReturnB, NextItemB, PrevItemB, StoreB;

        public GameObject GoHomeB, StartChatB, joystick, ScreenshotB;
        public GameObject objectTitle;

        // private StoreResponse AllStoreData;

        private List<StoreResponse> AllStoreData;

        public GameObject characterModel;
        public GameObject currentItem;

        public GameObject price;


        private int currentObjectType; // 0 is skin; 1 is object ; 2 is world

        public GameObject coins;

        public GameObject LoadingPurchase;

        string roomID;

        string username;

        public int coinsValue = -1;

        public PhotoManagerPublicScript PhotoManager;

        public GameObject JoinConvoB;

        public GameObject currentChatBubble;

        private bool chatBubbleIsActive = false;
        public GameObject SendRequest, RequestSent, AcceptRequest, RequestRejected;

        public GameObject uiManager;

        public int storeType;

        public GameObject SitDownB;

        private int currentStoreIndex = 0;

        public GameObject ItemLoaderStoreTerrain;

        public List<Sprite> menShoes;

        public List<Sprite> menShirts, menAccessories, menPants, menCaps;

        public List<Sprite> femaleShirts, femaleAccessories, femalePants, femaleCaps, femaleShoes;

        private List<List<Sprite>> menClothes, womenClothes;

        private List<List<Sprite>> clothingItems;

        public GameObject ClothingUI, ObjectUI;

        public TMPro.TextMeshProUGUI clothingName, clothingPrice;

        public Image ClothingImage;

        bool isMale;
        // index;type (0=accessory, 1=hat, 2=pants, 3=shirt, 4=shoes);gender(0=female,1=male)
        
        void Start() {
            AllStoreData = new List<StoreResponse>();
            ObjectHolder = transform.GetChild(0);
            roomID = PlayerPrefs.GetString("myRoomID");
            StartCoroutine(GetItems());
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://spaces-d9a3c.firebaseio.com/");
            womenClothes = new List<List<Sprite>>() {
                femaleAccessories, femaleCaps, femalePants, femaleShirts, femaleShoes
            };
            menClothes = new List<List<Sprite>>() {
                menAccessories, menCaps, menPants, menShirts, menShoes
            };
            isMale = PlayerPrefs.GetInt("isMale") == 1;
            clothingItems = isMale ? menClothes : womenClothes;
        }

        public GameObject ReturnUIManager() {
            return uiManager;
        }

        StoreResponse RemoveOtherGenderClothes(StoreResponse store) {
            // this is absolutely horrible - TODO: move the logic out to the backend
            StoreResponse tempResp = new StoreResponse();
            tempResp.data = new List<StoreItem>();
            foreach(StoreItem item in store.data) {
                if (item.type == "skin") {
                    if (item.location.Split(';')[2] == "1" && isMale) {
                       tempResp.data.Add(item);
                    } else if (item.location.Split(';')[2] == "0" && !isMale) {
                        tempResp.data.Add(item);
                    }
                } else {
                    tempResp.data.Add(item);
                }
            }
            return tempResp;
        }


        IEnumerator GetItems() {
            WWWForm form = new WWWForm();
            form.AddField("userID", roomID);
            form.AddField("storeType", 0);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-store", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string response = www.downloadHandler.text;
                yield return response;
                AllStoreData.Add(RemoveOtherGenderClothes(JsonUtility.FromJson<StoreResponse>(response)));
                StartCoroutine(GetItems2());
                GetCoins();
            }
        }

        IEnumerator GetItems2() {
            WWWForm form = new WWWForm();
            form.AddField("userID", roomID);
            form.AddField("storeType", 1);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-store", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string response = www.downloadHandler.text;
                yield return response;
                AllStoreData.Add(RemoveOtherGenderClothes(JsonUtility.FromJson<StoreResponse>(response)));
                StartCoroutine(GetItems3());
            }
        }

        IEnumerator GetItems3() {
            WWWForm form = new WWWForm();
            form.AddField("userID", roomID);
            form.AddField("storeType", 2);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-store", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string response = www.downloadHandler.text;
                yield return response;
                AllStoreData.Add(RemoveOtherGenderClothes(JsonUtility.FromJson<StoreResponse>(response)));
            }
        }

        public void SetStoreIndex(int index) {
            currentStoreIndex = index;
        }

        void GetCoins() {
            StartCoroutine(UpdateCoinsUI());
            username = PlayerPrefs.GetString("username");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("users").Child(username).Child("coins").GetValueAsync().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists) {
                    Debug.Log("zz error no coins");
                    return;
                }
                coinsValue = Convert.ToInt32(snapshot.Value);
            });
        }

        IEnumerator UpdateCoinsUI() {
            while(coinsValue == -1) {
                yield return null;
            }
            UpdateCoinsText(coinsValue);
        }

        public void UpdateCoinsText(int val) {
            coinsValue = val;
            coins.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "$" + val;
        }


        public void GoToItemSelection() {
            if (mainCam == null) {
                return;
            }
            mainCam.ToggleItemLoader();
            if (prefabList == null) {
                LoadItem();
            } else {
                if (!inEditing) {
                    Debug.Log("111: fitting camera");
                    FitCamera();
                }
            }
            inEditing = !inEditing;
            ToggleUI();
        }

        void ToggleUI() {
            if (inEditing && (int.Parse(AllStoreData[currentStoreIndex].data[currIndex].price) > coinsValue)) {
                BuyItemB.SetActive(false);
            } else {
                BuyItemB.SetActive(inEditing);
            }
            SitDownB.SetActive(false);
            ReturnB.SetActive(inEditing);
            NextItemB.SetActive(inEditing);
            PrevItemB.SetActive(inEditing);
            objectTitle.SetActive(inEditing);
            price.SetActive(inEditing);
            GoHomeB.SetActive(!inEditing);
            if (inEditing) {
                chatBubbleIsActive = currentChatBubble.activeSelf;
                RequestSent.SetActive(false);
                RequestRejected.SetActive(false);
                AcceptRequest.SetActive(false);
                SendRequest.SetActive(false);
                currentChatBubble.SetActive(false);
            } else {
                currentChatBubble.SetActive(chatBubbleIsActive);
            }
            ScreenshotB.SetActive(!inEditing);
            joystick.SetActive(!inEditing);
            StoreB.SetActive(!inEditing);
        }


        public void SetMainCam(PlayerFollow cam) {
            mainCam = cam;
            PhotoManager.SetMainCam(cam);
        }


        void FitCamera() {
            if (currentObjectType == 0) {
                mainCam.transform.position = new Vector3(15, 52, 13);
                mainCam.transform.rotation = Quaternion.Euler(0, 0, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                Bounds itemBounds = currentItem.GetComponent<BoxCollider>().bounds; //prefabList[currIndex].GetComponent<BoxCollider>().bounds;
                float cameraDistance = (currentObjectType == 1) ? 5.0f : 1f; // Constant factor
                Vector3 objectSizes = itemBounds.max - itemBounds.min;
                float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
                float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCam.gameObject.GetComponent<Camera>().fieldOfView); // Visible height 1 meter in front
                float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
                distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
                mainCam.gameObject.transform.position = currentItem.transform.position - distance * mainCam.transform.forward; //itemBounds.center - distance * mainCam.transform.forward;
            }
            StoreItem temp = AllStoreData[currentStoreIndex].data[currIndex];
            objectTitle.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (temp.type == "accessory") ? temp.name.Split('-')[1] : temp.name;
            price.transform.GetChild(0).GetComponent<Text>().text = "$" + temp.price;
        }

        public void NextItem() {
            int newIndex = currIndex + 1;
            if (newIndex >  AllStoreData[currentStoreIndex].data.Count - 1) {
                newIndex = 0;
            }
            currIndex = newIndex;
            characterModel.transform.parent.gameObject.SetActive(false);
            if (currentObjectType == 0) {

            } else {
                Destroy(currentItem);
            }
            LoadItem();
        }

        public void LoadItem() {
            StoreItem item = AllStoreData[currentStoreIndex].data[currIndex];
            if (int.Parse(item.price) > coinsValue) {
                BuyItemB.SetActive(false);
            } else {
                BuyItemB.SetActive(true);
            }
            if (item.type == "skin") {
                ToggleClothingUI(true);
                string[] itemSpec = item.location.Split(';');
                Sprite itemImage = clothingItems[int.Parse(itemSpec[1])][int.Parse(itemSpec[0])];
                ClothingImage.sprite = itemImage;
                clothingPrice.text = "$" + item.price;
                clothingName.text = item.name;
                currentObjectType = 0;
            } else if (item.type == "object" || item.type == "accessory") {
                ToggleClothingUI(false);
                GameObject currentAsset = Resources.Load<GameObject>("StoreItems/" + item.location) as GameObject;
                Vector3 prevScale = currentAsset.transform.localScale;
                GameObject instPrefab = Instantiate(currentAsset);
                instPrefab.transform.SetParent(ObjectHolder);
                instPrefab.transform.localPosition = new Vector3(0, 0, 0);
                float width = instPrefab.GetComponent<BoxCollider>().size.x * Screen.width/ Screen.height; // basically height * screen aspect ratio
                instPrefab.transform.localScale = prevScale;
                // instPrefab.transform.localScale = Vector3.one * width / 4f;
                // instPrefab.transform.localScale = instPrefab.transform.localScale * (1f / instPrefab.GetComponent<BoxCollider>().size.x);
                instPrefab.transform.Rotate(new Vector3(-20, 0, 0), Space.Self);
                instPrefab.SetActive(true);
                currentItem = instPrefab;
                currentObjectType = 1;
            } else {
                ToggleClothingUI(false);
                GameObject currentAsset = Resources.Load<GameObject>("Worlds/" + item.location) as GameObject;
                GameObject instPrefab = Instantiate(currentAsset);
                instPrefab.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                instPrefab.transform.SetParent(ObjectHolder);
                instPrefab.transform.localPosition = new Vector3(-0.8f, 1.5f, -3);
                float width = instPrefab.GetComponent<BoxCollider>().size.x * Screen.width/ Screen.height; // basically height * screen aspect ratio
                instPrefab.transform.Rotate(new Vector3(-20, 0, 0), Space.Self);
                instPrefab.SetActive(true);
                currentItem = instPrefab;
                currentObjectType = 2;
            }
            FitCamera();
        }

        void ToggleClothingUI(bool open) {
            ClothingUI.SetActive(open);
            ObjectUI.SetActive(!open);
        }

        public void PrevItem() {
            int newIndex = currIndex - 1;
            if (newIndex < 0) {
                newIndex = AllStoreData[currentStoreIndex].data.Count - 1;
            }
            currIndex = newIndex;
            characterModel.transform.parent.gameObject.SetActive(false);
            if (currentObjectType == 0) {

            } else {
                Destroy(currentItem);
            }
            LoadItem();
        }

        public void ConfirmItem() {
            LoadingPurchase.SetActive(true);
            if (currentObjectType == 2) {
                StartCoroutine(PurchaseWorld(AllStoreData[currentStoreIndex].data[currIndex]));
            } else {
                StartCoroutine(PurchaseItem(AllStoreData[currentStoreIndex].data[currIndex]));
            }
        }

        public IEnumerator PurchaseItem(StoreItem currentItem) {
            WWWForm form = new WWWForm();
            form.AddField("userID", roomID);
            form.AddField("name", currentItem.name);
            form.AddField("location", currentItem.location);
            form.AddField("price", currentItem.price.ToString());
            form.AddField("type", currentItem.type);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/purchase-item", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string response = www.downloadHandler.text;
                yield return response;
                PurchaseResponse resp = JsonUtility.FromJson<PurchaseResponse>(response);
                if (resp.success == "true") {
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "congratulations! purchase completed!";
                    yield return new WaitForSeconds(2);
                    coinsValue -= int.Parse(currentItem.price);
                    coins.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "$" + coinsValue.ToString();
                    UpdateFirebaseCoins(coinsValue);
                    AllStoreData[currentStoreIndex].data.RemoveAt(currIndex);
                    currIndex -= 1;
                    NextItem();
                    LoadingPurchase.SetActive(false);
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "completing purchase...";
                } else {
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "oops! something went wrong. try again";
                    yield return new WaitForSeconds(4);
                    LoadingPurchase.SetActive(false);
                    
                }
            }
        }
        public IEnumerator PurchaseWorld(StoreItem currentItem) {
            WWWForm form = new WWWForm();
            form.AddField("userID", roomID);
            string worldName = currentItem.location.Split('-')[0];
            form.AddField("name", currentItem.name);
            form.AddField("location", currentItem.location);
            form.AddField("price", currentItem.price.ToString());
            form.AddField("type", currentItem.type);
            form.AddField("worldType", worldName);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/purchase-world", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                string response = www.downloadHandler.text;
                yield return response;
                PurchaseResponse resp = JsonUtility.FromJson<PurchaseResponse>(response);
                if (resp.success == "true") {
                    PlayerPrefs.SetString("currentRoomID", roomID);
                    PlayerPrefs.SetString("currentWorldType", worldName);
                    PlayerPrefs.SetString("myWorldType", worldName);
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "congratulations! purchase completed!";
                    yield return new WaitForSeconds(4);
                    coinsValue -= int.Parse(currentItem.price);
                    coins.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "$" + coinsValue.ToString();
                    UpdateFirebaseCoins(coinsValue);
                    AllStoreData[currentStoreIndex].data.RemoveAt(currIndex);
                    NextItem();
                    LoadingPurchase.SetActive(false);
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "completing purchase...";
                } else {
                    LoadingPurchase.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "oops! something went wrong. try again";
                    yield return new WaitForSeconds(4);
                    LoadingPurchase.SetActive(false);
                    
                }
            }
        }

        public void UpdateFirebaseCoins(int coinsValue) {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                    { "coins", coinsValue.ToString() },
            };
            reference.Child("users").Child(username).UpdateChildrenAsync(user);
        }

        public void CancelEditing() {
            inEditing = false;
            if (currentObjectType == 1 || currentObjectType == 2 ){
                Destroy(currentItem);
            }
            ToggleClothingUI(false);
            ToggleUI();
            mainCam.ToggleItemLoader();
        }

        // provisional function

        public void DisplayJoinConversationB(bool open, string username) {
            JoinConvoB.GetComponent<TMPro.TextMeshProUGUI>().text = "@" + username + " wants to chat!";
            JoinConvoB.SetActive(true);
        }

        public void SitDownCamToggle() {
            mainCam.SitCameraToogle();
        }

    }
}

