using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.UIKit;
using SA.iOS.Photos;
using SA.iOS.Social;
using AdvancedCustomizableSystem;

namespace Spaces {
    public class PhotoManagerPublicScript : MonoBehaviour {
        private PlayerFollow mainCam;

        public GameObject GoBackHomeB, TakeScreenShotB, CaptureScreenShotB, CancelScreenShotB, SuccessScreenShotB, MessageButton;

        public GameObject ErrorModal, ScreenshotLogo, ScreenshotName, ModalPanel, ModalPanelTitle, ModalPanelSubtitle, ModalButtonText, joystick, coins;
        public GameObject WorldWelcome, ShareScreenShotB;

        private int mattermostGreen = -1;

        private Texture2D currentScreenShot;

        public GameObject microhponeButton;

        private bool isTalking = false;

        public GameObject uiManager;

        CharacterCustomization characterCustomization;


        public void ShareProperties(PlayerFollow cam, Transform character) {
            mainCam = cam;
            characterCustomization = character.GetComponent<CharacterCustomization>();
        }

        public void SetEmotion(int index) {
            var emotion = characterCustomization.emotionPresets[index];
            if (emotion != null) {
                characterCustomization.PlayEmotion(emotion.name, 3.5f, 2f);
            }
        }

        public void ToggleSitDownPerspective() {
            mainCam.GetComponent<PlayerFollow>().SitCameraToogle();
        }

        public void SetUpScreenshot() {
            ToggleScreenshot(true);
        }

        void ToggleScreenshot(bool open) {
            uiManager.GetComponent<UIManagerPublicScript>().ToggleScreenshot(open);
            mainCam.ToggleRotateCamera(open);
        }

        public void CancelScreenshot() {
            ToggleScreenshot(false);
        }

        public void TakeScreenshot() {
            ISN_PHPhotoLibrary.RequestAuthorization((status) => {
                if(status == ISN_PHAuthorizationStatus.Authorized) {
                    ToggleCaputreScreenshot(true);
                    ISN_UIImagePickerController.SaveScreenshotToCameraRoll((result) => {
                    if (result.IsSucceeded) {
                        StartCoroutine(ShareableScreenshot());
                        Debug.Log("screenshot saved saved");
                    } else {
                        CaputreScreenshotResult(false);
                        Debug.Log("Error: " + result.Error.Message);
                    }
                    });
                } else {
                    CaputreScreenshotResult(false);
                    Debug.Log("Permission denied");
                }
            });
        }

        public IEnumerator ShareableScreenshot() {
            yield return new WaitForEndOfFrame();
            Texture2D screenshotNonReadable = ScreenCapture.CaptureScreenshotAsTexture();
            currentScreenShot = duplicateTexture(screenshotNonReadable);
            CaputreScreenshotResult(true);
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

        public void ToggleCaputreScreenshot(bool isTakingShot) {
            CaptureScreenShotB.SetActive(!isTakingShot);
            CancelScreenShotB.SetActive(!isTakingShot);
            ScreenshotLogo.SetActive(isTakingShot);
            ScreenshotName.SetActive(isTakingShot);
            uiManager.GetComponent<UIManagerPublicScript>().ToggleEmotions(!isTakingShot);
        }

        public void CaputreScreenshotResult(bool success) {
            if (success) {
                ScreenshotName.SetActive(false);
                ScreenshotLogo.SetActive(false);
                SuccessScreenShotB.SetActive(true);
                ShareScreenShotB.SetActive(true);
            } else {
                ToggleCaputreScreenshot(false); 
                SetUpPanel("There was an error taking your screenshot", "In settings, make sure you have permissions enabled for spaces to take screenshots", "try again");
            }
        }

        public void CloseShareScreenShot() {
            SuccessScreenShotB.SetActive(false);
            ShareScreenShotB.SetActive(false);
            ToggleCaputreScreenshot(false);
        }


        public void CloseModalPanel() {
            ModalPanel.SetActive(false);
        }

        public void SetUpPanel(string title, string text, string buttonText) {
            ModalPanelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = title;
            ModalPanelSubtitle.GetComponent<TMPro.TextMeshProUGUI>().text = text;
            ModalButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = buttonText;
            ModalPanel.SetActive(true);
        }
        // UNITY EDITOR SCREENSHOT TESTING
        public void UnitEditorTestScreenshot() {
            StartCoroutine(TestSS(false));
        }

        IEnumerator TestSS(bool success) {
            ToggleCaputreScreenshot(true);
            yield return new WaitForSeconds(3);
            CaputreScreenshotResult(success);
        }
    }
}
