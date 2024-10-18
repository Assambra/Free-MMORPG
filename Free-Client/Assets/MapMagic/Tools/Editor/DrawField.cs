using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Den.Tools
{
	public static class EditorHelper
	{
		public static void DrawClass (object obj, bool sortParentClasses, UnityEngine.Object undoObj)
		{
			Type type = obj.GetType();

			IEnumerable<FieldInfo> fields = GetFieldsInDeclarationOrder(type); //this will also skip HideInInspector

			string declaringClass = null;
			foreach (FieldInfo field in fields)
			{
				if (sortParentClasses  &&  field.DeclaringType.Name != declaringClass)
				{
					EditorGUILayout.Space(10);
					declaringClass = field.DeclaringType.Name;
					EditorGUILayout.LabelField(declaringClass + ":");
				}

				object value = field.GetValue(obj);
				
				EditorGUILayout.BeginHorizontal();
					DrawField(field, ref value, obj, undoObj);
				EditorGUILayout.EndHorizontal();
			}
		}

		private static IEnumerable<FieldInfo> GetFieldsInDeclarationOrder(Type type) {
			var fields = new List<FieldInfo>();
			if (type.BaseType != null) {
				fields.AddRange(GetFieldsInDeclarationOrder(type.BaseType));
			}
			fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(f => !Attribute.IsDefined(f, typeof(HideInInspector)))
				.OrderBy(f => GetFieldPosition(f))
				.Where(f => !fields.Any(f2 => f2.Name == f.Name && f2.DeclaringType == f.DeclaringType)));
			return fields;
		}

		private static int GetFieldPosition(FieldInfo field) 
		{
			var module = field.Module;
			var type = field.DeclaringType;
			var name = field.Name;
			var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var method in methods) {
				var il = method.GetMethodBody()?.GetILAsByteArray();
				if (il != null) {
					for (var i = 0; i < il.Length - 2; i++) {
						if (il[i] == 0x06 && il[i + 1] == field.MetadataToken && il[i + 2] == 0x0e) {
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static void DrawField (FieldInfo field, ref object value, object obj, UnityEngine.Object undoObj)
		{
			EditorGUI.BeginChangeCheck();

			Type fieldType = field.FieldType;
			string label = ObjectNames.NicifyVariableName(field.Name);

			if (fieldType == typeof(int))
			{
				value = EditorGUILayout.IntField(label, (int)value);
			}
			else if (fieldType == typeof(float))
			{
				value = EditorGUILayout.FloatField(label, (float)value);
			}
			else if (fieldType == typeof(string))
			{
				value = EditorGUILayout.TextField(label, (string)value);
			}
			else if (fieldType == typeof(bool))
			{
				value = EditorGUILayout.Toggle(label, (bool)value);
			}
			else if (fieldType == typeof(Vector2))
			{
				value = EditorGUILayout.Vector2Field(label, (Vector2)value);
			}
			else if (fieldType == typeof(Vector3))
			{
				value = EditorGUILayout.Vector3Field(label,  (Vector3)value);
			}
			else if (fieldType == typeof(Vector4))
			{
				value = EditorGUILayout.Vector4Field(label,  (Vector4)value);
			}
			else if (fieldType == typeof(Color))
			{
				value = EditorGUILayout.ColorField(label,  (Color)value);
			}
			else if (fieldType == typeof(UnityEngine.Object))
			{
				value = EditorGUILayout.ObjectField(label,  (UnityEngine.Object)value, fieldType, true);
			}
			else if (fieldType == typeof(double))
			{
				value = EditorGUILayout.DoubleField(label, (double)value);
			}
			else if (fieldType == typeof(uint))
			{
				value = (uint)EditorGUILayout.LongField(label, (long)(uint)value);
			}
			else if (fieldType.IsEnum)
			{
				if (Attribute.IsDefined(fieldType, typeof(FlagsAttribute)))
					value = EditorGUILayout.EnumFlagsField(label, (Enum)value);
				else
					value = EditorGUILayout.EnumPopup(label, (Enum)value);
			}
			else if (fieldType == typeof(Quaternion))
			{
				value = QuaternionField(label,  (Quaternion)value);
			}
			else if (fieldType == typeof(Rect))
			{
				value = EditorGUILayout.RectField(label,  (Rect)value);
			}
			else if (fieldType == typeof(Bounds))
			{
				value = EditorGUILayout.BoundsField(label,  (Bounds)value);
			}
			else if (fieldType == typeof(LayerMask))
			{
				value = EditorGUILayout.LayerField((LayerMask)value);
			}
			else if (fieldType == typeof(AnimationCurve))
			{
				value = EditorGUILayout.CurveField(label,  (AnimationCurve)value);
			}
			else if (fieldType == typeof(Gradient))
			{
				value = EditorGUILayout.GradientField(label,  (Gradient)value);
			}
			else if (fieldType == typeof(Vector2Int))
			{
				value = EditorGUILayout.Vector2IntField(label,  (Vector2Int)value);
			}
			else if (fieldType == typeof(Vector3Int))
			{
				value = EditorGUILayout.Vector3IntField(label,  (Vector3Int)value);
			}
			else if (fieldType == typeof(Color32))
			{
				value = EditorGUILayout.ColorField(label,  (Color32)value);
			}
			else if (fieldType.IsSubclassOf(typeof(ScriptableObject)))
			{
				value = EditorGUILayout.ObjectField(label,  (ScriptableObject)value, fieldType, true);
			}
			else if (fieldType == typeof(AnimationClip))
			{
				value = EditorGUILayout.ObjectField(label,  (AnimationClip)value, fieldType, true);
			}
			else if (fieldType == typeof(AudioClip))
			{
				value = EditorGUILayout.ObjectField(label,  (AudioClip)value, fieldType, true);
			}
			else if (fieldType == typeof(GameObject))
			{
				value = EditorGUILayout.ObjectField(label,  (GameObject)value, fieldType, true);
			}
			else if (typeof(UnityEngine.Component).IsAssignableFrom(fieldType))
			{
				value = EditorGUILayout.ObjectField(label,  (UnityEngine.Component)value, fieldType, true);
			}
			/*else if (type == typeof(Vector2D))
			{
				Vector2D vector2D = (Vector2D)value;
				vector2D.x = EditorGUILayout.FloatField("X", vector2D.x);
				vector2D.z = EditorGUILayout.FloatField("Z", vector2D.z);
				return vector2D;
			}*/

			else if (obj?.GetType().IsArray == true)
				EditorGUILayout.LabelField(label, FormatArray(fieldType,value));

			//else if (fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(ICollection<>)  ||  fieldType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>))))
			//else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
			//	EditorGUILayout.LabelField(label, FormatCollection(fieldType,value));

			else
				EditorGUILayout.LabelField(label, value==null ? "null" : value.ToString());

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(undoObj, label);
				field.SetValue(obj, value);
				EditorUtility.SetDirty(undoObj);
			}
		}

		public static Quaternion QuaternionField(string label, Quaternion value)
		{
			Vector4 vector = new Vector4(value.x, value.y, value.z, value.w);
			vector = EditorGUILayout.Vector4Field(label, vector);
			return new Quaternion(vector.x, vector.y, vector.z, vector.w);
		}

		public static void LabelFieldFormatted(string label)
		{
			string formattedLabel = label.Replace("_", " "); // Replace underscores with spaces
			formattedLabel = formattedLabel.ToLower(); // Convert label to lowercase
			formattedLabel = char.ToUpper(formattedLabel[0]) + formattedLabel.Substring(1); // Capitalize first letter of each word

			EditorGUILayout.LabelField(formattedLabel);
		}

		public static string FormatArray (Type type, object collection)
		{
			Type elementType = type.GetElementType();
			int count = ((Array)collection).Length;
			string countString = count == 1 ? "1 element" : $"{count} elements";
			string elementTypeString = elementType.Name;
			return $"{elementTypeString}[] {countString}";
		}

		public static string FormatCollection (Type type, object collection)
		{
			ICollection c = (ICollection)collection;
			int count = c.Count;
			string countString = count == 1 ? "1 element" : $"{count} elements";
			Type genericType = c.GetType().GenericTypeArguments.FirstOrDefault();
			string genericTypeString = genericType != null ? genericType.Name : "object";
			return $"{type.Name}<{genericTypeString}> {countString}";
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
