using System.Collections.Generic;
using UMA;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Assambra/UMA/Recipes", order = 1)]
public class SO_Recipes : ScriptableObject
{
    public List<UMATextRecipe> Recipes = new List<UMATextRecipe>();
}
