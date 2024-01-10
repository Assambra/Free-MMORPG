using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] SceneHandler sceneHandler;
    [SerializeField] public Transform Canvas;

    private List<GameObject> uIElements = new List<GameObject>();

    private void OnEnable()
    {
        SceneHandler.OnSceneChanged += SceneChanged;
    }

    private void OnDisable()
    {
        SceneHandler.OnSceneChanged -= SceneChanged;
    }

    private void SceneChanged(Scene lastScenen, Scene newScene)
    {
        HashSet<string> lastUIElements = new HashSet<string>();

        if (lastScenen != null)
        {
            lastUIElements = lastScenen.SceneUISets
                .SelectMany(set => set.UIElementPrefabs)
                .Select(obj => obj.name)
                .ToHashSet();
        }

        HashSet<string> newUIElements = newScene.SceneUISets
            .SelectMany(set => set.UIElementPrefabs)
            .Select(obj => obj.name)
            .ToHashSet();

        foreach (var uIElement in uIElements.ToList())
        {
            if (!newUIElements.Contains(uIElement.name))
            {
                Destroy(uIElement);
                uIElements.Remove(uIElement);
            }
        }

        foreach (var sceneUISet in newScene.SceneUISets)
        {
            foreach (var obj in sceneUISet.UIElementPrefabs)
            {
                if (lastScenen == null || !lastUIElements.Contains(obj.name))
                {
                    InstantiateCurrentSceneUI(obj);
                }
            }
        }
    }

    private void InstantiateCurrentSceneUI(GameObject obj)
    {
        GameObject go = Instantiate(obj, Canvas);
        go.name = obj.name;
        uIElements.Add(go);
    }
}
