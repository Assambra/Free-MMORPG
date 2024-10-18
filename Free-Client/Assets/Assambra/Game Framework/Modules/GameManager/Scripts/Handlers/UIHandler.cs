using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assambra.GameFramework.GameManager
{
    public class UIHandler : MonoBehaviour
    {
        [SerializeField] SceneHandler sceneHandler;
        [field: SerializeField] public Transform Canvas { get; private set; }

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
            //获取上一个场景的 UI 元素名称。
            //获取新场景的 UI 元素名称。
            //销毁当前场景中已不再需要的 UI 元素。
            //实例化新场景中新增的 UI 元素。

            HashSet<string> lastUIElements = new HashSet<string>();

            //SelectMany 从每个 SceneUISet 中提取所有 UI 元素预制件。
            //Select(obj => obj.name) 获取每个 UI 预制件的名称。
            //最终，使用 ToHashSet() 将 UI 元素名称转换为一个 HashSet，避免重复元素。
            //这个 lastUIElements 集合保存了上一个场景中的 UI 元素名称。
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

            //遍历当前场景中的 UI 元素集合 uIElements（假设这是当前场景中的活动 UI 元素）。
            //如果当前的 UI 元素名称不在新场景的 newUIElements 集合中，
            //表示该 UI 元素在新场景中不再需要，调用 Destroy(uIElement) 销毁该 UI 元素，
            //并将其从 uIElements 集合中移除。
            foreach (var uIElement in uIElements.ToList())
            {
                if (!newUIElements.Contains(uIElement.name))
                {
                    Destroy(uIElement);
                    uIElements.Remove(uIElement);
                }
            }

            //遍历新场景的 SceneUISets 集合中的每个 UI 元素预制件。
            //如果上一个场景不存在（即 lastScenen == null），
            //或者该 UI 元素在上一个场景的 UI 集合中不存在（即它是新 UI 元素），
            //则调用 InstantiateCurrentSceneUI(obj) 实例化该 UI 元素并将其添加到当前场景中。
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
}
