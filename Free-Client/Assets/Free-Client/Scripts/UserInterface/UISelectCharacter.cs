using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISelectCharacter : MonoBehaviour
{
    [SerializeField] TMP_Text textCharacterNameValue;
    [SerializeField] TMP_Text textCharacterSexValue;
    [SerializeField] TMP_Text textCharacterRaceValue;
    [SerializeField] Button buttonPrevious;
    [SerializeField] Button buttonNext;

    private List<CharacterInfo> characterInfos;
    private int charactersCount;
    private int currentShownCharacter;

    private void OnEnable()
    {
        characterInfos = GameManager.Instance.characterInfos;

        textCharacterNameValue.text = string.Empty;
        textCharacterSexValue.text = string.Empty;
        textCharacterRaceValue.text = string.Empty;
        
        charactersCount = characterInfos.Count;
        currentShownCharacter = 0;
    }

    public void OnButtonPreviousCharacter()
    {
        if(currentShownCharacter == 0)
            currentShownCharacter = characterInfos.Count;
        else
            currentShownCharacter--;

        SetCharacter(currentShownCharacter);
    }

    public void OnButtonNextCharacter()
    {
        if (currentShownCharacter == charactersCount)
            currentShownCharacter = 0;
        else
            currentShownCharacter++;

        SetCharacter(currentShownCharacter);
    }

    public void OnButtonPlay()
    {
        Debug.LogError("Not implemented");
    }

    public void OnButtonNewCharacter()
    {
        GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
    }

    private void SetCharacter(int character)
    {
        CharacterInfo info = characterInfos[character];
        textCharacterNameValue.text = info.name;
        textCharacterSexValue.text = info.sex;
        textCharacterRaceValue.text = info.race;

        // Todo instantiate and set model
    }
}
