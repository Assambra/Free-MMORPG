using UMA;
using UMA.CharacterSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterTemplateBuilder : MonoBehaviour
{
    [SerializeField] private DynamicCharacterAvatar avatar;

    [Tooltip("Insert the CharacterTemplate file to write the uma character string")]
    [SerializeField] CharacterTemplate characterTemplate;

    [SerializeField] private string umaCharacterString;

    [SerializeField] private Button buttonSave;
    [SerializeField] private Button buttonLoad;

    private UMAData umaData;

    private bool doOnce = false;

    private void Update()
    {
        if(!doOnce)
        {
            doOnce = true;
            umaData = avatar.umaData;
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterUpdated));
        }
    }

    public void OnButtonSave()
    {
        characterTemplate.UmaCharacterString = UMAHelper.GetAvatarString(avatar);
        umaCharacterString = characterTemplate.UmaCharacterString;
        EditorUtility.SetDirty(characterTemplate);
    }

    public void OnButtonLoad()
    {
        buttonSave.interactable = false;
        buttonLoad.interactable = false;

        UMAHelper.SetAvatarString(avatar, characterTemplate.UmaCharacterString);
    }

    public void OnButtonClear()
    {
        characterTemplate.UmaCharacterString = "";
    }

    private void OnCharacterUpdated(UMAData data)
    {
        buttonSave.interactable = true;
        buttonLoad.interactable = true;

        Debug.Log("OnCharacterUpdated");
        umaCharacterString = UMAHelper.GetAvatarString(avatar);
        umaData.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnCharacterUpdated));
    }
}
