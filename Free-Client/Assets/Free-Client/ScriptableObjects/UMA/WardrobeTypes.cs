using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WardrobeTypes", menuName = "Assambra/UMA/WardrobeTypes", order = 1)]
public class WardrobeTypes : ScriptableObject
{
    public string Title;
    public List<WardrobeType> wardrobeTypes = new List<WardrobeType>();
}
