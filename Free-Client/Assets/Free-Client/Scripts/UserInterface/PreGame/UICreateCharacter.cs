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
    [SerializeField] GameObject prefabSliderGroup;
    [SerializeField] GameObject prefabHeader;
    [SerializeField] GameObject prefabSliderObject;
    [SerializeField] GameObject prefabColorPickerObject;
    [SerializeField] GameObject prefabWardrobeObject;
    [SerializeField] GameObject prefabColorObject;
    [SerializeField] GameObject prefabButtonObject;
    [SerializeField] Transform groupeHome;
    [SerializeField] Transform colorHome;
    [SerializeField] RectTransform sliderLayout;
    [SerializeField] RectTransform colorLayout;

    [SerializeField] Button buttonPlay;

    // Private variables UMA
    private DynamicCharacterAvatar avatar;
    private UMAData umaData;

    [Header("Default Character templates")]
    [SerializeField] private List<CharacterTemplate> characterTemplates = new List<CharacterTemplate>();

    [Header("Camera auto focus points")]
    [SerializeField] private List<CameraFocusPoint> cameraAutoFocusPoints = new List<CameraFocusPoint>();

    [Header("UMA")]
    [SerializeField] private List<string> excludeDna = new List<string>();

    // Private variables user interface
    private List<string> raceOptions = new List<string>();
    private List<GameObject> sliderGroups = new List<GameObject>();
    private List<GameObject> headerElements = new List<GameObject>();
    private List<string> categories = new List<string>();

    private List<GameObject> _activeColorObjects = new List<GameObject>();
    private GameObject _heightSlider;

    [SerializeField] private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    
    // Private variables network
    private string charname;
    private string sex;
    private string race;
    private string model;

    // Private variables helper/general
    private bool initalized = false;

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

    private void CreateLeftPanelHeaders()
    {
        prefabs.Add("ButtonObject", prefabButtonObject);

        GameObject ph = Instantiate(prefabHeader, colorHome);
        ph.name = "Character modifiers";
        headerElements.Add(ph);
        HeaderElement he = ph.GetComponent<HeaderElement>();
        string headerName = "Character Modifiers";
        he.InitializeHeaderElement(headerName, prefabs, colorHome.GetComponent<RectTransform>());

        GameObject goHead = he.CreateObject("ButtonObject", "Head");
        ButtonObject boHeader = goHead.GetComponent<ButtonObject>();
        boHeader.Initialize("Head", OnButtonHeadClick);

        GameObject goUpperBody = he.CreateObject("ButtonObject", "Upper Body");
        ButtonObject boUpperBody = goUpperBody.GetComponent<ButtonObject>();
        boUpperBody.Initialize("Upper Body", OnButtonUpperBodyClick);

        GameObject goLowerBody = he.CreateObject("ButtonObject", "Lower Body");
        ButtonObject boLowerBody = goLowerBody.GetComponent<ButtonObject>();
        boLowerBody.Initialize("Lower Body", OnButtonLowerBodyClick);
    }

    private void OnButtonHeadClick()
    {
        Debug.Log("Head button clicked");
    }

    private void OnButtonUpperBodyClick()
    {
        Debug.Log("UpperBody button clicked");
    }

    private void OnButtonLowerBodyClick()
    {
        Debug.Log("LowerBody button clicked");
    }

    private void CreateSliders()
    {
        UMADnaBase[] DNA = avatar.GetAllDNA();

        foreach(UMADnaBase dna in DNA)
        {
            string[] names = dna.Names;
            float[] values = dna.Values;

            CreateCategoryList(names);
            
            foreach(string category in categories)
            {
                if(category == "Height")
                {
                    GameObject go = Instantiate(prefabSliderObject, heightHome);
                    _heightSlider = go;
                    SliderObject heightSlider = go.GetComponent<SliderObject>();
                    
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (category == GetCategory(names[i].BreakupCamelCase()))
                        {
                            heightSlider.InitializeSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, avatar, dna, true, 150f);
                            break;
                        }
                    }
                }
                else
                {
                    GameObject go = Instantiate(prefabSliderGroup, groupeHome);
                    go.name = category;
                    sliderGroups.Add(go);
                    SliderGroup group = go.GetComponent<SliderGroup>();
                    group.SetGroupName(category);
                    group.CreateCharacterLayoutGroup = sliderLayout;

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (category == GetCategory(names[i].BreakupCamelCase()))
                        {
                            group.CreateSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, avatar, dna);
                        }
                    }
                }
            }
        }
    }

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

    private void CreateColors()
    {
        foreach(OverlayColorData colorType in avatar.CurrentSharedColors)
        {
            if(colorType.name == "Skin")
            {
                GameObject goColor = Instantiate(prefabColorObject, skinHome);
                _activeColorObjects.Add(goColor);
                ColorObject color = goColor.GetComponent<ColorObject>();
                color.Initialize(avatar, colorType, "Skin Color", allwaysOnTop);
            }
        }
    }

    private void CreateWardrobe()
    {
        prefabs.Add("Wardrobe", prefabWardrobeObject);
        Dictionary<string, List<UMATextRecipe>> recipes = avatar.AvailableRecipes;

        GameObject ph = Instantiate(prefabHeader, colorHome);
        ph.name = "Wardrobe";
        headerElements.Add(ph);

        HeaderElement he = ph.GetComponent<HeaderElement>();
        string name = "Warderobe";
        he.InitializeHeaderElement(name, prefabs, colorHome.GetComponent<RectTransform>());
        

        foreach (string s in recipes.Keys)
        {
            GameObject go = he.CreateObject("Wardrobe", s);
            go.GetComponent<WardrobeObject>().InitializeWardrobe(avatar, s);
        }
    }

    private void RemoveColorObjects()
    {
        foreach(GameObject color in _activeColorObjects)
        {
            ColorObject co = color.GetComponent<ColorObject>();
            co.RemoveColorPicker();
            Destroy(color);
        }

        _activeColorObjects.Clear();
    }

    private void RemoveSliders()
    {
        Destroy(_heightSlider);

        foreach (GameObject sliderGroup in sliderGroups)
        {
            SliderGroup group = sliderGroup.GetComponent<SliderGroup>();
            group.DestroySliders();
            Destroy(sliderGroup);
        }

        sliderGroups.Clear();
    }

    private void RemoveColorPicker()
    {
        prefabs.Remove("ColorPickerObject");

        foreach (GameObject headerElement in headerElements)
        {
            HeaderElement he = headerElement.GetComponent<HeaderElement>();
            he.DestroyObjects();
            Destroy(headerElement);
        }
        headerElements.Clear();
    }

    private void RemoveWardrobe()
    {
        prefabs.Remove("Wardrobe");

        //We use Header elements
    }

    private void CreateCategoryList(string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            string cat = GetCategory(names[i].BreakupCamelCase());
 
            if(!categories.Contains(cat) && !excludeDna.Contains(names[i]))
                categories.Add(cat);
        }   
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
        CreateLeftPanelHeaders();

        CreateColors();
        CreateSliders();
        CreateColorPicker();
        CreateWardrobe();
    }

    private void RemoveCharacterModifiers()
    {
        RemoveSliders();
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
