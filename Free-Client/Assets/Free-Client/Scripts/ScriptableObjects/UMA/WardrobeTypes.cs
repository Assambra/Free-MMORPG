using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeClient.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WardrobeTypes", menuName = "Assambra/UMA/WardrobeTypes", order = 1)]
    public class WardrobeTypes : ScriptableObject
    {
        public string Title;
        public List<WardrobeType> wardrobeTypes = new List<WardrobeType>();
    }
}
