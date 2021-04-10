using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Spaces {
    public class DeleteTrashCanScript : MonoBehaviour {


        List<GameObject> currentItems;

        string userID;

     
        void Start() {
            currentItems = new List<GameObject>();
            userID = PlayerPrefs.GetString("myRoomID");
        }

        void OnCollisionEnter(Collision collision) {
            if (collision.transform.parent == null) {
                return;
            }
            GameObject colliderParent = GetColliderParent(collision.gameObject);
            if (colliderParent.name == "ModifiedTerrain(Clone)") {
                GameObject item = GetItem(collision.gameObject);
                currentItems.Add(item);
                MeshRenderer meshRenderer;
                Material[] materials;
                Debug.Log("curr item nam e: " + item.name);
                try {
                    meshRenderer = item.GetComponentInChildren<MeshRenderer>();
                    materials = meshRenderer.materials;
                }
                catch {
                    meshRenderer = item.transform.GetChild(0).gameObject.GetComponentInChildren<MeshRenderer>();
                    materials = meshRenderer.materials;
                }
                foreach(Material m in materials) {
                    m.shader = Shader.Find("Unlit/Transparent Cutout");
                }
            }
        }

        GameObject GetColliderParent(GameObject collidingWith) {
            Transform parent = collidingWith.transform.parent;
            while (parent.parent != null) {
                parent = parent.parent;
            }
            return parent.gameObject;
        }

        GameObject GetItem(GameObject item) {
            Transform currItem = item.transform;
            while (currItem.parent.gameObject.name != "ModifiedTerrain(Clone)") {
                currItem = currItem.parent;
            }
            return currItem.gameObject;  
        }

        void OnCollisionExit(Collision collision) {
            if (collision.transform.parent == null) {
                return;
            }
            GameObject colliderParent = GetColliderParent(collision.gameObject);
            if (colliderParent.name == "ModifiedTerrain(Clone)") {
                GameObject item = GetItem(collision.gameObject);
                currentItems.Remove(item);
                MeshRenderer meshRenderer;
                Material[] materials;
                Debug.Log("curr item nam e: " + item.name);
                try {
                    meshRenderer = item.GetComponentInChildren<MeshRenderer>();
                    materials = meshRenderer.materials;
                }
                catch {
                    meshRenderer = item.transform.GetChild(0).gameObject.GetComponentInChildren<MeshRenderer>();
                    materials = meshRenderer.materials;
                }
                foreach(Material m in meshRenderer.materials) {
                    m.shader = Shader.Find("Standard (Specular setup)");
                }
            }
        }


        public void DeleteItem() {
            if (currentItems.Count < 1) {
                return;
                // nothing here;
            }
            foreach(GameObject item in currentItems) {
                RemoveIndividualItem(item);
            }
            currentItems = new List<GameObject>();
        }

        public void CancelEditing() {
            if (currentItems.Count < 1) {
                return;
            }
            foreach(GameObject item in currentItems) {
                MeshRenderer meshRenderer = item.GetComponentInChildren<MeshRenderer>();
                foreach(Material m in meshRenderer.materials) {
                    m.shader = Shader.Find("Standard (Specular setup)");
                }
            }
            currentItems = new List<GameObject>();
        }

        public void RemoveIndividualItem(GameObject currentItem) {
            currentItem.SetActive(false);
            float xPos = currentItem.transform.position.x;
            float yPos = currentItem.transform.position.y;
            float zPos= currentItem.transform.position.z;
            float yRot = currentItem.transform.eulerAngles.y;
            Dictionary<string, object> objectData = new Dictionary<string, object> {
                {"xPos", xPos},
                {"yPos", yPos},
                {"zPos", zPos},
                {"name", currentItem.name}
            };
            StartCoroutine(RemoveItemFromDB(objectData));
        }


        IEnumerator RemoveItemFromDB(Dictionary<string, object> data) {
            WWWForm form = new WWWForm();
            string world = JsonUtility.ToJson(data);
            form.AddField("userID", userID.ToString());
            form.AddField("name", data["name"] as string);
            form.AddField("xPos", data["xPos"].ToString());
            form.AddField("yPos", data["yPos"].ToString());
            form.AddField("zPos", data["zPos"].ToString());
            UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/remove-item-from-world", form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string response = www.downloadHandler.text;
                Debug.Log("zzzz response" + response);
                yield return response;
            }
        }
    }
}
