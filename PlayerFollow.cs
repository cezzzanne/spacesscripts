using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaces {
    public class PlayerFollow : MonoBehaviour {
        Transform target;
        public float lookSmooth = 0.09f;
        private Vector3 offsetFromTarget = new Vector3(0, 2f, -3f);
        public float xTilt = 10;

        Vector3 destination = Vector3.zero;

        CharacterScript characterController;
        public Transform[] targetList;
        
        float rotateVel = 0;

        private CharacterScript controller;

        private bool selectingItem = false;

        private Touch touch;

        private bool rotating = false;

        float eulerX;

        private bool isPlacingItem = false;

        private int cameraDistance = 4;

        private Vector3 prevPos;

        // new code

     public Vector3 targetOffset;
     public float distance = 5.0f;
     public float maxDistance = 20;
     public float minDistance = .6f;
     public float xSpeed = 150.0f;
     public float ySpeed = 150.0f;
     public int yMinLimit = -80;
     public int yMaxLimit = 80;
     public int zoomRate = 40;
     public float panSpeed = 0.5f;
     public float zoomDampening = 5.0f;
     private float xDeg = 0.0f;
     private float yDeg = 0.0f;
     private float currentDistance;
     private float desiredDistance;
     private Quaternion currentRotation;
     private Quaternion desiredRotation;
     private Quaternion rotation;
     private Vector3 position;
     // new new code
    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    private bool sitCameraZommedIn = false;
    private bool sitting = false;
    Joystick joystick;
    // new new code
  
    float maxYEulers;
    float minYEulers;
    float maxXEulers;
    float minXEulers;
    float initialFacingX;
    float initialFacingY;
    private bool initialMove = false;
   
    float pitch = 0.0f;
    float yaw = 0.0f;
     void Update() {
         if (rotating) {
             // on editor test
        //  if (Input.GetMouseButton(2)) {
        //      desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        //  }
        //  if (Input.GetMouseButton(0)) {
        //    xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.05f;
        //      yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.05f;
        //      yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        //      desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        //      currentRotation = transform.rotation;
        //      rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        //      transform.rotation = rotation;  
        //  }
        //  else if (Input.GetMouseButton(2)) {
        //      target.rotation = transform.rotation;
        //      target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
        //      target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        //  }
        //  desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //  desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        //  currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        //  position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        //  transform.position = position;


        // on unity editor test

                                            //// ACTIVATE CODE BELOW FOR IPHONE
            if (Input.touchCount==2) {
                Touch touchZero = Input.GetTouch(0);

                Touch touchOne = Input.GetTouch(1);



                Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;

                Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;



                float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;

                float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;



                float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

                desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
            }
            if (Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
                Vector2 touchposition = Input.GetTouch(0).deltaPosition;
                xDeg += touchposition.x * xSpeed * 0.002f;
                yDeg -= touchposition.y * ySpeed * 0.002f;
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            }
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            position = target.position - (rotation * Vector3.forward * currentDistance );

            position = position - targetOffset;

            transform.position = position;
        }
     }
        private static float ClampAngle(float angle, float min, float max) {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
        

        public void SetCameraTarget(Transform t, int inPublicRoom) {
            target = t;
            offsetFromTarget = new Vector3(0, 2f, -3f); //(inPublicRoom == 1) ? new Vector3(0, 4.8f, -6.5f): new Vector3(0, 2f, -3f);
            transform.LookAt(target);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Rotate(new Vector3(xTilt, 0, 0), Space.Self);
            eulerX = transform.eulerAngles.x;
            if (target != null) {
                if (target.GetComponent<CharacterScript>() != null) {
                    characterController = target.GetComponent<CharacterScript>();
                } else {
                    Debug.Log("yyy No Character Script added to Character");
                }
            } else {
                Debug.Log("yyy No target to camera");
            }
        }

        public GameObject GetCharacter() {
            return target.gameObject;
        }

        public void ToggleRotateCamera(bool goingToRotate) {
            if (!rotating){
                distance = Vector3.Distance(transform.position, target.position);
                currentDistance = distance;
                desiredDistance = distance;
                position = transform.position;
                rotation = transform.rotation;
                currentRotation = transform.rotation;
                desiredRotation = transform.rotation;
                xDeg = Vector3.Angle(Vector3.right, transform.right);
                yDeg = Vector3.Angle(Vector3.up, transform.up);
            }
            rotating = goingToRotate;
        }

        private void LateUpdate() {
           if (sitCameraZommedIn) {
               pitch += joystick.Vertical * 55 * Time.deltaTime;
               yaw += joystick.Horizontal * 55 * Time.deltaTime;
               pitch = Mathf.Clamp(pitch, minXEulers, maxXEulers);
               yaw = Mathf.Clamp(yaw, minYEulers, maxYEulers);
               if (initialMove) {
                   pitch = initialFacingX;
                   yaw = initialFacingY;
                   initialMove = false;
               }
                transform.eulerAngles = new Vector3(-pitch, yaw, 0.0f);
               return;
           }
           if (target && !selectingItem && !rotating) {
               if (isPlacingItem) {
                   FitCamera();
                    LookAtTarget();
               } else {
                MoveToTarget();
                LookAtTarget();
               }
           }
        }
        public void ToggleItemLoader() {
            selectingItem = !selectingItem;
            if (selectingItem) {
                transform.position = new Vector3(15, 132, 13); // changed this
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(new Vector3(xTilt, 0, 0), Space.Self);
            }
        }

        public void ToggleItemSelect() {
            selectingItem = !selectingItem;
            if (selectingItem) {
                transform.position = new Vector3(15, 52, 13);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(new Vector3(xTilt, 0, 0), Space.Self);
            }
        }

        public void ToggleCharacterChange() {
            selectingItem = !selectingItem;
            if (selectingItem) {
                transform.position = new Vector3(23.7f, 91, 19);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(new Vector3(xTilt, 0, 0), Space.Self);
            }
        }
        
        void MoveToTarget() {
            destination = target.rotation * offsetFromTarget; // characterController.TargetRotation() * offsetFromTarget;
            destination += target.position;
            transform.position = destination;
        }
         public void ChangeCameraViewpoint(bool insideRoom) {
            // if (insideRoom) {
            //     offsetFromTarget = new Vector3(0, 3.2f, -3.5f);
            // } else {
            //     offsetFromTarget = new Vector3(0, 4.8f, -6.5f);
            // }
        }

        public void ZoomInPlayer() {
            offsetFromTarget = new Vector3(0, 1.7f, 0);
        }

        public void ZoomOutPlayer() {
            offsetFromTarget = new Vector3(0, 2f, -3f);
        }

        void LookAtTarget() {
            float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref rotateVel, lookSmooth);
            transform.rotation = Quaternion.Euler(eulerX, eulerYAngle, 0);
        }

        public void NowFollowing(Transform toFollow, bool placingItem) {
            ToggleRotateCamera(placingItem);
            target = toFollow;
            isPlacingItem = placingItem;
        }

        public void SetCameraDistance(int zoomIn) {
            cameraDistance += zoomIn;
        }

        void FitCamera() {
            Bounds itemBounds = target.GetComponent<BoxCollider>().bounds;
            Vector3 objectSizes = itemBounds.max - itemBounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * transform.GetComponent<Camera>().fieldOfView); // Visible height 1 meter in front
            // float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            // distance += 0.1f * objectSize; // Estimated offset from the center to the outside of the object
            transform.position = itemBounds.center - cameraDistance * transform.forward;
        }


        public void SitCameraToogle() {
            target.GetComponent<CharacterScript>().ToggleZoomViewSitDown(!sitCameraZommedIn);
            if (sitCameraZommedIn) {
                offsetFromTarget = new Vector3(0, 2f, -3f);
                initialMove = false;
            } else {
                offsetFromTarget = new Vector3(0, 1.7f, 0);
                if (joystick == null) {
                    joystick = FindObjectOfType<Joystick>();
                }
                MoveToTarget();
                LookAtTarget();
                initialMove = true;
                initialFacingX = transform.rotation.eulerAngles.x;
                initialFacingY = transform.rotation.eulerAngles.y;
                maxYEulers = transform.rotation.eulerAngles.y + 90;
                minYEulers = transform.rotation.eulerAngles.y - 90;
                minXEulers = transform.rotation.eulerAngles.x - 55;
                maxXEulers = transform.rotation.eulerAngles.x + 20;
            }
            sitCameraZommedIn = !sitCameraZommedIn;
        }

        public void SetInitialCameraState() {
            if (sitCameraZommedIn) {
                offsetFromTarget = new Vector3(0, 2f, -3f);
            }
        }

        public void SetSitting(bool sit) {
            sitting = sit;
            if (sit) {
                MoveToTarget();
                LookAtTarget();
            }
        }

        public void StartElevator() {
            ZoomInPlayer();
        }

        public void StopElevator() {
            ZoomOutPlayer();
        }

    }
}
