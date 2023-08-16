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
        characterInfos = GameManager.Instance.characterInfos;

        if (characterInfos.Count > 0)
        {
            umaCharacter = GameObject.Instantiate(uMADynamicCharacterAvatar, new Vector3(0, 0, 0), Quaternion.identity);
            avatar = umaCharacter.GetComponent<DynamicCharacterAvatar>();

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

    private void OnDisable()
    {
        if(umaCharacter != null)
            Destroy(umaCharacter);
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

    public void OnButtonQuit()
    {
        NetworkManager.Instance.Disconnect();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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
