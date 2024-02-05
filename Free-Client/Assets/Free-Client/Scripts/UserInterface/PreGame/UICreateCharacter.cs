using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UMA.CharacterSystem;
using UMA;
using UnityEngine.UI;

public class UICreateCharacter : MonoBehaviour
{
    [field: SerializeField] public bool UseCameraAutoFocus { private get; set; }

    [Header("User Interface")]
    [SerializeField] TMP_InputField inputFieldNameValue;
    [SerializeField] Button buttonFemale;
    [SerializeField] Button buttonMale;
    [SerializeField] Transform heightHome;
    [SerializeField] Transform skinHome;
    [SerializeField] Transform allwaysOnTop;
    [SerializeField] TMP_Dropdown dropdownRace;
    [SerializeField] TMP_Dropdown dropdownProfession;
    [SerializeField] private GameObject _prefabTitleElement;
    [SerializeField] private GameObject _prefabSubtitleElement;
    [SerializeField] private GameObject _prefabButtonElement;
    [SerializeField] private GameObject _prefabSliderElement;
    [SerializeField] private GameObject _prefabColorElement;
    [SerializeField] private GameObject _prefabWardrobeElement;
    
    [SerializeField] Transform _modifiersHome;
    [SerializeField] Transform _modifiersButtonHome;

    [SerializeField] Button buttonPlay;

    // Private variables UMA
    private DynamicCharacterAvatar avatar;
    private UMAData umaData;

    [Header("Default Character templates")]
    [SerializeField] private List<CharacterTemplate> characterTemplates = new List<CharacterTemplate>();

    [Header("Camera auto focus points")]
    [SerializeField] private List<CameraFocusPoint> cameraAutoFocusPoints = new List<CameraFocusPoint>();

    [Header("UMA DNA")]
    [SerializeField] private List<string> _heightDNA = new List<string>();
    [SerializeField] private List<string> _headDNA = new List<string>();
    [SerializeField] private List<string> _upperBodyDNA = new List<string>();
    [SerializeField] private List<string> _lowerBodyDNA = new List<string>();

    [Header("UMA Wardrope Types")]
    [SerializeField] private string[] _femaleHairWardropeTypes = { "Hair", "Eyebrows" };
    [SerializeField] private string[] _maleHairWardropeTypes = { "Hair", "Eyebrows", "Beard" };
    [SerializeField] private string[] _femaleClothesWardropeTypes = { "Underwear"};
    [SerializeField] private string[] _maleClothesWardropeTypes = { "Underwear"};

    [Header("UMA Female Recipes")]
    [SerializeField] private List<UMATextRecipe> _femaleHairRecipes = new List<UMATextRecipe>();
    [SerializeField] private List<UMATextRecipe> _femaleEyebrowsRecipes = new List<UMATextRecipe>();
    [SerializeField] private List<UMATextRecipe> _femaleUnderwearRecipes = new List<UMATextRecipe>();

    [Header("UMA Male Recipes")]
    [SerializeField] private List<UMATextRecipe> _maleHairRecipes = new List<UMATextRecipe>();
    [SerializeField] private List<UMATextRecipe> _maleEyebrowsRecipes = new List<UMATextRecipe>();
    [SerializeField] private List<UMATextRecipe> _maleBeardRecipes = new List<UMATextRecipe>();
    [SerializeField] private List<UMATextRecipe> _maleUnderwearRecipes = new List<UMATextRecipe>();

    // Private variables user interface
    private List<string> raceOptions = new List<string>();
    private List<GameObject> headerElements = new List<GameObject>();

    private List<GameObject> _activeColorObjects = new List<GameObject>();
    
    
    // Private variables network
    private string charname;
    private string sex;
    private string race;
    private string model;

    // Private variables helper/general
    private bool initalized = false;

    private GameObject _heightSlider;
    private GameObject _headSliderGroup = null;
    private GameObject _upperBodySliderGroup = null;
    private GameObject _lowerBodySliderGroup = null;
    private GameObject _hairGroup = null;
    private GameObject _clothesGroup = null;

    private void Awake()
    {
        avatar = GameManager.Instance.Avatar;

        sex = "male";

        raceOptions.Add("Select race");
        raceOptions.Add("Humanoid");
        dropdownRace.AddOptions(raceOptions);
    }

    private void Start()
    {
        umaData = avatar.umaData;
    }

