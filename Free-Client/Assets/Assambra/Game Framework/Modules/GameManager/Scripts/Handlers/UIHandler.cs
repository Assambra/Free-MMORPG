using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] SceneHandler sceneHandler;
    [SerializeField] private Transform canvas;

    private List<GameObject> uIElements = new List<GameObject>();

    private void OnEnable()
    {
        SceneHandler.OnSceneChanged += SceneChanged;
    }

    private void OnDisable()
    {
        SceneHandler.OnSceneChanged -= SceneChanged;
    }

    private void SceneChanged()
    {
        bool isTheSameSet = false;

        if (sceneHandler.LastScene != null)
        {
            isTheSameSet = Enumerable.SequenceEqual(sceneHandler.LastScene.SceneUISets.OrderBy(e => e.name), sceneHandler.CurrentScene.SceneUISets.OrderBy(e => e.name));
            
            if (!isTheSameSet)
            {
                DestroyLastUIElements();
                uIElements.Clear();
            }
        }

        if (!isTheSameSet)
        {
            foreach (Scene scene in sceneHandler.Scenes)
            {
                if (scene == sceneHandler.CurrentScene)
                {
                    foreach (SceneUISet sceneUISet in scene.SceneUISets)
                    {
                        foreach (GameObject obj in sceneUISet.UIElementPrefabs)
                        {
                            InstantiateCurrentSceneUI(obj);
                        }
                    }
                    return;
                }
            }
        }
    }

    private void InstantiateCurrentSceneUI(GameObject obj)
    {
        GameObject go = Instantiate(obj, canvas);
        go.name = obj.name;
        uIElements.Add(go);
    }

    private void DestroyLastUIElements()
    {
        foreach (GameObject uIElement in uIElements)
        {
            Destroy(uIElement);
        }
    }
}
