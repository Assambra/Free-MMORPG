using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Constants;
using Assambra.FreeClient.Helper;
using Assambra.FreeClient.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UMA.CharacterSystem;
using UMA;
using UnityEngine.UI;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;

namespace Assambra.FreeClient.UserInterface
{
    public class UICreateCharacter : MonoBehaviour
    {
        [field: SerializeField] public bool UseCameraAutoFocus { private get; set; }

        [Header("User Interface")]
        [SerializeField] private TMP_InputField _inputFieldName;
        [SerializeField] private Button _buttonFemale;
        [SerializeField] private Button _buttonMale;
        [SerializeField] private Button _buttonBack;
        [SerializeField] private Button _buttonPlay;
        [SerializeField] private TMP_Dropdown _dropdownRace;

        [Header("Home")]
        [SerializeField] private Transform _heightHome;
        [SerializeField] private Transform _skinHome;
        [SerializeField] private Transform _modifiersHome;
        [SerializeField] private Transform _modifiersButtonHome;
        [SerializeField] private Transform _allwaysOnTop;

        [Header("Prefabs")]
        [SerializeField] private GameObject _prefabTitleElement;
        [SerializeField] private GameObject _prefabSubtitleElement;
        [SerializeField] private GameObject _prefabButtonElement;
        [SerializeField] private GameObject _prefabSliderElement;
        [SerializeField] private GameObject _prefabColorElement;
        [SerializeField] private GameObject _prefabWardrobeElement;

        // Private variables UMA
        private DynamicCharacterAvatar _avatar;
        private UMAData _umaData;

        [Header("Default Character templates")]
        [SerializeField] private List<CharacterTemplate> _characterTemplates = new List<CharacterTemplate>();

        [Header("Camera auto focus points")]
        [SerializeField] private List<CameraFocusPoint> _cameraAutoFocusPoints = new List<CameraFocusPoint>();

        [Header("UMA DNA")]
        [SerializeField] private List<string> _heightDNA = new List<string>();
        [SerializeField] private List<string> _headDNA = new List<string>();
        [SerializeField] private List<string> _upperBodyDNA = new List<string>();
        [SerializeField] private List<string> _lowerBodyDNA = new List<string>();

        [Header("UMA Female Wardrope Types")]
        [SerializeField] private WardrobeTypes _femaleHairWardropeTypes;
        [SerializeField] private WardrobeTypes _femaleClothesWardropeTypes;
        [SerializeField] private WardrobeTypes _femaleEyesWardropeTypes;

        [Header("UMA Male Wardrope Types")]
        [SerializeField] private WardrobeTypes _maleHairWardropeTypes;
        [SerializeField] private WardrobeTypes _maleClothesWardropeTypes;
        [SerializeField] private WardrobeTypes _maleEyesWardropeTypes;

        [Header("UMA General Colors")]
        [SerializeField] private string[] _femaleGeneralColors;
        [SerializeField] private string[] _maleGeneralColors;

        [Header("UMA Female Recipes")]
        [SerializeField] private SO_RecipesToShow _femaleHairRecipesToShow;
        [SerializeField] private SO_RecipesToShow _femaleUnderwearRecipesToShow;
        [SerializeField] private SO_RecipesToShow _femaleEyesRecipesToShow;

        [Header("UMA Male Recipes")]
        [SerializeField] private SO_RecipesToShow _maleHairRecipesToShow;
        [SerializeField] private SO_RecipesToShow _maleUnderwearRecipesToShow;
        [SerializeField] private SO_RecipesToShow _maleEyesRecipesToShow;

        // Private variables user interface
        private List<string> _raceOptions = new List<string>();
        private List<GameObject> _titleElements = new List<GameObject>();

        // Private variables network
        private string _characterName;
        private string _sex;
        private string _race;
        private string _model;

        // Private variables helper/general
        private bool _initalized = false;
        private bool _zeroCharacters = false;
        private GameObject _heightSlider;
        private GameObject _skinColorSelector;

