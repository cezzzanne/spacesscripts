using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class JobManagerScript : MonoBehaviour {

        private int currentJobType = -1;


        public void SetTypeOfJob(int jobType) {
            currentJobType = jobType;
        }
    }
}
