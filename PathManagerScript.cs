using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;


namespace Spaces {
    public class PathManagerScript : MonoBehaviour {

        public PathCreator pathCreator;


        public float speed = 2f;

        float distanceTravelled;

        public Transform transport;

        private bool advance = true;

        void Start() {
            transport.GetComponent<CarScript>().SetPathManager(this);
        }


        void FixedUpdate() {
            if (pathCreator != null && advance) {
                distanceTravelled += speed * (Time.deltaTime);
                transport.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
                transport.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
            }
        }

        public void ToggleRoadIsClear(bool isClear) {
            advance = isClear;
        }
    }
}
