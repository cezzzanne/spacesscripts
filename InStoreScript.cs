using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class InStoreScript : MonoBehaviour {
        // Start is called before the first frame update

        public GameObject ItemLoaderStore;

        public int storeType;


        void OnTriggerEnter(Collider other) {
            CharacterScript character = other.gameObject.GetComponent<CharacterScript>();
            if (character != null) {
                if (!character.otherPlayer) {
                    ItemLoaderStore.GetComponent<ItemLoaderStore>().SetStoreIndex(storeType);
                }
            }
        }
    }
}
