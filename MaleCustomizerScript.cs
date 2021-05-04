using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedCustomizableSystem;
using System;
using UnityEngine.UI;

namespace Spaces {
    public class MaleCustomizerScript : MonoBehaviour {

        // the index of the available items



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

        public bool inGame = false;

        public List<int> availableShirts, availablePants = new List<int>();

        private List<int> availableShoes = new List<int>() {
            -1
        };
        private List<int> availableHats = new List<int>() {
            -1
        };

        private List<int> availableAccessories = new List<int>() {
            -1
        };
        public GameObject ExtrasPanel;

        public GameObject DummyMale, DummyFemale;

        void Start() {
            skinColors = new List<Color>() {
                new Color(0.9058824f, 0.654902f, 0.5137255f, 1f),
                new Color(0.9528301954269409f, 0.7022832632064819f, 0.624733030796051f, 1f),
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
                new Color(1f, 0.9529412f, 0.5607843f, 1f),
                new Color(0.1647059f, 0.09803922f, 0f, 1f),
                new Color(0.7764706f, 0.3058824f, 0f, 1f),
                new Color(0.945098f, 0.3803922f, 0f, 1f),
                new Color(1f, 1f, 1f, 1f),
                new Color(0.8980392f, 0f, 0f, 1f),
                new Color(1f, 0.9529412f, 0.2705882f, 1f),
                new Color(0.9686275f, 0.7215686f, 0f, 1f),
                new Color(0.1411765f, 0f, 0f, 1f)
            };
            availableShirts = new List<int>(){0, 1};
            availablePants = new List<int>() {0, 1};
            if (inGame) {
                InGameStart();
                return;
            }
            string menjson =  "{\"Hair\":7,\"Hat\":-1,\"TShirt\":9,\"Pants\":7,\"Shoes\":4,\"Beard\":-1,\"Accessory\":9,\"Fat\":0.0,\"Muscles\":1.9389452934265137,\"Slimness\":0.0,\"Thin\":72.66344451904297,\"BreastSize\":0.0,\"Neck_Width\":0.0,\"Ear_Size\":0.0,\"Ear_Angle\":0.0,\"Jaw_Width\":0.0,\"Jaw_Shift\":0.0,\"Jaw_Offset\":0.0,\"Cheek_Size\":0.0,\"Chin_Offset\":0.0,\"Eye_Width\":0.0,\"Eye_Form\":100.0,\"Eye_InnerCorner\":0.0,\"Eye_Corner\":0.0,\"Eye_Rotation\":0.0,\"Eye_Offset\":0.0,\"Eye_ScaleX\":0.0,\"Eye_ScaleY\":0.0,\"Eye_Size\":0.0,\"Eye_Close\":0.0,\"Eye_Height\":0.0,\"Brow_Height\":0.0,\"Brow_Shape\":0.0,\"Brow_Thickness\":0.0,\"Brow_Length\":0.0,\"Nose_Length\":0.0,\"Nose_Size\":0.0,\"Nose_Angle\":0.0,\"Nose_Offset\":0.0,\"Nose_Bridge\":0.0,\"Nose_Hump\":0.0,\"Mouth_Offset\":0.0,\"Mouth_Width\":0.0,\"Mouth_Size\":0.0,\"Mouth_Open\":0.0,\"Mouth_Bulging\":0.0,\"LipsCorners_Offset\":0.0,\"Face_Form\":0.0,\"Chin_Width\":0.0,\"Chin_Form\":0.0,\"Head_Offset\":0.0,\"Height\":0.0,\"Smile\":0.0,\"Sadness\":0.0,\"Surprise\":0.0,\"Thoughtful\":0.0,\"Angry\":0.0,\"HeadSize\":0.0,\"SkinColor\":{\"r\":0.9528301954269409,\"g\":0.7022832632064819,\"b\":0.624733030796051,\"a\":1.0},\"EyeColor\":{\"r\":0.23529411852359773,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"HairColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":1.0},\"UnderpantsColor\":{\"r\":0.5660377740859985,\"g\":0.5660377740859985,\"b\":0.5660377740859985,\"a\":1.0}}";
            string femalejson = "{\"Hair\":0,\"Hat\":-1,\"TShirt\":6,\"Pants\":4,\"Shoes\":0,\"Beard\":-1,\"Accessory\":-1,\"Fat\":0.0,\"Muscles\":0.0,\"Slimness\":110.0,\"Thin\":0.0,\"BreastSize\":1.8776158094406129,\"Neck_Width\":0.0,\"Ear_Size\":0.0,\"Ear_Angle\":0.0,\"Jaw_Width\":0.0,\"Jaw_Shift\":0.0,\"Jaw_Offset\":0.0,\"Cheek_Size\":0.0,\"Chin_Offset\":0.0,\"Eye_Width\":0.0,\"Eye_Form\":0.0,\"Eye_InnerCorner\":0.0,\"Eye_Corner\":0.0,\"Eye_Rotation\":0.0,\"Eye_Offset\":0.0,\"Eye_ScaleX\":0.0,\"Eye_ScaleY\":0.0,\"Eye_Size\":0.0,\"Eye_Close\":0.0,\"Eye_Height\":0.0,\"Brow_Height\":0.0,\"Brow_Shape\":0.0,\"Brow_Thickness\":0.0,\"Brow_Length\":0.0,\"Nose_Length\":0.0,\"Nose_Size\":0.0,\"Nose_Angle\":0.0,\"Nose_Offset\":0.0,\"Nose_Bridge\":0.0,\"Nose_Hump\":0.0,\"Mouth_Offset\":0.0,\"Mouth_Width\":0.0,\"Mouth_Size\":0.0,\"Mouth_Open\":0.0,\"Mouth_Bulging\":0.0,\"LipsCorners_Offset\":0.0,\"Face_Form\":0.0,\"Chin_Width\":0.0,\"Chin_Form\":0.0,\"Head_Offset\":0.0,\"Height\":-0.008261614479124546,\"Smile\":0.0,\"Sadness\":0.0,\"Surprise\":0.0,\"Thoughtful\":0.0,\"Angry\":0.0,\"HeadSize\":0.0,\"SkinColor\":{\"r\":0.9528301954269409,\"g\":0.7022832632064819,\"b\":0.624733030796051,\"a\":1.0},\"EyeColor\":{\"r\":0.7568627595901489,\"g\":0.35686275362968447,\"b\":0.0,\"a\":1.0},\"HairColor\":{\"r\":1.0,\"g\":0.886274516582489,\"b\":0.615686297416687,\"a\":1.0},\"UnderpantsColor\":{\"r\":0.27358490228652956,\"g\":0.27358490228652956,\"b\":0.27358490228652956,\"a\":1.0}}";
            CharacterCustomizationSetup maleCust = CharacterCustomizationSetup.DeserializeFromJson(menjson);
            DummyMale.GetComponent<CharacterCustomization>().SetCharacterSetup(maleCust);
            CharacterCustomizationSetup femaleCust = CharacterCustomizationSetup.DeserializeFromJson(femalejson);
            DummyFemale.GetComponent<CharacterCustomization>().SetCharacterSetup(femaleCust);
            DummyFemale.GetComponent<CharacterCustomization>().SetFaceShape(FaceShapeType.Eye_Form, 100);
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
            CharacterCustomization.SetBeardByIndex(index - 1);
        }

