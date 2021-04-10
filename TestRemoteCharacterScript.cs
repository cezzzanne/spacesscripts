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
    public class TestRemoteCharacterScript : MonoBehaviourPun{
        Joystick joystick;
        Animator animator;
        public float inputDelay = 0.1f;
        public float forwardVel = 0.01f;
        public float rotateVel = 0.01f;
        Quaternion targetRotation;
        Rigidbody rBody;
        float forwardInput, turnInput = 0;

        public bool otherPlayer = false;

        CharacterController characterController;

        GameObject mainCam;


        Vector3 prevPos;
        
        Quaternion prevRot;

        string username;


        public void StartCharacter(string charUsername) {
            animator = GetComponent<Animator>();
            targetRotation = transform.rotation;
            forwardInput = turnInput = 0;
            characterController = transform.GetComponent<CharacterController>();
            username = charUsername;
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("inWorld").Child(username).ValueChanged += HandleValueChanged;
        }

        void HandleValueChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            DataSnapshot snapshot = args.Snapshot;
            if (snapshot.Value != null) {
                Dictionary<string, object> data = snapshot.Value as Dictionary<string, object>;
                Vector3 pos = StringToVector3(data["position"] as string);
                Quaternion rot = StringToQuaternion(data["rotation"] as string);
                float forwardInp = float.Parse(data["forwardInput"] as string);
                float turnInp = float.Parse(data["forwardInput"] as string);
                Vector3 newPos = pos - transform.position;
                characterController.Move(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 5);
                if (animator) {
                    animator.SetFloat("Speed", forwardInp);
                    animator.SetFloat("TurnDirection", turnInp);
                }
            }
        }

        public static Vector3 StringToVector3(string sVector) {
            if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
                sVector = sVector.Substring(1, sVector.Length-2);
            }
            string[] sArray = sVector.Split(',');
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));
            return result;
        }

        public static Quaternion StringToQuaternion(string sQuat) {
            if (sQuat.StartsWith ("(") && sQuat.EndsWith (")")) {
                sQuat = sQuat.Substring(1, sQuat.Length-2);
            }
            string[] qarray = sQuat.Split(',');
            Quaternion result = new Quaternion(
                float.Parse(qarray[0]),
                float.Parse(qarray[1]),
                float.Parse(qarray[2]),
                float.Parse(qarray[3]));
    
            return result;
        }

        void OnDestroy() {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("inWorld").Child(username).ValueChanged -= HandleValueChanged;
        }

        void OnApplicationQuit() {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("inWorld").Child(username).ValueChanged -= HandleValueChanged;
        }
    }
}
