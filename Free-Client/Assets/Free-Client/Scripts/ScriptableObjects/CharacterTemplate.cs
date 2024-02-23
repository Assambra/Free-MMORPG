using UnityEngine;

namespace Assambra.FreeClient.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterTemplate", menuName = "Assambra/CharacterTemplate", order = 1)]
    public class CharacterTemplate : ScriptableObject
    {
        public string UmaCharacterString;
    }
}