        public void SetShirt() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Shirt, availableShirts[index]);
            hasShirt = true;
        }
        public void SetPants() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Pants, availablePants[index]);
            hasPants = true;
        }
        public void SetShoes() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Shoes, availableShoes[index]);
        }

        public void SetHat() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Hat, availableHats[index]);
        }

        public void SetAccessory() {
            CharacterCustomization.SetElementByIndex(ClothesPartType.Accessory, availableAccessories[index]);
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
                maxIndex = CharacterCustomization.beardPresets.Count + 1;
                PlaceItem = () => {SetBeard();};
                ZoomInOnFace();
            } else if (type == 2) {
                maxIndex = availableShirts.Count;
                PlaceItem = () => {SetShirt();};
                ZoomOutOnFace();
            } else if (type == 3) {
                maxIndex = availablePants.Count;
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
                BodySlider.maxValue = 0.08f;
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
                BodySlider.maxValue = 120;
                BodySlider.value = 0;
                ZoomOutOnFace();
            } else if (type == 10) {
                PlaceItem = ()=> {BodyMuscles();};
                BodySlider.minValue = 0;
                BodySlider.maxValue = 100;
                BodySlider.value = 0;
                ZoomOutOnFace();
            } else if (type == 11) {
                maxIndex = availableShoes.Count;
                PlaceItem = () => {SetShoes();};
                ZoomOutOnFace();
            } else if (type == 12) {
                maxIndex = availableHats.Count;
                PlaceItem = () => {SetHat();};
                ZoomOutOnFace();
            } else if (type == 13) {
                maxIndex = availableAccessories.Count;
                PlaceItem = () => {SetAccessory();};
                ZoomInOnFace();
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
            if (currentBrowsingType > 6 && currentBrowsingType < 11) {
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
                FinalizeSelection.SetActive(false);
                ZoomInOnFace();
            } else if (type == 4) {
                CurrentSelectionPanel = ExtrasPanel;
            }
            MainSelectionPanel.SetActive(false);
            CurrentSelectionPanel.SetActive(true);
            FinalizeSelection.SetActive(false);
            GoBackButton.SetActive(true);
        }


        private void ZoomInOnFace() {
            if (inGame) {
                gameCam.ZoomInOnCharacterFace();
            } else {
                mainCam.ZoomInOnCharacterFace();
            }
        }

        private void ZoomOutOnFace() {
            if (inGame) {
                gameCam.ZoomOutOfCharacterFace();
            } else {
                mainCam.ZoomOutOfCharacterFace();

            }
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
            PlayerPrefs.SetInt("isMale", (isMale ? 1 : 0));
            MaleCustomizationPanel.SetActive(false);
            StartCoroutine(mainCam.GetComponent<CameraTour>().MoveCameraToWorld(json));
            PrebuiltTerrain.SetActive(true);
            PrebuiltWorld.SetActive(true);
        }

        public void SelectSex(bool man) {
            DummyFemale.SetActive(false);
            DummyMale.SetActive(false);
            isMale = man;
            if (man) {
                CharacterCustomization = MaleCustomizer;
                availableShirts = new List<int>(){0, 1};
            } else {
                CharacterCustomization = FemaleCustomizer;
                availableShirts = new List<int>() {1, 2};
            }
            SexSelectionPanel.SetActive(false);
            StartCustomization();
        }

        // IN GAME EDITING FUNCS START HERE


        public string charSetup;

        public CharacterCustomization MaleChar, FemaleChar;

        public UIManagerScript uIManager;

        PlayerFollow gameCam;

        CharacterScript character;

        public void InGameStart() {
            isMale = PlayerPrefs.GetInt("isMale") == 1;
            charSetup = PlayerPrefs.GetString("myCharacter");
            CharacterCustomizationSetup setup = CharacterCustomizationSetup.DeserializeFromJson(charSetup);
            if (isMale) {
                MaleChar.gameObject.SetActive(true);
                CharacterCustomization = MaleChar;
            } else {
                FemaleChar.gameObject.SetActive(true);
                availableShirts = new List<int>() {1, 2};
                CharacterCustomization = FemaleChar;
            }
            CharacterCustomization.SetCharacterSetup(setup);
        }

        public void SetCam(PlayerFollow cam) {
            gameCam = cam;
            gameCam.SetEditableCharacter(MaleChar.transform); // male and female are in same pos
        }


        public void CancelEditing() {
            uIManager.ToggleEditCharacter(false);
            gameCam.ToggleEditing();
        }

        public void StartEditing() {
            gameCam.ToggleEditing();
            gameCam.ZoomInOnCharacter();
        }

        public void FinishedEditingInGame() {
            CharacterCustomizationSetup characterCustomizationSetup = CharacterCustomization.GetSetup();
            string json = characterCustomizationSetup.SerializeToJson();
            PlayerPrefs.SetString("myCharacter", json);  
            character.UpdateCharacter(json);
            CancelEditing();
        }

        public void AddAvailableItems(StoreItem item) {
            // index;type (0=accessory, 1=hat, 2=pants, 3=shirt, 4=shoes)
            try {
                string[] itemData = item.location.Split(';');
                int index = int.Parse(itemData[0]);
                int type = int.Parse(itemData[1]);
                if (type == 0) {
                    availableAccessories.Add(index);
                } else if (type == 1) {
                    availableHats.Add(index);
                } else if (type == 2) {
                    availablePants.Add(index);
                } else if (type == 3) {
                    availableShirts.Add(index);
                } else if (type == 4) {
                    availableShoes.Add(index);
                }
            } catch {
                print("bad item");
            }
        }

        public void SetCharacterScript(CharacterScript cs) {
            character = cs;
        }
    }
}