        private GameObject _headSliderGroup = null;
        private GameObject _eyeGroup = null;
        private GameObject _upperBodySliderGroup = null;
        private GameObject _lowerBodySliderGroup = null;
        private GameObject _hairGroup = null;
        private GameObject _clothesGroup = null;
        private GameObject _colorsGroup = null;

        private void Awake()
        {
            _avatar = GameManager.Instance.Avatar;

            _sex = "male";

            _raceOptions.Add("Humanoid");
            _dropdownRace.AddOptions(_raceOptions);

            if (GameManager.Instance.CharacterInfos.Count == 0)
            {
                TMP_Text buttonText = _buttonBack.GetComponentInChildren<TMP_Text>();
                buttonText.text = "Back to Login";
                _zeroCharacters = true;
            }
        }

        private void Start()
        {
            _umaData = _avatar.umaData;
        }

        private void OnDestroy()
        {
            GameManager.Instance.CharacterCreatedAndReadyToPlay = false;
            GameManager.Instance.CharacterId = 0;
            if (_buttonPlay.gameObject.activeSelf)
                _buttonPlay.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_initalized)
            {
                // To initialize the HumanMale Race with the default CharacterTemplate
                OnButtonMale();

                _initalized = true;
            }

            if (GameManager.Instance.CharacterCreatedAndReadyToPlay)
            {
                if (!_buttonPlay.gameObject.activeSelf)
                    _buttonPlay.gameObject.SetActive(true);

                if (_zeroCharacters)
                {
                    _zeroCharacters = false;
                    TMP_Text buttonText = _buttonBack.GetComponentInChildren<TMP_Text>();
                    buttonText.text = "Back";
                }
            }
        }

        private void CreateSkinColor()
        {
            foreach (OverlayColorData colorType in _avatar.ActiveColors)
            {
                if (colorType.name == "Skin")
                {
                    _skinColorSelector = Instantiate(_prefabColorElement, _skinHome);
                    ColorSelectorElement color = _skinColorSelector.GetComponent<ColorSelectorElement>();
                    color.Initialize(_avatar, colorType, "Skin", _allwaysOnTop);
                }
            }
        }

