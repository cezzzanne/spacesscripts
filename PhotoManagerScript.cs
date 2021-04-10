using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.UIKit;
using SA.iOS.Photos;
using SA.iOS.Social;

namespace Spaces {
    public class PhotoManagerScript : MonoBehaviour {
        private PlayerFollow mainCam;
        public GameObject UIManager;
        private int hasAccessedScreenshot = -1;

        private Texture2D currentScreenShot;

        public void SetMainCam(PlayerFollow cam) {
            mainCam = cam;
        }
        public void SetUpScreenshot() {
            if (hasAccessedScreenshot == -1) {
                hasAccessedScreenshot = PlayerPrefs.GetInt("FirstScreenshot", 0);
            }
            if (hasAccessedScreenshot == 0) {
                UIManager.GetComponent<UIManagerScript>().SetUpPanel("Welcome to your Spaces Camera Roll", "Move around the camera with your fingers to find the perfect shot to take of your avatar! Your photo will be saved to your camera roll and you will be gicen the option to share on other platforms!", "Go!");
                PlayerPrefs.SetInt("FirstScreenshot", 1);
                hasAccessedScreenshot = 1;
            }
            UIManager.GetComponent<UIManagerScript>().TakeScreenshot(true);
            mainCam.ToggleRotateCamera(true);
        }

        public void CancelScreenshot() {
            UIManager.GetComponent<UIManagerScript>().TakeScreenshot(false);
            mainCam.ToggleRotateCamera(false);
        }

        public void TakeScreenshot() {
            ISN_PHPhotoLibrary.RequestAuthorization((status) => {
                if(status == ISN_PHAuthorizationStatus.Authorized) {
                    UIManager.GetComponent<UIManagerScript>().ToggleCaputreScreenshot(true);
                    ISN_UIImagePickerController.SaveScreenshotToCameraRoll((result) => {
                    if (result.IsSucceeded) {
                        StartCoroutine(ShareableScreenshot());
                    } else {
                        UIManager.GetComponent<UIManagerScript>().CaputreScreenshotResult(false);
                    }
                    });
                } else {
                    UIManager.GetComponent<UIManagerScript>().CaputreScreenshotResult(false);
                }
            });
        }

        public IEnumerator ShareableScreenshot() {
            yield return new WaitForEndOfFrame();
            Texture2D screenshotNonReadable = ScreenCapture.CaptureScreenshotAsTexture();
            currentScreenShot = duplicateTexture(screenshotNonReadable);
            // here we are done taking the screenshots
            UIManager.GetComponent<UIManagerScript>().CaputreScreenshotResult(true);
        }

        public void ShareScreenShot() {
            ISN_UIActivityViewController controller = new ISN_UIActivityViewController();
            controller.SetText("🌎 http://bit.ly/spaces-app 🌎");
            controller.AddImage(currentScreenShot);
            controller.ExcludedActivityTypes.Add(ISN_UIActivityType.SaveToCameraRoll);
            controller.Present((result) => {
                if(result.IsSucceeded) {
                    Debug.Log("Completed: " + result.Completed);
                    Debug.Log("ActivityType: " + result.ActivityType);
                } else {
                    Debug.Log("ISN_UIActivityViewController error: " + result.Error.FullMessage);
                }
            });
        }


        Texture2D duplicateTexture(Texture2D source) {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);
        
            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }


        // UNITY EDITOR SCREENSHOT TESTING
        public void UnitEditorTestScreenshot() {
            StartCoroutine(TestSS(false));
        }

        IEnumerator TestSS(bool success) {
            UIManager.GetComponent<UIManagerScript>().ToggleCaputreScreenshot(true);
            yield return new WaitForSeconds(3);
            UIManager.GetComponent<UIManagerScript>().CaputreScreenshotResult(success);
        }
    }
}
