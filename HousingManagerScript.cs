using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Spaces {
    
    
    [System.Serializable]
    public  struct Apartment {
        public bool occupied;

        public int price;

        public string owner;

        public int floor_number;

        public bool is_mine;

        public bool is_friends;
    }


    [System.Serializable]
    public  struct ApartmentBuilding {
        public List<Apartment> apartments;
        
    }

    [System.Serializable]
    public  struct ApartmentsItems {
        public ApartmentItems[] data;
    }

    [System.Serializable]
    public  struct ApartmentItems {
        public ItemInApartment[] items;
    }


    [System.Serializable]
    public  struct ItemInApartment {
        public string name;

        public float x_pos, y_pos, z_pos, rotation_y;
    }

    public class HousingManagerScript : MonoBehaviour {

        private List<Apartment> apartments;

        private string roomID, username;

        public GameObject SellSign, BuyHomeModal, CongratsModal, UIManager;

        public GameObject modifiedTerrain;

        void Start() {
            StartCoroutine(GetApartmentsInfo());
        }

        public IEnumerator GetApartmentsInfo() {
            string apartmentsToPopulate = "";
            UIManagerPublicScript UIScript = UIManager.GetComponent<UIManagerPublicScript>();
            WWWForm form = new WWWForm();
            roomID = PlayerPrefs.GetString("myRoomID");
            username = PlayerPrefs.GetString("username");
            form.AddField("userID", roomID);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-apartments", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string response = www.downloadHandler.text;
                yield return response;
                ApartmentBuilding building = JsonUtility.FromJson<ApartmentBuilding>(response);
                apartments = building.apartments;     
                foreach(Apartment ap in apartments) {
                    GameObject door = transform.GetChild(ap.floor_number - 1).GetChild(0).GetChild(0).gameObject;
                    GameObject floor = transform.GetChild(ap.floor_number - 1).GetChild(1).gameObject;
                    // open the door if it's mine, a friends or if I can buy it (not occupied)
                    if (!ap.is_mine && !ap.is_friends && ap.occupied) {
                        door.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        door.GetComponent<DoorOpen>().DisableDoorOpen();
                        door.GetComponent<BoxCollider>().isTrigger = false;
                        floor.GetComponent<ApartmentScript>().SetToOtherOwner(UIScript, ap.owner);
                    }
                    if (!ap.occupied) {
                        floor.GetComponent<ApartmentScript>().SetForSale(ap.price, ap.floor_number, SellSign, BuyHomeModal, roomID, username, this, CongratsModal, UIScript);
                    } else if (ap.occupied && ap.is_mine) {
                        floor.GetComponent<ApartmentScript>().SetAsUsers(UIScript);
                    }
                    if (ap.is_friends || ap.is_mine) {
                        if (apartmentsToPopulate == "") {
                            apartmentsToPopulate = ap.floor_number.ToString();
                        } else {
                            apartmentsToPopulate = apartmentsToPopulate + "," + ap.floor_number.ToString();
                        }
                    }
                }
                if (apartmentsToPopulate.Split(',').Length > 0) {
                    StartCoroutine(LoadApartmentItems(apartmentsToPopulate));
                }
            }
        }

        IEnumerator LoadApartmentItems(string floorNums) {
            WWWForm form = new WWWForm();
            form.AddField("floorNums", floorNums);
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/get-apartment-items", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string response = www.downloadHandler.text;
                yield return response;
                ApartmentsItems building = JsonUtility.FromJson<ApartmentsItems>(response);
                foreach(ApartmentItems ap in building.data) {
                    foreach(ItemInApartment item in ap.items) {
                        string name = item.name.Substring(0, item.name.Length - 7);
                        GameObject prefab;
                        GameObject currentItem;
                        try {
                            prefab = Resources.Load<GameObject>("TownPrefabs/" + name);
                            currentItem = Instantiate(prefab) as GameObject;
                        } catch {
                            prefab = Resources.Load<GameObject>("StoreItems/" + name);
                            currentItem = Instantiate(prefab) as GameObject;
                        }
                        currentItem.transform.position = new Vector3(item.x_pos, item.y_pos, item.z_pos);
                        currentItem.transform.Rotate(currentItem.transform.rotation.x, item.rotation_y, currentItem.transform.rotation.z);
                        currentItem.transform.SetParent(modifiedTerrain.transform);
                    }
                }
            }
        }

        public void ShiftApartmentOwnership(int floor) {
            GameObject door = transform.GetChild(floor - 1).GetChild(0).GetChild(0).gameObject;
            door.GetComponent<BoxCollider>().isTrigger = true;
        } 

    }
}
