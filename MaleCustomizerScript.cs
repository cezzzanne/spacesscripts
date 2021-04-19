using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedCustomizableSystem;
using System;
using UnityEngine.UI;

namespace Spaces {
    public class MaleCustomizerScript : MonoBehaviour {

        List<int> availablePants = new List<int>() {
            0,
            1
        };

        // the index of the available items

        List<int> availabeShirts = new List<int>() {
            0,
            1
        };

        int index = 0;

        int currentBrowsingType = 0;

        int currentBrowsingSection = 0;
        // 0 is hair, 1 is beard, 2 is shirt, and 3 is pants

        public GameObject MainSelectionPanel, MainClothesPanel, MainColorsPanel;

        public GameObject PreviousItemButton, NextItemButton;

        public CharacterCustomization CharacterCustomization;

        private GameObject CurrentSelectionPanel;

        public GameObject ConfirmItemButton, MainBackDrop;


        private Action PlaceItem;

        int maxIndex = 0;

        private List<Color> skinColors, hairColors, eyeColors;

        public GameObject MainBodyPanel;

        public GameObject BodySliderObject;

        public Slider BodySlider;

        public float currentSliderValue = 0;


        public Slider[] faceShapeSliders;

        private int currentFaceTypeIndex = 0;

        private int currentFaceShapeSliderIndex = 0;

        private Dictionary<int, int> faceShapeSliderToFaceType = new Dictionary<int, int>() {
            {0, 0},
            {1, 1},
            {2, 2},
            {3, 4},
            {4, 6},
            {5, 7},
            {6, 8},
            {7, 13},
            {8, 17},
            {9, 19},
            {10, 21},
            {11, 22},
            {12, 23},
            {13, 24},
            {14, 25},
            {15, 26}, 
            {16, 28},
            {17, 30},
            {18, 36},
            {19, 37}
        };

        public TMPro.TextMeshProUGUI FaceShapeText;

        public GameObject MainDetailsNamePanel;

        public CameraTour mainCam;

        private Vector3 mainCamInitialPos;

        public GameObject GoBackButton;

        private bool isMale = true;

        public GameObject MaleCustomizationPanel;

        public GameObject  SexSelectionPanel;

        public CharacterCustomization MaleCustomizer, FemaleCustomizer;

        public GameObject FemaleClothesPanel, FemaleColorsPanel, FemaleBodyPanel;

        bool hasShirt, hasPants;

        public GameObject NeedsToSetClothes, FinalizeSelection;

        public GameObject PrebuiltTerrain, PrebuiltWorld;


        void Start() {
            skinColors = new List<Color>() {
                new Color(0.9058824f, 0.654902f, 0.5137255f, 1f),
                new Color(0.9058824f, 0.5764706f, 0.4470588f, 1f),
                new Color(0.8901961f, 0.5372549f, 0.4117647f, 1f),
                new Color(0.8196079f, 0.4666667f, 0.3372549f, 1f),
                new Color(0.7764706f, 0.4078431f, 0.2745098f, 1f),
                new Color(0.7568628f, 0.3882353f, 0.254902f, 1f),
                new Color(0.6980392f, 0.3372549f, 0.2078431f, 1f),
                new Color(0.627451f, 0.2901961f, 0.1647059f, 1f),
                new Color(0.6235294f, 0.2862745f, 0.1607843f, 1f),
                new Color(0.5450981f, 0.2627451f, 0.1568628f, 1f),
                new Color(0.4470588f, 0.2196078f, 0.1333333f, 1f)
            };
            eyeColors = new List<Color>() {
                new Color(0f, 0.003921569f, 0f, 1f),
                new Color(0f, 0.2705882f, 0f, 1f),
                new Color(0.3686275f, 0.3137255f, 0f, 1f),
                new Color(0.3058824f, 0.2705882f, 0f, 1f),
                new Color(0f, 0.5254902f, 0.5254902f, 1f),
                new Color(0.6313726f, 0.3686275f, 0f, 1f)
            };
            hairColors = new List<Color>() {
                new Color(0.9960784f, 0.9764706f, 0.6156863f, 1f),
                new Color(0.1137255f, 0.07450981f, 0f, 1f),
                new Color(0f, 0.05882353f, 0f, 1f),
                new Color(0.7764706f, 0.3058824f, 0f, 1f),
                new Color(0.945098f, 0.3803922f, 0f, 1f),
                new Color(1f, 1f, 1f, 1f),
                new Color(0.8980392f, 0f, 0f, 1f),
                new Color(1f, 0.9529412f, 0.2705882f, 1f),
                new Color(0.9686275f, 0.7215686f, 0f, 1f),
                new Color(0.1411765f, 0f, 0f, 1f)
            };
            int menOrWoman = UnityEngine.Random.Range(-1, 1);
            if (menOrWoman < 0) {
                MaleCustomizer.gameObject.SetActive(true);
                MaleCustomizer.Randomize();
            } else {
                FemaleCustomizer.gameObject.SetActive(true);
                FemaleCustomizer.Randomize();
            }
        }

