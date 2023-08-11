using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UMA.CharacterSystem;

public class UICreateCharacter : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldNameValue;
    [SerializeField] TMP_Dropdown dropdownRaceValue;

    [SerializeField] GameObject uMADynamicCharacterAvatar;

    private string charname;
    private string sex;
    private string race;
    private string model;

    private List<string> raceOptions = new List<string>();

    private GameObject umaCharacter;
    private DynamicCharacterAvatar avatar;

    private void OnEnable()
    {
        umaCharacter = GameObject.Instantiate(uMADynamicCharacterAvatar, new Vector3(0, 0, 0), Quaternion.identity);

        avatar = umaCharacter.GetComponent<DynamicCharacterAvatar>();

        sex = "male";

        raceOptions.Add("Select race");
        raceOptions.Add("Humanoid");

        dropdownRaceValue.AddOptions(raceOptions);
    }

    private void OnDisable()
    {
        Destroy(umaCharacter);
    }

    public void OnButtonMale()
    {
        if (avatar.activeRace.name != "HumanMale")
        {
            avatar.ChangeRace("HumanMale", true);
        }
        sex = "male";
    }

    public void OnButtonFemale()
    {
        if(avatar.activeRace.name != "HumanFemale")
        {
            avatar.ChangeRace("HumanFemale", true);
        }
        sex = "female";
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
}
