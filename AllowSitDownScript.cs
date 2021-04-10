using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class AllowSitDownScript : MonoBehaviour {

        private bool isOccupied = false;
        private void OnTriggerEnter(Collider other) {
            CharacterScript character = other.GetComponent<CharacterScript>() ;
            if (character != null && !isOccupied) {
                character.ShowSitButton(transform);
            }
        }

    
        private void OnTriggerExit(Collider other) {
            CharacterScript character = other.GetComponent<CharacterScript>() ;
            if (character != null) {
                character.HideSitDownButton();
            }
        }

        public void SitCharacter(CharacterScript character) {
                int amountOfColliders = transform.GetComponents<BoxCollider>().Length;
                Collider collider = transform.GetComponents<BoxCollider>()[0];
                Collider collider2 = transform.GetComponents<BoxCollider>()[amountOfColliders - 1];
                Vector3 pos = collider.transform.position;

                float length = collider.transform.localScale.x * ((BoxCollider)collider).size.x;
                float width = collider.transform.localScale.z * ((BoxCollider)collider).size.z;
                float height = collider.transform.localScale.y * ((BoxCollider)collider).size.y;
                Vector3 dimensions = new Vector3(length, height, width);

                float length2 = collider2.transform.localScale.x * ((BoxCollider)collider2).size.x;
                float width2 = collider2.transform.localScale.z * ((BoxCollider)collider2).size.z;
                float height2 = collider2.transform.localScale.y * ((BoxCollider)collider2).size.y;
                Vector3 dimensions2 = new Vector3(length2, height2, width2);

                pos.y += (dimensions2.y - 0.4f);
                character.transform.position = pos;
                character.transform.Translate(new Vector3(dimensions.x / 2, 0, 0), transform); 
                character.transform.rotation = transform.rotation*Quaternion.AngleAxis(180, Vector3.up);
        }

        public void OccupyChair(bool occupy) {
            isOccupied = occupy;
        }
    }
}
