using System;
using System.Collections;
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

        private bool loadingInProgress = false;

        private void Start()
        {
            ChangeScene(GetFirstScene());
        }

        public void ChangeScene(Scene newScene)
        {
            if (IsLoadingInProgress())
                StartCoroutine(WaitLoadInProgressCoroutine(newScene));
            else
            {
                if (lastScene != newScene)
                {
                    OnSceneChanged?.Invoke(lastScene, newScene);

                    if (lastScene != null)
                        UnloadSceneAsync(lastScene.ScenePath);

                    LoadSceneAsync(newScene.ScenePath, LoadSceneMode.Additive);
                    lastScene = newScene;
                }
            }
        }

        private bool IsLoadingInProgress()
        {
            return loadingInProgress;
        }

        private IEnumerator WaitLoadInProgressCoroutine(Scene newScene)
        {
            yield return new WaitUntil(() => !loadingInProgress);
            ChangeScene(newScene);
        }

        public Scenes GetScenesByName(string scenename)
        {
            Scenes scene = (Scenes)Enum.Parse(typeof(Scenes), scenename);

            return scene;
        }

        private Scene GetFirstScene()
        {
            Debug.Log(Scenes.Find(scene => scene.IsFirstScene).name);
            return Scenes.Find(scene => scene.IsFirstScene);
        }

        private void LoadSceneAsync(string scene, LoadSceneMode mode)
        {
            loadingInProgress = true;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, mode);
            asyncOperation.completed += (asyncOperation) =>
            {
                loadingInProgress = false;
            };
        }

        private void UnloadSceneAsync(string scene)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }
}
