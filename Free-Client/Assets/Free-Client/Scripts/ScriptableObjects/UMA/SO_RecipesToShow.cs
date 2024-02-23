using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeClient.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RecipesToShow", menuName = "Assambra/UMA/RecipesToShow", order = 1)]
    public class SO_RecipesToShow : ScriptableObject
    {
        public List<SO_Recipes> RecipesToShow = new List<SO_Recipes>();
    }
}
