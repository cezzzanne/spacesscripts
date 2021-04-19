using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraTour : MonoBehaviour {
    // Start is called before the first frame update

    public GameObject intro, editWorld, editCharacter, finalText;
    public GameObject character;

    private string characterSkin;
    bool firstTimeInCameraToWorld = true;
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

    private bool rotating = false;

    public Transform target;

    private bool rotationEnabled = true;

    private Vector3 initialPosition;

    void Start() {
        desiredDistance = 7;
        currentDistance = 7;
    }

     void Update() {
         if (rotating) {
             // on editor test
         if (Input.GetMouseButton(2)) {
             desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
         }
         if (Input.GetMouseButton(0)) {
           xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.05f;
             yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.05f;
             yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
             desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
             currentRotation = transform.rotation;
             rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
             transform.rotation = rotation;  
         }
         else if (Input.GetMouseButton(2)) {
             target.rotation = transform.rotation;
             target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
             target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
         }
         desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
         desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
         currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
         position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
         transform.position = position;


        // on unity editor test

                                            //// ACTIVATE CODE BELOW FOR IPHONE
            // if (Input.touchCount==2) {
            //     Touch touchZero = Input.GetTouch(0);

            //     Touch touchOne = Input.GetTouch(1);



            //     Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;

            //     Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;



            //     float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;

            //     float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;



            //     float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

            //     desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(desiredDistance);
            // }
            // if (Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            //     Vector2 touchposition = Input.GetTouch(0).deltaPosition;
            //     xDeg += touchposition.x * xSpeed * 0.002f;
            //     yDeg -= touchposition.y * ySpeed * 0.002f;
            //     yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // }
            // desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            // currentRotation = transform.rotation;
            // rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            // transform.rotation = rotation;
            // desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // position = target.position - (rotation * Vector3.forward * currentDistance );

            // position = position - targetOffset;

            // transform.position = position;
        }
     }

    private static float ClampAngle(float angle, float min, float max) {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
        
    public IEnumerator RotateCam() {
        Quaternion des = Quaternion.Euler(15, 0, 0);
        while (Quaternion.Angle(transform.rotation, des) > 2) {
            yield return new WaitForSeconds(0.01f);
            Quaternion to = Quaternion.Lerp(transform.rotation, des, Time.deltaTime);
            transform.rotation = to;
        }
        Debug.Log("done rotating cam");
        intro.SetActive(true);
        yield return new WaitForSeconds(10);
        intro.SetActive(false);
        editWorld.SetActive(true);
        yield return new WaitForSeconds(10);
        editWorld.SetActive(false);
        StartCoroutine(MoveCameraToCloset());
    }

    IEnumerator MoveCameraToCloset() {
        Vector3 des = new Vector3(24.8344f, 108.1046f, 24.33046f);
        while ((transform.position - des).magnitude > 0.2) {
            yield return new WaitForSeconds(0.01f);
            transform.position = Vector3.Lerp(transform.position, new Vector3(24.8344f, 108.1046f, 24.33046f), Time.deltaTime);
        }
        editCharacter.SetActive(true);
        yield return new WaitForSeconds(10);
        editCharacter.SetActive(false);
        firstTimeInCameraToWorld = false;
        StartCoroutine(MoveCameraToWorld(characterSkin)); // pass it again because I need to not because I want to
    }

    public IEnumerator MoveCameraToWorld(string characterSelected) {
        rotating = false;
        rotationEnabled = false;
        Vector3 des = new Vector3(16, 104, -3);
        while ((transform.position - des).magnitude > 0.2) {
            yield return new WaitForSeconds(0.004f);
            transform.position = Vector3.Lerp(transform.position, new Vector3(16, 104, -3), Time.deltaTime);
        }
        if (firstTimeInCameraToWorld) {
            StartCoroutine(RotateCam());
        } else {
            finalText.SetActive(true);
        }
    }

    public void ZoomInOnCharacter() {
        initialPosition = transform.position;
        Vector3 characterPos = target.position;
        characterPos.z -= 2.2f;
        characterPos.y += 1.9f;
        transform.position = characterPos;
    }

    public void ZoomOutOnCharacter() {
        transform.position = initialPosition;
    }

    public void ZoomInOnCharacterFace() {
        Vector3 characterPos = target.position;
        characterPos.z -= 1.2f;
        characterPos.y += 1.9f;
        transform.position = characterPos;
    }

    public void ZoomOutOfCharacterFace() {
        ZoomInOnCharacter();
    }

    public void ReadyPlayerOne() {
        Debug.Log("is finished and load scene");
        SceneManager.LoadScene("MainGame");
    }
}
