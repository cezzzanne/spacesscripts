using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class DeliveryCustomerScript : MonoBehaviour {
        private DeliverJobManager jobManager;
        
        void OnTriggerEnter(Collider other) {
            CharacterScript player = other.GetComponent<CharacterScript>();
            if (player != null) {
                if (!player.otherPlayer) {
                    jobManager.IncrementPackageCount(gameObject);
                }
            }
        }

        public void SetJobManager(DeliverJobManager djm) {
            jobManager = djm;
        }
    }
}
