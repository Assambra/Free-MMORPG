using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UMA.CharacterSystem;

public class UISelectCharacter : MonoBehaviour
{
    [SerializeField] TMP_Text textCharacterNameValue;
    [SerializeField] TMP_Text textCharacterSexValue;
    [SerializeField] TMP_Text textCharacterRaceValue;
    [SerializeField] Button buttonPrevious;
    [SerializeField] Button buttonNext;

    [SerializeField] GameObject uMADynamicCharacterAvatar;

    private List<CharacterInfo> characterInfos;
    private int charactersCount;
    private int currentShownCharacter;

    private GameObject umaCharacter;
    private DynamicCharacterAvatar avatar;

    private void OnEnable()
    {
        umaCharacter = GameObject.Instantiate(uMADynamicCharacterAvatar, new Vector3(0, 0, 0), Quaternion.identity);
        avatar = umaCharacter.GetComponent<DynamicCharacterAvatar>();

        characterInfos = GameManager.Instance.characterInfos;

        textCharacterNameValue.text = string.Empty;
        textCharacterSexValue.text = string.Empty;
        textCharacterRaceValue.text = string.Empty;
        
        charactersCount = characterInfos.Count-1;
        currentShownCharacter = 0;

        UMAHelper.SetAvatarString(avatar, characterInfos[0].model);

        SetCharacter(currentShownCharacter);
    }

    private void OnDisable()
    {
        Destroy(umaCharacter);
    }

    public void OnButtonPreviousCharacter()
    {
        if(currentShownCharacter == 0)
            currentShownCharacter = charactersCount;
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
        UMAHelper.SetAvatarString(avatar, info.model);
    }
}
