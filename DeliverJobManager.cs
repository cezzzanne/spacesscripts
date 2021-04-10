using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class DeliverJobManager : MonoBehaviour {

        public UIManagerPublicScript uIManager;

        private int currentPackageCount = 6;

        private int currentPackagesdelivered = 0;

        private int currentLevel = 0;

        public List<Material> availableSkins;

        public List<Vector3> possibleCustomerLocations = new List<Vector3>() {
            new Vector3(-23.21033f, 1.5f, 24.28153f), new Vector3(-30.60223f, 1.5f, -1.766478f), new Vector3(-31.8154f, 1.5f, 52.06707f), new Vector3(-50.48364f, 4.409973f, 13.00843f), new Vector3(-43.72168f, 4.409973f, 23.54873f), new Vector3(-55.92999f, 4.409973f, 19.22245f), new Vector3(-13.54788f, 1.950012f, 45.49654f), new Vector3(7.891022f, 1.950012f, 60.64288f), new Vector3(14.7157f, 1.950012f, 64.70414f), new Vector3(15.37292f, 1.950012f, 51.02825f), new Vector3(20.01855f, 1.950012f, 67.80496f), new Vector3(40.112f, 1.844971f, 67.43806f), new Vector3(30.18872f, 1.844971f, 59.3516f), new Vector3(19.24576f, 1.950012f, 46.32035f), new Vector3(27.00253f, 1.844971f, 28.32986f), new Vector3(35.52414f, 1.969788f, 32.14121f), new Vector3(45.5961f, 1.969788f, 29.96579f), new Vector3(44.54504f, 1.969788f, 37.2179f), new Vector3(29.70129f, 1.969788f, 41.78841f), new Vector3(40.06818f, 1.969788f, 46.42926f), new Vector3(37.13339f, 1.950012f, -5.384221f), new Vector3(30.45157f, 1.950012f, 4.898506f), new Vector3(43.12473f, 1.950012f, 5.06196f), new Vector3(31.237f, 1.950012f, -3.860554f), new Vector3(43.42017f, 1.950012f, -8.823696f), new Vector3(35.96411f, 1.950012f, 10.8456f), new Vector3(45.90393f, 1.950012f, 11.02993f), new Vector3(32.9978f, 1.950012f, 11.41329f), new Vector3(27.22638f, 1.950012f, 8.358541f), new Vector3(28.74487f, 1.950012f, -18.07608f), new Vector3(42.16568f, 1.950012f, -29.79678f), new Vector3(19.57623f, 1.950012f, -6.515203f), new Vector3(10.01904f, 2.400024f, -12.30892f), new Vector3(14.57965f, 2.400024f, -13.10899f), new Vector3(15.99329f, 2.400024f, -18.75686f), new Vector3(0.1257629f, 2.400024f, -18.55389f), new Vector3(6.222839f, 2.400024f, -18.33148f), new Vector3(0.4845276f, 2.400024f, -12.77241f), new Vector3(0.6449585f, 1.950012f, -25.06831f), new Vector3(16.35675f, 1.950012f, -27.20019f), new Vector3(6.093689f, 1.950012f, -30.19028f), new Vector3(-3.768707f, 1.950012f, -12.29535f), new Vector3(-10.62534f, 1.971619f, -24.69591f), new Vector3(-9.64563f, 1.971619f, -11.57103f), new Vector3(-21.57526f, 1.971619f, -10.73258f), new Vector3(-23.87686f, 1.971619f, -15.62428f), new Vector3(-17.2106f, 1.971619f, -30.354f), new Vector3(-24.01093f, 1.971619f, -25.54774f), new Vector3(-24.15631f, 1.950012f, -6.973829f), new Vector3(-17.42529f, 1.679993f, -2.531259f), new Vector3(-30.01178f, 1.5f, -11.63428f), new Vector3(-31.49936f, 1.5f, -28.73779f)
        };

        public List<Vector3> locationsInUse;

        public GameObject CustomerPrefab;


        void Start() {
            // TODO: get last time you did delivery 
            // if less than 12 hours show uiManager.WaitForForJob(timeLeft)
            int childCount = transform.childCount;
            string allPos = "";
            for(int i = 0; i < childCount; i++) {
                Transform child = transform.GetChild(i);
                string thisPos = "new Vector3(" + child.position.x.ToString() + "f, " + child.transform.position.y.ToString() + "f, " + child.transform.position.z.ToString() + "f), ";
                allPos = allPos + thisPos;
            }
            print(allPos);
        }

        public void StartJob() {
            uIManager.ShowDeliverJobLevel(0);
        }

        public void InitiateJob() {
            // if level is 0 - set in player prefs the time
            currentPackageCount = (currentLevel == 0) ? 6 : 7;
            currentPackagesdelivered = 0;
            locationsInUse = new List<Vector3>();
            for(int i = 0; i < currentPackageCount; i++) {
                GameObject customer = Instantiate(CustomerPrefab);
                Vector3 pos = GetRandomPosition();
                customer.transform.position = pos;
                customer.transform.rotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
                customer.GetComponent<DeliveryCustomerScript>().SetJobManager(this);
            }
            // uIManager.SetInDeliveryJob()
            // DropPackages();
        }

        Vector3 GetRandomPosition() {
            int randInt = Mathf.FloorToInt(Random.Range(0, possibleCustomerLocations.Count - locationsInUse.Count));
            Vector3 tempLoc = possibleCustomerLocations[randInt];
            while (locationsInUse.Contains(tempLoc)) {
                randInt = Mathf.FloorToInt(Random.Range(0, possibleCustomerLocations.Count - locationsInUse.Count));
                tempLoc = possibleCustomerLocations[randInt];        
            }
            return tempLoc;
        }

        public void CancelJob() {
            uIManager.CloseJob();
        }

        public void IncrementPackageCount() {
            currentPackagesdelivered++;
            if (currentPackageCount == currentPackagesdelivered) {
                currentLevel++;
                if (currentLevel < 3) {
                    uIManager.ShowDeliverJobLevel(currentLevel);
                } else {
                    // uiManager.JobFinished(0);
                }
            }
        }        
    }
}
