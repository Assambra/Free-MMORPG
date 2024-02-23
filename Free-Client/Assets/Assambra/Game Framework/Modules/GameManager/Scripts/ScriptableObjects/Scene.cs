using UnityEngine;
using System.Collections.Generic;

namespace Assambra.GameFramework.GameManager
{
    [CreateAssetMenu(fileName = "Scene", menuName = "Assambra/Scene", order = 1)]
    public class Scene : ScriptableObject
    {
        public bool IsFirstScene = false;

        public string[] scenePaths;

        public List<SceneUISet> SceneUISets = new List<SceneUISet>();
    }
}