    private void OnDestroy()
    {
        GameManager.Instance.CharacterCreatedAndReadyToPlay = false;
        GameManager.Instance.CharacterId = 0;
        if(buttonPlay.gameObject.activeSelf)
            buttonPlay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!initalized) 
        {
            // To initialize the HumanMale Race with the default CharacterTemplate
            OnButtonMale();

            initalized = true;
        }
        
        if(GameManager.Instance.CharacterCreatedAndReadyToPlay && !buttonPlay.gameObject.activeSelf)
        {
            buttonPlay.gameObject.SetActive(true);
        }
    }

    private void CreateSkinColor()
    {
        foreach (OverlayColorData colorType in avatar.CurrentSharedColors)
        {
            if (colorType.name == "Skin")
            {
                GameObject goColor = Instantiate(_prefabColorElement, skinHome);
                _activeColorObjects.Add(goColor);
                ColorElement color = goColor.GetComponent<ColorElement>();
                color.Initialize(avatar, colorType, "Skin", allwaysOnTop);
            }
        }
    }

    private void CreateHeightSlider()
    {
        UMADnaBase[] DNA = avatar.GetAllDNA();

        foreach(UMADnaBase dna in DNA)
        {
            string[] names = dna.Names;
            float[] values = dna.Values;

            List<string> categories = new List<string>();
            categories = CreateCategoryList(names, _heightDNA);
            
            foreach(string category in categories)
            {
                _heightSlider = Instantiate(_prefabSliderElement, heightHome);
                SliderElement heightSlider = _heightSlider.GetComponent<SliderElement>();
                    
                for (int i = 0; i < names.Length; i++)
                {
                    if (category == GetCategory(names[i].BreakupCamelCase()))
                    {
                        heightSlider.InitializeSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, avatar, dna, true, 150f);
                        break;
                    }
                }
            }
        }
    }
    
    #region CHARACTER MODIFIER BUTTONS

    private void CreateModifierButtons()
    {
        GameObject ph = Instantiate(_prefabTitleElement, _modifiersButtonHome);
        ph.name = "Character modifiers";
        headerElements.Add(ph);
        TitleElement he = ph.GetComponent<TitleElement>();
        string headerName = "Character Modifiers";
        he.InitializeHeaderElement(headerName, _modifiersButtonHome.GetComponent<RectTransform>());

        GameObject goHead = he.CreateObject(_prefabButtonElement, "Head");
        ButtonElement boHeader = goHead.GetComponent<ButtonElement>();
        boHeader.Initialize("Head", OnButtonHeadClick);

        GameObject goUpperBody = he.CreateObject(_prefabButtonElement, "Upper Body");
        ButtonElement boUpperBody = goUpperBody.GetComponent<ButtonElement>();
        boUpperBody.Initialize("Upper Body", OnButtonUpperBodyClick);

        GameObject goLowerBody = he.CreateObject(_prefabButtonElement, "Lower Body");
        ButtonElement boLowerBody = goLowerBody.GetComponent<ButtonElement>();
        boLowerBody.Initialize("Lower Body", OnButtonLowerBodyClick);

        GameObject goHair = he.CreateObject(_prefabButtonElement, "Hair");
        ButtonElement boHair = goHair.GetComponent<ButtonElement>();
        boHair.Initialize("Hair", OnButtonHairClick);

        GameObject goClothes = he.CreateObject(_prefabButtonElement, "Clothes");
        ButtonElement boClothes = goClothes.GetComponent<ButtonElement>();
        boClothes.Initialize("Clothes", OnButtonClothesClick);
    }

    private void OnButtonHeadClick()
    {
        if(_headSliderGroup == null)
        {
            _headSliderGroup = CreateSlidersGroup("Head", _headDNA);
        }
        else
            Destroy(_headSliderGroup);
    }

    private void OnButtonUpperBodyClick()
    {
        if(_upperBodySliderGroup == null)
        {
            _upperBodySliderGroup = CreateSlidersGroup("Upper Body", _upperBodyDNA);
        }
        else 
            Destroy(_upperBodySliderGroup);
    }

    private void OnButtonLowerBodyClick()
    {
        if(_lowerBodySliderGroup == null)
        {
            _lowerBodySliderGroup = CreateSlidersGroup("Lower Body", _lowerBodyDNA);
        }
        else
            Destroy(_lowerBodySliderGroup);
    }
    
    private void OnButtonHairClick()
    {
        if(_hairGroup == null)
        {
            if (avatar.activeRace.name == "HumanMale")
            {
                List<UMATextRecipe>[] recipesToShow = new List<UMATextRecipe>[3];
                recipesToShow[0] = _maleHairRecipes;
                recipesToShow[1] = _maleEyebrowsRecipes;
                recipesToShow[2] = _maleBeardRecipes;
                _hairGroup = CreateWardrobeGroup("Hair", _maleHairWardropeTypes, recipesToShow);
            }
            else
            {
                List<UMATextRecipe>[] recipesToShow = new List<UMATextRecipe>[2];
                recipesToShow[0] = _femaleHairRecipes;
                recipesToShow[1] = _femaleEyebrowsRecipes;

                _hairGroup = CreateWardrobeGroup("Hair", _femaleHairWardropeTypes, recipesToShow);
            }
        }
        else
            Destroy(_hairGroup);
    }

    private void OnButtonClothesClick()
    {
        if (_clothesGroup == null)
        {
            if (avatar.activeRace.name == "HumanMale")
            {
                List<UMATextRecipe>[] recipesToShow = new List<UMATextRecipe>[1];
                recipesToShow[0] = _maleUnderwearRecipes;

                _clothesGroup = CreateWardrobeGroup("Clothes", _maleClothesWardropeTypes, recipesToShow);
            }
            else
            {
                List<UMATextRecipe>[] recipesToShow = new List<UMATextRecipe>[1];
                recipesToShow[0] = _femaleUnderwearRecipes;

                _clothesGroup = CreateWardrobeGroup("Clothes", _femaleClothesWardropeTypes, recipesToShow);
            }
        }
        else
            Destroy(_clothesGroup);
    }

    private GameObject CreateSlidersGroup(string title, List<string> dnaToShow)
    {
        GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersHome);
        gotitle.name = title;
        headerElements.Add(gotitle);

        TitleElement te = gotitle.GetComponent<TitleElement>();
        te.InitializeHeaderElement(title, _modifiersHome.GetComponent<RectTransform>());

        RectTransform parentLayout = te.GetParentLayout();
        RectTransform layout = te.GetLayout();

        UMADnaBase[] DNA = avatar.GetAllDNA();

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
                        so.InitializeSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, avatar, dna);
                    }
                }
            }
        }

        return gotitle;
    }

    private GameObject CreateWardrobeGroup(string title, string[] wardrobeType, List<UMATextRecipe>[] recipesToShow)
    {
        GameObject gotitle = Instantiate(_prefabTitleElement, _modifiersHome);
        gotitle.name = title;
        headerElements.Add(gotitle);

        TitleElement te = gotitle.GetComponent<TitleElement>();
        te.InitializeHeaderElement(title, _modifiersHome.GetComponent<RectTransform>());

        Dictionary<string, List<UMATextRecipe>> recipes = avatar.AvailableRecipes;

        foreach (string r in recipes.Keys)
        {
            int i = 0;
            foreach(string t in wardrobeType)
            {
                if (r == wardrobeType[i])
                {
                    GameObject go = te.CreateObject(_prefabWardrobeElement, r);
                    go.GetComponent<WardrobeObject>().InitializeWardrobe(avatar, r, allwaysOnTop, recipesToShow, true);
                }
                i++;
            }
        }

        return gotitle;
    }

    #endregion

    private void CreateColorPicker()
    {
        /*
        prefabs.Add("ColorPickerObject", prefabColorPickerObject);

        foreach(OverlayColorData colorType in avatar.CurrentSharedColors)
        {
            GameObject ph = Instantiate(prefabHeader, colorHome);
            ph.name = colorType.name;
            headerElements.Add(ph);

            HeaderElement he = ph.GetComponent<HeaderElement>();
            string name = colorType.name + " Color";
            he.InitializeHeaderElement(name, prefabs, colorHome.GetComponent<RectTransform>());
            GameObject go = he.CreateObject("ColorPickerObject", colorType.name);
            if(go != null)
                go.GetComponent<ColorPickerObject>().ColorData = colorType;
        }
        */
    }

    private void CreateWardrobe()
    {
        Dictionary<string, List<UMATextRecipe>> recipes = avatar.AvailableRecipes;

        GameObject ph = Instantiate(_prefabTitleElement, _modifiersButtonHome);
        ph.name = "Wardrobe";
        headerElements.Add(ph);

        TitleElement he = ph.GetComponent<TitleElement>();
        string name = "Warderobe";
        he.InitializeHeaderElement(name, _modifiersButtonHome.GetComponent<RectTransform>());


        foreach (string s in recipes.Keys)
        {
            GameObject go = he.CreateObject(_prefabWardrobeElement, s);
            go.GetComponent<WardrobeObject>().InitializeWardrobe(avatar, s, allwaysOnTop);
        }
    }

    private void RemoveSkinColor()
    {

    }

    private void RemoveHeightSlider()
    {
        Destroy(_heightSlider);
    }

    private void RemoveModifierButtons()
    {
        Destroy(_headSliderGroup);
        Destroy(_upperBodySliderGroup);
        Destroy(_lowerBodySliderGroup);
    }

    private void RemoveColorObjects()
    {
        foreach(GameObject color in _activeColorObjects)
        {
            ColorElement co = color.GetComponent<ColorElement>();
            co.RemoveColorPicker();
            Destroy(color);
        }

        _activeColorObjects.Clear();
    }

    private void RemoveColorPicker()
    {
        foreach (GameObject headerElement in headerElements)
        {
            TitleElement he = headerElement.GetComponent<TitleElement>();
            he.DestroyObjects();
            Destroy(headerElement);
        }
        headerElements.Clear();
    }

    private void RemoveWardrobe()
    {
    }

    private List<string> CreateCategoryList(string[] names, List<string> toShow)
    {
        var list = new List<string>();

        for (int i = 0; i < names.Length; i++)
        {
            if (toShow.Contains(names[i]) )
            {
                string cat = GetCategory(names[i].BreakupCamelCase());
                
                if(!list.Contains(cat))
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

        if(cat.Length == 3)
            return cat[1] +" "+ cat[2];
        else if(cat.Length == 2)
            return cat[1];
        else
            return cat[0];
    }

    private void CreateCharacterModifiers()
    {
        CreateSkinColor();
        CreateHeightSlider();
        CreateModifierButtons();
        CreateWardrobe();

        CreateColorPicker();
    }

    private void RemoveCharacterModifiers()
    {
        RemoveSkinColor();
        RemoveHeightSlider();
        RemoveModifierButtons();


        RemoveColorPicker();
        RemoveWardrobe();
        RemoveColorObjects();
    }

    #region BUTTON HANDLER

    public void OnButtonMale()
    {
        if (avatar.activeRace.name != "HumanMale" || !initalized)
        {
            buttonMale.interactable = false;
            buttonFemale.interactable = false;

            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnMaleCharacterUpdated));
            avatar.ChangeRace("HumanMale", true);

            RemoveCharacterModifiers();

            sex = "male";
        }
    }

    public void OnButtonFemale()
    {
        if (avatar.activeRace.name != "HumanFemale")
        {
            buttonMale.interactable = false;
            buttonFemale.interactable = false;

            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnFemaleCharacterUpdated));
            avatar.ChangeRace("HumanFemale", true);

            RemoveCharacterModifiers();

            sex = "female";
        }
    }

    public void OnButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
    }

    public void OnButtonCreate()
    {
        model = UMAHelper.GetAvatarString(avatar);
        charname = inputFieldNameValue.text;
        race = raceOptions[dropdownRace.value];

        NetworkManagerGame.Instance.CreateCharacter(charname, sex, race, model);
    }

    public void OnButtonPlay()
    {
        NetworkManagerGame.Instance.PlayRequest(GameManager.Instance.CharacterId);
    }

    #endregion

    #region UMA EVENT HANDLER

    public void OnFemaleCharacterUpdated(UMAData data)
    {
        Debug.Log("OnFemaleCharacterUpdated");
        data.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnFemaleCharacterUpdated));

        UMAHelper.SetAvatarString(avatar, characterTemplates[1].UmaCharacterString);
        CreateCharacterModifiers();

        buttonMale.interactable = true;
        buttonFemale.interactable = true;
    }

    public void OnMaleCharacterUpdated(UMAData data)
    {
        Debug.Log("OnMaleCharacterUpdated");
        data.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnMaleCharacterUpdated));

        UMAHelper.SetAvatarString(avatar, characterTemplates[0].UmaCharacterString);
        CreateCharacterModifiers();

        buttonMale.interactable = true;
        buttonFemale.interactable = true;
    }

    #endregion
}