        public void GoToSexSelection(CameraTour ct) {
            mainCam = ct;
            SexSelectionPanel.SetActive(true);
        }

        public void StartCustomization() {
            MaleCustomizationPanel.SetActive(true);
            CharacterCustomization.gameObject.SetActive(true);
            CharacterCustomization.ResetAll();
            CharacterCustomization.SetFaceShape(FaceShapeType.Eye_Form, 100);
            mainCam.ZoomInOnCharacter();
        }

        public void SetHair() {
            CharacterCustomization.SetHairByIndex(index);
        }
        public void SetBeard() {
            CharacterCustomization.SetBeardByIndex(index);
        }

        public void SetShirt() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Shirt, index);
            hasShirt = true;
        }
        public void SetPants() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Pants, index);
            hasPants = true;
        }

        public void SetSkinColor() {
            Color color = skinColors[index];
            CharacterCustomization.SetBodyColor(BodyColorPart.Skin, color);
        }

        public void SetEyeColor() {
            Color color = eyeColors[index];
            CharacterCustomization.SetBodyColor(BodyColorPart.Eye, color);
        }

        public void SetHairColor() {
            Color color = hairColors[index];
            CharacterCustomization.SetBodyColor(BodyColorPart.Hair, color);
        }

        public void BodyFat() {
            CharacterCustomization.SetBodyShape(BodyShapeType.Fat, currentSliderValue);
        }

        public void BodyMuscles() {
            CharacterCustomization.SetBodyShape(BodyShapeType.Muscles, currentSliderValue);
        }

        public void BodyThin() {
            if (isMale) {
                CharacterCustomization.SetBodyShape(BodyShapeType.Thin, currentSliderValue);
            } else {
                BodySlimness();
            }
        }

        public void BodySlimness() {
            CharacterCustomization.SetBodyShape(BodyShapeType.Slimness, currentSliderValue);
        }


        public void SetHeight() {
            CharacterCustomization.SetHeight(currentSliderValue);
        }


        public void NextItem() {
            if (currentBrowsingSection == 3) {
                NextDetails();
                return;
            }
            int newIndex = index + 1;
            newIndex = (newIndex < maxIndex) ? newIndex : 0; // TODO: maybe zoom in on face
            index = newIndex;
            PlaceItem();
        }

        public void PreviousItem() {
            if (currentBrowsingSection == 3) {
                PreviousDetails();
                return;
            }
            int newIndex = index - 1;
            newIndex = (newIndex >= 0) ? newIndex : (maxIndex - 1);
            index = newIndex;
            PlaceItem();
        }
        public void SetBrowsingType(int i) {
            currentBrowsingType = i;
            SetUpBrowsing(i);
            TogglePickItem(true);
        }

        private void SetUpBrowsing(int type) {
            if (type == 0) {
                maxIndex = CharacterCustomization.hairPresets.Count;
                PlaceItem = () => {SetHair();};
                ZoomInOnFace();
            } else if (type == 1) {
                maxIndex = CharacterCustomization.beardPresets.Count;
                PlaceItem = () => {SetBeard();};
                ZoomInOnFace();
            } else if (type == 2) {
                maxIndex = 2;
                PlaceItem = () => {SetShirt();};
                ZoomOutOnFace();
            } else if (type == 3) {
                maxIndex = 2;
                PlaceItem = () => {SetPants();};
                ZoomOutOnFace();
            } else if (type == 4) {
                maxIndex = skinColors.Count;
                PlaceItem = () => {SetSkinColor();};
                ZoomOutOnFace();
            } else if (type == 5) {
                maxIndex = eyeColors.Count;
                PlaceItem = () => {SetEyeColor();};
                ZoomInOnFace();
            } else if (type == 6) {
                maxIndex = hairColors.Count;
                PlaceItem = () => {SetHairColor();};
                ZoomInOnFace();
            } else if (type == 7) {
                PlaceItem = ()=> {SetHeight();};
                BodySlider.minValue = 0;
                BodySlider.maxValue = 0.16f;
                BodySlider.value = BodySlider.maxValue / 2;
                ZoomOutOnFace();
            } else if (type == 8) {
                PlaceItem = ()=> {BodyFat();};
                BodySlider.minValue = 0;
                BodySlider.maxValue = 100;
                BodySlider.value = 0;
                ZoomOutOnFace();
            } else if (type == 9) {
                PlaceItem = ()=> {BodyThin();};
                BodySlider.minValue = 0;
                BodySlider.maxValue = 100;
                BodySlider.value = 0;
                ZoomOutOnFace();
            } else if (type == 10) {
                PlaceItem = ()=> {BodyMuscles();};
                BodySlider.minValue = 0;
                BodySlider.maxValue = 100;
                BodySlider.value = 0;
                ZoomOutOnFace();
            }
        }

        public void SliderValueChanged(float i) {
            currentSliderValue = i;
            PlaceItem();           
        }
        // specific functions for details

        public void SetFaceShape() {
            CharacterCustomization.SetFaceShape((FaceShapeType)(currentFaceTypeIndex), faceShapeSliders[currentFaceShapeSliderIndex].value);
        }

        public void NextDetails() {
            int newIndex = currentFaceShapeSliderIndex + 1;
            newIndex = (newIndex < faceShapeSliders.Length) ? newIndex : 0;
            currentFaceTypeIndex = faceShapeSliderToFaceType[newIndex];
            faceShapeSliders[newIndex].gameObject.transform.parent.parent.gameObject.SetActive(true);
            faceShapeSliders[currentFaceShapeSliderIndex].gameObject.transform.parent.parent.gameObject.SetActive(false);
            currentFaceShapeSliderIndex = newIndex;
            FaceShapeText.text = faceShapeSliders[newIndex].gameObject.transform.parent.parent.gameObject.name;
        }

        public void PreviousDetails() {
            int newIndex = currentFaceShapeSliderIndex - 1;
            newIndex = (newIndex < 0) ? (faceShapeSliders.Length - 1) : newIndex;
            currentFaceTypeIndex = faceShapeSliderToFaceType[newIndex];
            faceShapeSliders[newIndex].gameObject.transform.parent.parent.gameObject.SetActive(true);
            faceShapeSliders[currentFaceShapeSliderIndex].gameObject.transform.parent.parent.gameObject.SetActive(false);
            currentFaceShapeSliderIndex = newIndex;
            FaceShapeText.text = faceShapeSliders[newIndex].gameObject.transform.parent.parent.gameObject.name;
        }


        // end of specific functions for details

        public void ConfirmItem() {
            if (currentBrowsingSection == 3) {
                NextItemButton.SetActive(false);
                PreviousItemButton.SetActive(false);
                CurrentSelectionPanel.SetActive(false);
                faceShapeSliders[currentFaceShapeSliderIndex].gameObject.transform.parent.parent.gameObject.SetActive(false);
                MainSelectionPanel.SetActive(true);
                FinalizeSelection.SetActive(true);
                ConfirmItemButton.SetActive(false);
                return;
            }
            ZoomOutOnFace();
            TogglePickItem(false);
        }

        void TogglePickItem(bool open) {
            if (currentBrowsingType > 6) {
                BodySliderObject.SetActive(open);
            } else {
                NextItemButton.SetActive(open);
                PreviousItemButton.SetActive(open);
                MainBackDrop.SetActive(!open);
            }
            ConfirmItemButton.SetActive(open);
            GoBackButton.SetActive(!open);
            CurrentSelectionPanel.SetActive(!open); 
        }

        public void SelectBrowsingSection(int type) {
            currentBrowsingSection = type;
            if (type == 0) {
                CurrentSelectionPanel = isMale ? MainClothesPanel : FemaleClothesPanel;
            } else if (type == 1) {
                CurrentSelectionPanel = isMale ? MainColorsPanel : FemaleColorsPanel;
            } else if (type == 2) {
                CurrentSelectionPanel = isMale ? MainBodyPanel : FemaleBodyPanel;
            } else if (type == 3) {
                CurrentSelectionPanel = MainDetailsNamePanel;
                faceShapeSliders[currentFaceShapeSliderIndex].gameObject.transform.parent.parent.gameObject.SetActive(true);
                NextItemButton.SetActive(true);
                PreviousItemButton.SetActive(true);
                ConfirmItemButton.SetActive(true);
                ZoomInOnFace();
            }
            MainSelectionPanel.SetActive(false);
            CurrentSelectionPanel.SetActive(true);
            FinalizeSelection.SetActive(false);
            GoBackButton.SetActive(true);
        }


        private void ZoomInOnFace() {
            mainCam.ZoomInOnCharacterFace();
        }

        private void ZoomOutOnFace() {
            mainCam.ZoomOutOfCharacterFace();
        }

        public void GoBackToMainSelection() {
            GoBackButton.SetActive(false);
            ConfirmItemButton.SetActive(false);
            faceShapeSliders[currentFaceShapeSliderIndex].gameObject.transform.parent.parent.gameObject.SetActive(false);
            NextItemButton.SetActive(false);
            PreviousItemButton.SetActive(false);
            CurrentSelectionPanel.SetActive(false);
            MainSelectionPanel.SetActive(true);
            FinalizeSelection.SetActive(true);
            ZoomOutOnFace();
        }

        public void FinishedEditing() {
            NeedsToSetClothes.SetActive(false);
            if (!(hasShirt && hasPants)) {
                NeedsToSetClothes.SetActive(true);
                return;
            }
            CharacterCustomizationSetup characterCustomizationSetup = CharacterCustomization.GetSetup();
            string json = characterCustomizationSetup.SerializeToJson();
            mainCam.ZoomOutOnCharacter();
            PlayerPrefs.SetString("myWorldType", "MainGame");
            PlayerPrefs.SetString("myCharacter", json);
            MaleCustomizationPanel.SetActive(false);
            StartCoroutine(mainCam.GetComponent<CameraTour>().MoveCameraToWorld(json));
            PrebuiltTerrain.SetActive(true);
            PrebuiltWorld.SetActive(true);
        }

        public void SelectSex(bool man) {
            isMale = man;
            if (man) {
                CharacterCustomization = MaleCustomizer;
                FemaleCustomizer.gameObject.SetActive(false);
            } else {
                CharacterCustomization = FemaleCustomizer;
                MaleCustomizer.gameObject.SetActive(false);
            }
            SexSelectionPanel.SetActive(false);
            StartCustomization();
        }

    }
}