        private void CreateHeightSlider()
        {
            UMADnaBase[] DNA = _avatar.GetAllDNA();

            foreach (UMADnaBase dna in DNA)
            {
                string[] names = dna.Names;
                float[] values = dna.Values;

                List<string> categories = new List<string>();
                categories = CreateCategoryList(names, _heightDNA);

                foreach (string category in categories)
                {
                    _heightSlider = Instantiate(_prefabSliderElement, _heightHome);
                    SliderElement heightSlider = _heightSlider.GetComponent<SliderElement>();

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (category == GetCategory(names[i].BreakupCamelCase()))
                        {
                            heightSlider.InitializeSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, _avatar, dna, true, 150f);
                            break;
                        }
                    }
                }
            }
        }

        #region CHARACTER MODIFIER BUTTONS

        private void CreateModifierButtons()
        {
            GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersButtonHome);
            gotitle.name = "Character modifiers";
            _titleElements.Add(gotitle);

            TitleElement te = gotitle.GetComponent<TitleElement>();
            string headerName = "Character Modifiers";
            te.InitializeHeaderElement(headerName, _modifiersButtonHome.GetComponent<RectTransform>());

            GameObject goHead = te.CreateObject(_prefabButtonElement, "Head");
            ButtonElement beHead = goHead.GetComponent<ButtonElement>();
            beHead.Initialize("Head", OnButtonHeadClick);

            GameObject goEye = te.CreateObject(_prefabButtonElement, "Eyes");
            ButtonElement beEye = goEye.GetComponent<ButtonElement>();
            beEye.Initialize("Eyes", OnButtonEyeClick);

            GameObject goUpperBody = te.CreateObject(_prefabButtonElement, "Upper Body");
            ButtonElement beUpperBody = goUpperBody.GetComponent<ButtonElement>();
            beUpperBody.Initialize("Upper Body", OnButtonUpperBodyClick);

            GameObject goLowerBody = te.CreateObject(_prefabButtonElement, "Lower Body");
            ButtonElement beLowerBody = goLowerBody.GetComponent<ButtonElement>();
            beLowerBody.Initialize("Lower Body", OnButtonLowerBodyClick);

            GameObject goHair = te.CreateObject(_prefabButtonElement, "Hair");
            ButtonElement beHair = goHair.GetComponent<ButtonElement>();
            beHair.Initialize("Hair", OnButtonHairClick);

            GameObject goColors = te.CreateObject(_prefabButtonElement, "Colors");
            ButtonElement beColors = goColors.GetComponent<ButtonElement>();
            beColors.Initialize("Colors", OnButtonColorsClick);

            GameObject goClothes = te.CreateObject(_prefabButtonElement, "Clothes");
            ButtonElement beClothes = goClothes.GetComponent<ButtonElement>();
            beClothes.Initialize("Clothes", OnButtonClothesClick);
        }

        private void OnButtonHeadClick()
        {
            if (_headSliderGroup == null)
            {
                _headSliderGroup = CreateSlidersGroup("Head", _headDNA);
            }
            else
                Destroy(_headSliderGroup);
        }

        private void OnButtonEyeClick()
        {
            if (_eyeGroup == null)
            {
                if (_avatar.activeRace.name == "HumanMale")
                {
                    _eyeGroup = CreateWardrobeGroup(_maleEyesWardropeTypes.Title, _maleEyesWardropeTypes, _maleEyesRecipesToShow);
                }
                else if (_avatar.activeRace.name == "HumanFemale")
                {
                    _eyeGroup = CreateWardrobeGroup(_femaleEyesWardropeTypes.Title, _femaleEyesWardropeTypes, _femaleEyesRecipesToShow);
                }
            }
            else
                Destroy(_eyeGroup);
        }

        private void OnButtonUpperBodyClick()
        {
            if (_upperBodySliderGroup == null)
            {
                _upperBodySliderGroup = CreateSlidersGroup("Upper Body", _upperBodyDNA);
            }
            else
                Destroy(_upperBodySliderGroup);
        }

        private void OnButtonLowerBodyClick()
        {
            if (_lowerBodySliderGroup == null)
            {
                _lowerBodySliderGroup = CreateSlidersGroup("Lower Body", _lowerBodyDNA);
            }
            else
                Destroy(_lowerBodySliderGroup);
        }

        private void OnButtonHairClick()
        {
            if (_hairGroup == null)
            {
                if (_avatar.activeRace.name == "HumanMale")
                {
                    _hairGroup = CreateWardrobeGroup(_maleHairWardropeTypes.Title, _maleHairWardropeTypes, _maleHairRecipesToShow);
                }
                else if (_avatar.activeRace.name == "HumanFemale")
                {
                    _hairGroup = CreateWardrobeGroup(_femaleHairWardropeTypes.Title, _femaleHairWardropeTypes, _femaleHairRecipesToShow);
                }
            }
            else
                Destroy(_hairGroup);
        }

        private void OnButtonColorsClick()
        {
            if (_colorsGroup == null)
            {
                if (_avatar.activeRace.name == "HumanMale")
                {
                    _colorsGroup = CreateColorGroup("Colors", _maleGeneralColors);
                }
                else if (_avatar.activeRace.name == "HumanFemale")
                {
                    _colorsGroup = CreateColorGroup("Colors", _femaleGeneralColors);
                }
            }
            else
                Destroy(_colorsGroup);
        }

        private void OnButtonClothesClick()
        {
            if (_clothesGroup == null)
            {
                if (_avatar.activeRace.name == "HumanMale")
                {
                    _clothesGroup = CreateWardrobeGroup(_maleClothesWardropeTypes.Title, _maleClothesWardropeTypes, _maleUnderwearRecipesToShow);
                }
                else if (_avatar.activeRace.name == "HumanFemale")
                {
                    _clothesGroup = CreateWardrobeGroup(_femaleClothesWardropeTypes.Title, _femaleClothesWardropeTypes, _femaleUnderwearRecipesToShow);
                }
            }
            else
                Destroy(_clothesGroup);
        }

        private GameObject CreateSlidersGroup(string title, List<string> dnaToShow)
        {
            GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersHome);
            gotitle.name = title;
            _titleElements.Add(gotitle);

            TitleElement te = gotitle.GetComponent<TitleElement>();
            te.InitializeHeaderElement(title, _modifiersHome.GetComponent<RectTransform>());

            RectTransform parentLayout = te.GetParentLayout();
            RectTransform layout = te.GetLayout();

            UMADnaBase[] DNA = _avatar.GetAllDNA();

            foreach (UMADnaBase dna in DNA)
            {
                string[] names = dna.Names;
                float[] values = dna.Values;

                List<string> categories = new List<string>();
                categories = CreateCategoryList(names, dnaToShow);

                foreach (string category in categories)
                {
                    GameObject subtitle = te.CreateObject(_prefabSubtitleElement, category);
                    TitleElement subhe = subtitle.GetComponent<TitleElement>();
                    subhe.InitializeHeaderElement(category, layout, true, parentLayout);

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (category == GetCategory(names[i].BreakupCamelCase()))
                        {
                            GameObject slider = subhe.CreateObject(_prefabSliderElement, GetSlider(names[i].BreakupCamelCase()));
                            SliderElement so = slider.GetComponent<SliderElement>();
                            so.InitializeSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, _avatar, dna);
                        }
                    }
                }
            }

            return gotitle;
        }

        private GameObject CreateWardrobeGroup(string title, WardrobeTypes wardrobeTypes, SO_RecipesToShow recipesToShow)
        {
            GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersHome);
            gotitle.name = title;
            _titleElements.Add(gotitle);

            TitleElement te = gotitle.GetComponent<TitleElement>();
            te.InitializeHeaderElement(title, _modifiersHome.GetComponent<RectTransform>());

            Dictionary<string, List<UMATextRecipe>> recipes = _avatar.AvailableRecipes;

            foreach (string r in recipes.Keys)
            {
                foreach (WardrobeType wt in wardrobeTypes.wardrobeTypes)
                {
                    if (r == wt.Type)
                    {
                        GameObject go = te.CreateObject(_prefabWardrobeElement, r);
                        go.GetComponent<WardrobeElement>().InitializeWardrobe(_avatar, r, _modifiersButtonHome, _allwaysOnTop, recipesToShow, wt.HasNoneOption);
                    }
                }
            }

            return gotitle;
        }

        private GameObject CreateColorGroup(string title, string[] colorsToShow)
        {
            GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersHome);
            gotitle.name = title;
            _titleElements.Add(gotitle);

            TitleElement te = gotitle.GetComponent<TitleElement>();
            te.InitializeHeaderElement(title, _modifiersHome.GetComponent<RectTransform>());

            int i = colorsToShow.Length - 1;
            foreach (OverlayColorData colorType in _avatar.ActiveColors)
            {
                if (colorType.name == colorsToShow[i])
                {
                    GameObject go = te.CreateObject(_prefabColorElement, colorsToShow[i]);
                    ColorSelectorElement cse = go.GetComponent<ColorSelectorElement>();
                    cse.Initialize(_avatar, colorType, colorType.name, _allwaysOnTop);

                    i--;

                    if (i < 0)
                        break;
                }
            }
            return gotitle;
        }

        #endregion

        private void RemoveSkinColor()
        {
            ColorSelectorElement co = _skinColorSelector.GetComponent<ColorSelectorElement>();
            co.RemoveColorPicker();
            Destroy(_skinColorSelector);
        }

        private void RemoveHeightSlider()
        {
            Destroy(_heightSlider);
        }

        private void RemoveModifiers()
        {
            Destroy(_headSliderGroup);
            Destroy(_eyeGroup);
            Destroy(_upperBodySliderGroup);
            Destroy(_lowerBodySliderGroup);
            Destroy(_hairGroup);
            Destroy(_clothesGroup);
            Destroy(_colorsGroup);

            foreach (GameObject go in _titleElements)
            {
                if (go != null)
                    Destroy(go);
            }
        }

        private List<string> CreateCategoryList(string[] names, List<string> toShow)
        {
            var list = new List<string>();

            for (int i = 0; i < names.Length; i++)
            {
                if (toShow.Contains(names[i]))
                {
                    string cat = GetCategory(names[i].BreakupCamelCase());

                    if (!list.Contains(cat))
                        list.Add(cat);
                }
            }

            return list;
        }

        private string GetCategory(string name)
        {
            string[] cat = name.Split();
            return cat[0];
        }

        private string GetSlider(string name)
        {
            string[] cat = name.Split();

            if (cat.Length == 3)
                return cat[1] + " " + cat[2];
            else if (cat.Length == 2)
                return cat[1];
            else
                return cat[0];
        }

        private void CreateCharacterModifiers()
        {
            CreateSkinColor();
            CreateHeightSlider();
            CreateModifierButtons();
        }

        private void RemoveCharacterModifiers()
        {
            RemoveSkinColor();
            RemoveHeightSlider();
            RemoveModifiers();
        }

        #region BUTTON HANDLER

        public void OnButtonMale()
        {
            if (_avatar.activeRace.name != "HumanMale" || !_initalized)
            {
                _buttonMale.interactable = false;
                _buttonFemale.interactable = false;

                _umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnMaleCharacterUpdated));
                _avatar.ChangeRace("HumanMale", true);

                if (_initalized)
                    RemoveCharacterModifiers();

                _sex = "male";
            }
        }

        public void OnButtonFemale()
        {
            if (_avatar.activeRace.name != "HumanFemale")
            {
                _buttonMale.interactable = false;
                _buttonFemale.interactable = false;

                _umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnFemaleCharacterUpdated));
                _avatar.ChangeRace("HumanFemale", true);

                RemoveCharacterModifiers();

                _sex = "female";
            }
        }

        public void OnButtonBack()
        {
            if (!_zeroCharacters)
                GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
            else
            {
                NetworkManager.Instance.LoginState = LoginState.Lobby;
                GameManager.Instance.ChangeScene(Scenes.Login);
            }
        }

        public void OnButtonCreate()
        {
            _model = UMAHelper.GetAvatarString(_avatar);
            _characterName = _inputFieldName.text;
            _race = _raceOptions[_dropdownRace.value];

            if (ValidateCharacterName(_characterName))
                NetworkManager.Instance.CreateCharacter(_characterName, _sex, _race, _model);
        }

        public void OnButtonPlay()
        {
            NetworkManager.Instance.PlayRequest(GameManager.Instance.CharacterId);
        }

        #endregion

        #region UMA EVENT HANDLER

        public void OnFemaleCharacterUpdated(UMAData data)
        {
            Debug.Log("OnFemaleCharacterUpdated");
            data.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnFemaleCharacterUpdated));

            UMAHelper.SetAvatarString(_avatar, _characterTemplates[1].UmaCharacterString);
            CreateCharacterModifiers();

            _buttonMale.interactable = true;
            _buttonFemale.interactable = true;
        }

        public void OnMaleCharacterUpdated(UMAData data)
        {
            Debug.Log("OnMaleCharacterUpdated");
            data.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnMaleCharacterUpdated));

            UMAHelper.SetAvatarString(_avatar, _characterTemplates[0].UmaCharacterString);
            CreateCharacterModifiers();

            _buttonMale.interactable = true;
            _buttonFemale.interactable = true;
        }

        #endregion

        #region VALIDATE

        private bool ValidateCharacterName(string characterName)
        {
            bool isValid = true;

            if (!InputValidator.IsLengthValid(characterName, GameConstants.CHARACTER_NAME_LENGTH_MIN, GameConstants.CHARACTER_NAME_LENGTH_MAX))
            {
                isValid = false;
                ErrorPopup("The username must be at least " + GameConstants.CHARACTER_NAME_LENGTH_MIN + " and at most " + GameConstants.CHARACTER_NAME_LENGTH_MAX + " letters long.");
            }

            if (!InputValidator.DoesNotContainDisallowedName(characterName, GameConstants.DISALLOWED_NAMES))
            {
                isValid = false;
                ErrorPopup("Username are not allowed!");
            }

            return isValid;
        }

        #endregion

        #region POPUP

        private void ErrorPopup(string error)
        {
            string title = "Error";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Error, title, error);
        }

        #endregion
    }
}
