using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Spaces {
    public class FollowPathScript : MonoBehaviour {
        public PathCreator pathCreator;

        public float speed = 2f;

        float distanceTravelled;

        private bool isDoneWithPath = true;

        private int currentPathIndex;

        private Animator animator;

        private NPCManagerScript NPCManager;

        private string username;

        void Start() {
            animator = GetComponent<Animator>();
        }

        public void SetPathCreator(PathCreator pc, int i, NPCManagerScript NPCM, string un) {
            pathCreator = pc;
            distanceTravelled = 0;
            transform.position = pc.transform.position;
            currentPathIndex = i;
            speed = Random.Range(0.9f, 2.5f);
            NPCManager = NPCM;
            isDoneWithPath = false;
            username = un;
        }


        void FixedUpdate() {
            if (!isDoneWithPath && (pathCreator != null)) {
                animator.SetFloat("Speed", speed / 2.5f);
                distanceTravelled += speed * (Time.deltaTime * 1f);
                Vector3 newPos = pathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop) - transform.position;
                newPos.y = 0;
                transform.GetComponent<CharacterController>().Move(newPos);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
                if (Mathf.Abs(pathCreator.path.length - distanceTravelled) < 0.5f) {
                    animator.SetFloat("Speed", 0);
                    StartCoroutine(DoneWithPath());
                }
            }
        }

        private IEnumerator DoneWithPath() {
            isDoneWithPath = true;
            float waitFor =  Random.Range(9,20);
            pathCreator = null;
            yield return new WaitForSeconds(waitFor);
            NPCManager.FinishPath(currentPathIndex, this, username);
        }
    }
}
