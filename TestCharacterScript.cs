using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using Photon.Voice.PUN;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

namespace Spaces {
    public class TestCharacterScript : MonoBehaviourPun{
        Joystick joystick;
        Animator animator;
        public float inputDelay = 0.1f;
        public float forwardVel = 0.09f;
        public float rotateVel = 60f;
        Quaternion targetRotation;
        Rigidbody rBody;
        float forwardInput, turnInput = 0;

        public bool otherPlayer = false;

        CharacterController characterController;

        GameObject mainCam;

        string otherPlayerUsername;

        Vector3 prevPos;
        Quaternion prevRot;

        string username;

        private bool readyToAddOthers = false;

        public List<string> otherPlayers = new List<string>();

        public Quaternion TargetRotation() {
            return transform.rotation;
        }
        
        void Start() {
            if (!otherPlayer) {
                username = PlayerPrefs.GetString("username");
                mainCam = Resources.Load("Main Camera") as GameObject;
                mainCam = Instantiate(mainCam);
                PlayerFollow cameraScript = mainCam.GetComponent<PlayerFollow>();
                cameraScript.SetCameraTarget(transform, 0);
            }
            animator = GetComponent<Animator>();
            joystick = FindObjectOfType<Joystick>();
            targetRotation = transform.rotation;
            //rBody = GetComponent<Rigidbody>();
            forwardInput = turnInput = 0;
            characterController = transform.GetComponent<CharacterController>();
        }


        public void LogToFirebase() {
            Dictionary<string, object> values = new Dictionary<string, object>
            {
                    { "forwardInput", forwardInput.ToString()},
                    { "position", transform.position.ToString()},
                    { "rotation", transform.rotation.ToString()},
                    { "turnInput", turnInput.ToString()}
            };
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("inWorld").Child(username).UpdateChildrenAsync(values);
        }

        void GetInput() {
            if (otherPlayer) {
                // do something with photon
            } else {
                if (joystick == null) {
                    return;
                }
                forwardInput =  joystick.Vertical;
                turnInput = joystick.Horizontal; 
            }
        }

        void Update() {
            if (otherPlayer) {
                return;
            }
            GetInput();
            Turn();
        }

        void FixedUpdate() {
            if (otherPlayer) {
                return;
            }
            Run();
        }

        void LateUpdate() {
            if (!otherPlayer) {
                LogToFirebase();
            }
        }


        void Run() {
            Vector3 newPos = transform.position + (transform.forward * forwardInput * forwardVel * 0.05f);
            // transform.position = newPos; //Vector3.Lerp(transform.position, newPos, Time.deltaTime * 10f);
            characterController.Move(transform.forward * forwardInput * forwardVel);
            animator.SetFloat("Speed", forwardInput);
        }

        void Turn() {
            targetRotation *= Quaternion.AngleAxis(rotateVel * turnInput * Time.deltaTime, Vector3.up);
            animator.SetFloat("TurnDirection", turnInput);
            transform.rotation = targetRotation;//Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            if (transform.position.y > 0.3) {
                Vector3 pos = transform.position;
                pos.y -= 0.3f;
                transform.position = pos;
            }
        }

    }
}
