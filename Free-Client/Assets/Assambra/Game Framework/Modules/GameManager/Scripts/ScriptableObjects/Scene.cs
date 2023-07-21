using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene", menuName = "Assambra/Scene", order = 1)]
public class Scene : ScriptableObject
{
    public string ScenePath;

    public bool IsFirstScene = false;

    public List<SceneUISet> SceneUISets = new List<SceneUISet>();
}
