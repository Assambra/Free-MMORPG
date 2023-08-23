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

    private List<CharacterInfo> characterInfos;
    private int charactersCount;
    private int currentShownCharacter;

    //private GameObject umaCharacter;
    private DynamicCharacterAvatar avatar;

    private void OnEnable()
    {
        characterInfos = GameManager.Instance.characterInfos;

        if (characterInfos.Count > 0)
        {
            avatar = GameManager.Instance.Avatar;

            UMAHelper.SetAvatarString(avatar, characterInfos[0].model);
            
            currentShownCharacter = 0;
            
            SetCharacter(currentShownCharacter);
        }
        else
        {
            // Todo inform the player that no characters available
            Debug.Log("Todo inform the player that no characters available");
        }

        charactersCount = characterInfos.Count - 1;
    }

    public void OnButtonPreviousCharacter()
    {
        if(charactersCount >= 0)
        {
            if (currentShownCharacter == 0)
                currentShownCharacter = charactersCount;
            else
                currentShownCharacter--;

            SetCharacter(currentShownCharacter);
        }
    }

    public void OnButtonNextCharacter()
    {
        if (charactersCount >= 0)
        {
            if (currentShownCharacter == charactersCount)
                currentShownCharacter = 0;
            else
                currentShownCharacter++;

            SetCharacter(currentShownCharacter);
        }
    }

    public void OnButtonBackToLogin()
    {
        GameManager.Instance.ChangeScene(Scenes.Login);
        NetworkManager.Instance.Disconnect();
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
