using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UMA.CharacterSystem;
using UMA;

public class UICreateCharacter : MonoBehaviour
{
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
    

    // Private variables UMA
    //private GameObject umaCharacter;
    private DynamicCharacterAvatar avatar;
    private UMAData umaData;

    // Private variables user interface
    private List<string> raceOptions = new List<string>();
    private List<GameObject> sliderGroups = new List<GameObject>();
    private List<string> categories = new List<string>();
    private List<string> excludeDna = new List<string>();
    [SerializeField] private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    
    // Private variables network
    private string charname;
    private string sex;
    private string race;
    private string model;

    // Private variables helper/general
    private bool isInitialized = false;

    

    private void Awake()
    {
        avatar = GameManager.Instance.Avatar;
        umaData = avatar.umaData;

        sex = "male";

        raceOptions.Add("Select race");
        raceOptions.Add("Humanoid");
        dropdownRaceValue.AddOptions(raceOptions);

        excludeDna.Add("skinGreenness");
        excludeDna.Add("skinBlueness");
        excludeDna.Add("skinRedness");
    }

    private void OnEnable()
    {
        CameraController cc = GameManager.Instance.cameraController;
        if(cc.GetCameraPanAngle() != -180f)
        {
            cc.SetCameraPan(-180f);
        }
    }

    private void Update()
    {
        if (!isInitialized) 
        {
            isInitialized = true;
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterUpdated));
            avatar.ChangeRace("HumanMale", true);
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


        foreach(OverlayColorData colorType in avatar.characterColors.Colors)
        {
            GameObject ph = Instantiate(prefabHeader, colorHome);
            ph.name = colorType.name;
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

    public void OnButtonMale()
    {
        if (avatar.activeRace.name != "HumanMale")
        {
            sex = "male";
            avatar.ChangeRace("HumanMale", true);
            RemoveSliders();
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterUpdated));
        }
    }

    public void OnButtonFemale()
    {
        if(avatar.activeRace.name != "HumanFemale")
        {
            sex = "female";
            avatar.ChangeRace("HumanFemale", true);
            RemoveSliders();
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterUpdated));
        }
    }

    public void ButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
    }

    public void ButtonCreate()
    {
        model = UMAHelper.GetAvatarString(avatar);
        charname = inputFieldNameValue.text;
        race = raceOptions[dropdownRaceValue.value];

        NetworkManager.Instance.CreateCharacter(charname, sex, race, model);
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

    public void OnCharacterUpdated(UMAData data)
    {
        data.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnCharacterUpdated));

        CreateSliders();
        CreateColorPicker();
    }
}
