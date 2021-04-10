using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class JobManagerScript : MonoBehaviour {

        private int currentJobType = -1;

        public DeliverJobManager deliveryJob;

        public ChoppingWoodManager choppingWood;


        public void SetTypeOfJob(int jobType) {
            currentJobType = jobType;
        }
 
        public void StartJob() {
            if (currentJobType == 0) {
                deliveryJob.StartJob();
            }
        }

        public void EndJob() {
            if (currentJobType == 0) {
                deliveryJob.CancelJob();
            }
        }
    }
}
