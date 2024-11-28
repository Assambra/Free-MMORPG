#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assambra.GameFramework.GameManager
{
    #if UNITY_EDITOR

    [CustomEditor(typeof(Scene), true)]
    public class SceneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var scene = (Scene)target;
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.ScenePath);

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty("ScenePath");
                scenePathProperty.stringValue = newPath;
            }

            scene.IsFirstScene = EditorGUILayout.Toggle("Is First Scene", scene.IsFirstScene);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SceneUISets"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }

    #endif
}
