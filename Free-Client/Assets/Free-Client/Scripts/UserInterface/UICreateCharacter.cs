using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UICreateCharacter : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldNameValue;
    [SerializeField] TMP_Dropdown dropdownRaceValue;
    [SerializeField] TMP_InputField inputFieldModelValue;

    private string charname;
    private string sex;
    private string race;
    private string model;

    private List<string> raceOptions = new List<string>();

    private void OnEnable()
    {
        raceOptions.Add("Select race");
        raceOptions.Add("Humanoid");

        dropdownRaceValue.AddOptions(raceOptions);
    }

    public void OnButtonMale()
    {
        sex = "male";
    }

    public void OnButtonFemale()
    {
        sex = "female";
    }

    public void ButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
    }

    public void ButtonCreate()
    {
        charname = inputFieldNameValue.text;
        model = inputFieldModelValue.text;
        race = raceOptions[dropdownRaceValue.value];

        NetworkManager.Instance.CreateCharacter(charname, sex, race, model);
    }
}
