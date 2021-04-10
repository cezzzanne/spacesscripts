using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Database;

namespace Spaces {
    public class ApartmentScript : MonoBehaviour {

        // private int price;

        // private int floorNum;

        // private GameObject SellSign;

        // private GameObject BuyHomeModal;

        // private string userID, username;

        // private int userCoins = -1;

        // private HousingManagerScript HousingManagerScript;

        // private bool purchased = false;

        // private GameObject CongratsModal;

        // private UIManagerPublicScript UIScript;

        // private bool isUsers = false;

        // private bool active = true;

        // public void SetForSale(int aptPrice, int aptNum, GameObject sellSign, GameObject bhModal, string id, string usern, HousingManagerScript hms, GameObject congModal, UIManagerPublicScript uis) {
        //     userID = id;
        //     SellSign = sellSign;
        //     price = aptPrice;
        //     username = usern;
        //     floorNum = aptNum;
        //     BuyHomeModal = bhModal;
        //     HousingManagerScript = hms;
        //     CongratsModal = congModal;
        //     UIScript = uis;
        //     transform.GetComponents<BoxCollider>()[1].gameObject.SetActive(true);
        // }

        // public void SetToOtherOwner(UIManagerPublicScript uis, string ownerName) {
        //     active = false;
        //     username = ownerName;
        //     UIScript = uis;
        // }

        // public void SetAsUsers(UIManagerPublicScript uis) {
        //     UIScript = uis;
        //     isUsers = true;
        // }

        // void OnTriggerEnter(Collider other) {
        //     if (!active) {
        //         UIScript.ToggleOwnerAppDisplay(true, username);
        //         return;
        //     }
        //     if (other.gameObject.GetComponent<CharacterScript>() != null && !purchased && !isUsers) {
        //         SetBuyHomeModal();
        //         SellSign.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "floor " + floorNum.ToString() + " is for sale for $" + price.ToString() + "! click for more info";
        //         SellSign.SetActive(true);
        //         SellSign.GetComponent<Button>().onClick.RemoveAllListeners();
        //         SellSign.GetComponent<Button>().onClick.AddListener(()=> {BuyHomeModal.SetActive(true);});
        //     } else if (other.gameObject.GetComponent<CharacterScript>() != null && isUsers) {
        //         UIScript.ToggleEditMode(true);
        //     }
        // }

        // void OnTriggerExit(Collider other) {
        //     if (!active) {
        //         UIScript.ToggleOwnerAppDisplay(false, username);
        //         return;
        //     }
        //     if (other.gameObject.GetComponent<CharacterScript>() != null && !isUsers) {
        //         SellSign.SetActive(false);
        //     } else if (other.gameObject.GetComponent<CharacterScript>() != null && isUsers) {
        //         UIScript.ToggleEditMode(false);
        //     }
        // }

        // private void SetBuyHomeModal() {
        //     Transform mainPage = BuyHomeModal.transform.GetChild(0);
        //     mainPage.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Unit Floor " + floorNum.ToString();
        //     mainPage.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hey there! This floor costs $" + price.ToString() + " coins If you buy the unit, only you and your friends will have access and only you'll be able to buy furniture around the world and decorate your home however you like!";
        //     DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //     StartCoroutine(SetPurchaseButton());
        //     reference.Child("users").Child(username).Child("coins").GetValueAsync().ContinueWith(task => {
        //         DataSnapshot snapshot = task.Result;
        //         userCoins = int.Parse(snapshot.Value.ToString());
        //     });
        // }

        // private IEnumerator SetPurchaseButton() {
        //     while (userCoins == -1) {
        //         yield return null;
        //     }
        //     Transform mainPage = BuyHomeModal.transform.GetChild(0);
        //     if (userCoins >= price) {
        //         mainPage.GetChild(4).gameObject.SetActive(false);
        //         mainPage.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        //         mainPage.GetChild(2).GetComponent<Button>().onClick.AddListener(()=> {StartCoroutine(PurchaseHome());});
        //     } else {
        //         mainPage.GetChild(4).gameObject.SetActive(true);
        //     }
        // }

        // private IEnumerator PurchaseHome() {
        //     WWWForm form = new WWWForm();
        //     form.AddField("userID", userID);
        //     form.AddField("floor", floorNum);
        //     UnityWebRequest www = UnityWebRequest.Post("https://circles-parellano.herokuapp.com/api/buy-apartment", form); 
        //     yield return www.SendWebRequest();
        //     if(www.isNetworkError || www.isHttpError) {
        //         Debug.Log(www.error);
        //     } else {
        //         string response = www.downloadHandler.text;
        //         yield return response;
        //         DeductCoins();
        //         ShiftApartmentToUser();
        //         StartCoroutine(CongratulateUser());
        //     }
        // }

        // public void DeductCoins() {
        //     DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //     int totalCoins = userCoins - price;
        //     Dictionary<string, object> coinsData = new Dictionary<string, object>() {
        //         {"coins", totalCoins},
        //     };
        //     reference.Child("users").Child(username).UpdateChildrenAsync(coinsData);
        // }


        // private void ShiftApartmentToUser() {
        //     purchased = true;
        //     HousingManagerScript.ShiftApartmentOwnership(floorNum);
        //     isUsers = true;
        // }

        // private IEnumerator CongratulateUser() {
        //     BuyHomeModal.SetActive(false);
        //     SellSign.SetActive(false);
        //     CongratsModal.SetActive(true);
        //     yield return new WaitForSeconds(6f);
        //     CongratsModal.SetActive(false);
        // }
    }
}
