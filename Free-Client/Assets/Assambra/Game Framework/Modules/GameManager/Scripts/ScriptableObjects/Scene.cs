using Assambra.GameFramework.GameManager;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Assambra/Scene", order = 1)]
public class Scene : ScriptableObject
{
    public string ScenePath;

    public bool IsFirstScene = false;

    [SerializeField] public List<SceneUISet> SceneUISets = new List<SceneUISet>();
}
