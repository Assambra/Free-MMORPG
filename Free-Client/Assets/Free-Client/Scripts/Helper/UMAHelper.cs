using UMA.CharacterSystem;
using UnityEngine;

public static class UMAHelper 
{
    private static DynamicCharacterAvatar avatar;


    public static string GetAvatarString(DynamicCharacterAvatar avatar)
    {
        string model = avatar.GetAvatarDefinition(true).ToCompressedString("|");
        return model;
    }

    public static void SetAvatarString(DynamicCharacterAvatar avatar, string model)
    {
        AvatarDefinition adf = AvatarDefinition.FromCompressedString(model, '|');
        avatar.LoadAvatarDefinition(adf);
        avatar.BuildCharacter(false);
    }
}
