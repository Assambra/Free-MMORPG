using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public List<Scene> Scenes = new List<Scene>();
    public Scene CurrentScene = null;
    public Scene LastScene = null;

    public delegate void SceneAction();
    public static event SceneAction OnSceneChanged;

    private void Awake()
    {
        foreach (Scene scene in Scenes)
        {
            if (scene.IsFirstScene)
                CurrentScene = scene;
        }
    }

    void Update()
    {
        if (CurrentScene != LastScene)
        {
            OnSceneChanged?.Invoke();

            foreach (Scene scene in Scenes)
            {
                if (scene == CurrentScene)
                    LoadSceneAsync(scene.ScenePath, LoadSceneMode.Additive);
            }

            if (LastScene != null)
                UnloadSceneAsync(LastScene.ScenePath);

            LastScene = CurrentScene;
        }
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
