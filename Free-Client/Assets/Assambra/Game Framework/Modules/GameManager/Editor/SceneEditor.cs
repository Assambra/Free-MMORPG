using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Scene))]
public class SceneEditor : Editor
{
    SerializedProperty scenePathsProperty;

    private void OnEnable()
    {
        scenePathsProperty = serializedObject.FindProperty("scenePaths");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsFirstScene"), true);

        for (int i = 0; i < scenePathsProperty.arraySize; i++)
        {
            SerializedProperty scenePathProp = scenePathsProperty.GetArrayElementAtIndex(i);
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePathProp.stringValue);
            EditorGUI.BeginChangeCheck();
            SceneAsset newScene = EditorGUILayout.ObjectField("Scene " + (i + 1), scene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                scenePathProp.stringValue = AssetDatabase.GetAssetPath(newScene);
            }
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Scene"))
        {
            scenePathsProperty.arraySize++;
        }
        if (scenePathsProperty.arraySize > 0 && GUILayout.Button("Remove Scene"))
        {
            scenePathsProperty.arraySize--;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("SceneUISets"), true);

        serializedObject.ApplyModifiedProperties();
    }
}