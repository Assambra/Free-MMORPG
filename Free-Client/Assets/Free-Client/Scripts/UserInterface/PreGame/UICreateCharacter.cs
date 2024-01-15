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
    [SerializeField] TMP_Dropdown dropdownRaceValue;
    [SerializeField] GameObject prefabSliderGroup;
    [SerializeField] GameObject prefabHeader;
    [SerializeField] GameObject prefabColorPickerObject;
    [SerializeField] Transform groupeHome;
    [SerializeField] Transform colorHome;
    [SerializeField] RectTransform sliderLayout;
    [SerializeField] RectTransform colorLayout;
    [SerializeField] Button buttonFemale;
    [SerializeField] Button buttonMale;
    [SerializeField] Button buttonPlay;

    // Private variables UMA
    private DynamicCharacterAvatar avatar;
    private UMAData umaData;

    [Header("Default Character templates")]
    [SerializeField] private List<CharacterTemplate> characterTemplates = new List<CharacterTemplate>();

    [Header("Camera auto focus points")]
    [SerializeField] private List<CameraFocusPoint> cameraAutoFocusPoints = new List<CameraFocusPoint>();

    // Private variables user interface
    private List<string> raceOptions = new List<string>();
    private List<GameObject> sliderGroups = new List<GameObject>();
    private List<GameObject> headerElements = new List<GameObject>();
    private List<string> categories = new List<string>();
    private List<string> excludeDna = new List<string>();
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
        dropdownRaceValue.AddOptions(raceOptions);

        excludeDna.Add("skinGreenness");
        excludeDna.Add("skinBlueness");
        excludeDna.Add("skinRedness");
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

    private void CreateSliders()
    {
        Dictionary<string, float> defaultDNA = avatar.activeRace.data.GetDefaultDNA();

        UMADnaBase[] DNA = avatar.GetAllDNA();

        foreach(UMADnaBase d in DNA)
        {
            string[] names = d.Names;
            float[] values = d.Values;

            CreateCategoryList(names);
            
            foreach(string category in categories)
            {
                GameObject go = Instantiate(prefabSliderGroup, groupeHome);
                go.name = category;
                sliderGroups.Add(go);
                SliderGroup group = go.GetComponent<SliderGroup>();
                group.SetGroupName(category);
                group.CreateCharacterLayoutGroup = sliderLayout;
                
                for (int i = 0; i < names.Length; i++)
                {
                    if(category == GetCategory(names[i].BreakupCamelCase()))
                    {
                        group.CreateSlider(GetSlider(names[i].BreakupCamelCase()), names[i], values[i], i, avatar, d);
                    }
                }
            }
        }
    }

    private void CreateColorPicker()
    {
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
    }

    public void SetColor(string colorName, Color basecolor, Color metalliccolor, float gloss)
    {
        avatar.SetColor(colorName, basecolor, Gloss: gloss, UpdateTexture: true);
    }

    private void RemoveSliders()
    {
        foreach(GameObject sliderGroup in sliderGroups)
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

    public void OnButtonMale()
    {
        if (avatar.activeRace.name != "HumanMale" || !initalized)
        {
            buttonMale.interactable = false;
            buttonFemale.interactable = false;

            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnMaleCharacterUpdated));
            avatar.ChangeRace("HumanMale", true);

            RemoveSliders();
            RemoveColorPicker();

            sex = "male";
        }
    }

    public void OnButtonFemale()
    {
        if(avatar.activeRace.name != "HumanFemale")
        {
            buttonMale.interactable = false;
            buttonFemale.interactable = false;

            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnFemaleCharacterUpdated));
            avatar.ChangeRace("HumanFemale", true);

            RemoveSliders();
            RemoveColorPicker();

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
        race = raceOptions[dropdownRaceValue.value];

        NetworkManagerGame.Instance.CreateCharacter(charname, sex, race, model);
    }

    public void OnButtonPlay()
    {
        NetworkManagerGame.Instance.PlayRequest(GameManager.Instance.CharacterId);
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

    private void CreateCharacterModifiers()
    {
        CreateSliders();
        CreateColorPicker();
    }
}
