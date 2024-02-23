using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assambra.GameFramework.GameManager
{
    public class SceneHandler : MonoBehaviour
    {
        public static event Action<Scene, Scene> OnSceneChanged;

        public List<Scene> Scenes = new List<Scene>();

        private Scene lastScene = null;

        private void Start()
        {
            ChangeScene(GetFirstScene());
        }

        public void ChangeScene(Scene newScene)
        {
            if (lastScene != newScene)
            {
                OnSceneChanged?.Invoke(lastScene, newScene);
                UnloadScenesInSceneObject(lastScene);
                LoadScenesInSceneObject(newScene);
                lastScene = newScene;
            }
        }

        private void LoadScenesInSceneObject(Scene scene)
        {
            if (scene != null)
            {
                foreach (string scenePath in scene.scenePaths)
                {
                    LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                }
            }
        }

        private void UnloadScenesInSceneObject(Scene scene)
        {
            if (scene != null)
            {
                foreach (string scenePath in scene.scenePaths)
                {
                    UnloadSceneAsync(scenePath);
                }
            }
        }

        private Scene GetFirstScene()
        {
            Debug.Log(Scenes.Find(scene => scene.IsFirstScene).name);
            return Scenes.Find(scene => scene.IsFirstScene);
        }

        private void LoadSceneAsync(string scene, LoadSceneMode mode)
        {
            SceneManager.LoadSceneAsync(scene, mode);
        }

        private void UnloadSceneAsync(string scene)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }
}
