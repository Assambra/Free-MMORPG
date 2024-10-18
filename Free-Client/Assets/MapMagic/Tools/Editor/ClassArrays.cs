using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Den.Tools
{
	public static class ClassArrays
	{
			public static void DrawClassArray<T> (ref T[] array, string name, Action<T> assign, UnityEngine.Object undoObject) where T: IEditorGuiArrayElement
			{
				//undoObject.Update();

				GUILayout.Space(10);
				EditorGUILayout.LabelField(name);

				if (array == null)
					array = new T[0];

				for (int i=0; i<array.Length; i++)
					DrawClassElement(ref array, i, undoObject);

				EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("Add", GUILayout.Width(70f), GUILayout.Height(16f)))
						ChooseCreateClassElement(assign, undoObject);
				EditorGUILayout.EndHorizontal();
		
				//undoObject.ApplyModifiedProperties();
			}


			public static void DrawClassElement<T> (ref T[] controllers, int index, UnityEngine.Object undoObject) where T: IEditorGuiArrayElement
			{
				T controller = controllers[index];
				Type controllerType = controller.GetType();


				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				EditorGUILayout.BeginVertical();


				Rect foldoutRect = EditorGUILayout.BeginHorizontal();
				controller.Expanded = EditorGUILayout.Foldout(controller.Expanded,  ObjectNames.NicifyVariableName(controllerType.Name), true);
				GUILayout.Space(10f);

				bool controllerEnabled = GUILayout.Toggle(controller.Enabled, "", GUILayout.Width(20f), GUILayout.Height(14f));
				if (controllerEnabled != controller.Enabled)
				{
					Undo.RecordObject(undoObject, "Enable State Switch");
					controller.Enabled = controllerEnabled;
					EditorUtility.SetDirty(undoObject);
				}

				if (GUILayout.Button("↑", GUILayout.Width(20f), GUILayout.Height(14f)) && index != 0)
				{
					Undo.RecordObject(undoObject, "Moving up");
					ArrayTools.Switch(controllers, index, index-1);
					EditorUtility.SetDirty(undoObject);
				}

				if (GUILayout.Button("↓", GUILayout.Width(20f), GUILayout.Height(14f)) && index != controllers.Length-1)
				{
					Undo.RecordObject(undoObject, "Moving down");
					ArrayTools.Switch(controllers, index, index+1);
					EditorUtility.SetDirty(undoObject);
				}

				if (GUILayout.Button("✕", GUILayout.Width(20f), GUILayout.Height(14f)))
				{
					Undo.RecordObject(undoObject, "Removing Element");
					ArrayTools.RemoveAt(ref controllers, index);
					EditorUtility.SetDirty(undoObject);
				}

				EditorGUILayout.EndHorizontal();

					//controller.guiExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), controller.guiExpanded, controllerType.Name, true);//EditorStyles.foldoutHeader);

				if (controller.Expanded)
					EditorHelper.DrawClass(controller, false, undoObject);

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}


			public static void ChooseCreateClassElement<T> (Action<T> assign, UnityEngine.Object undoObject)
			{
				GenericMenu menu = new GenericMenu();

				List<Type> classTypes = new List<Type>();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (Assembly assembly in assemblies)
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
							classTypes.Add(type);
					}
				}

				EditorGUI.BeginChangeCheck();

				foreach (Type controllerType in classTypes)
					menu.AddItem(new GUIContent(controllerType.ToString()), false, () =>
					{
						Undo.RecordObject(undoObject, "Add New");

						T obj = (T)Activator.CreateInstance(controllerType);
						assign(obj);

						EditorUtility.SetDirty(undoObject);
					});

				menu.ShowAsContext();

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(undoObject, "Controller Added");
					EditorUtility.SetDirty(undoObject);
				}
			}


			public static List<Type> GetSubclassList (Type baseType)
			{
				List<Type> classTypes = new List<Type>();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (Assembly assembly in assemblies)
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
							classTypes.Add(type);
					}
				}

				return classTypes;
			}
	}
}
