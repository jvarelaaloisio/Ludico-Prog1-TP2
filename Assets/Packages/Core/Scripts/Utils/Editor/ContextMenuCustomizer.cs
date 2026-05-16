using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VarelaAloisio.Core.Editor
{
	[InitializeOnLoad]
	public static class ContextMenuCustomizer
	{
		private static readonly PropertyInfo ObjectReferenceProperty = typeof(SerializedProperty)
			.GetProperty("objectReferenceTypeString", BindingFlags.NonPublic | BindingFlags.Instance);
		static ContextMenuCustomizer()
		{
			EditorApplication.contextualPropertyMenu -= HandleContextMenu;
			EditorApplication.contextualPropertyMenu += HandleContextMenu;
		}

		private static void HandleContextMenu(GenericMenu menu, SerializedProperty property)
		{
			if (property.serializedObject.targetObject is Component
			    && property.propertyType is SerializedPropertyType.ObjectReference)
			{
				var type = property.serializedObject.targetObject.GetType();
				bool isComponent = type.IsSubclassOf(typeof(Component));
				if (isComponent)
				{
					menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(nameof(GetComponent))), false, GetComponent, property);
				}
			}
			menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(nameof(ResetValue))), false, ResetValue, property);
			if (property.propertyType is not SerializedPropertyType.ObjectReference)
				return;
			menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(nameof(SetToNone))), false, SetToNone, property);
		}

		private static void GetComponent(object data)
		{
			if (data is not SerializedProperty property
			    || property.serializedObject.targetObject is not Component subject)
				return;

			var typeFromPath = GetTypeFromPath(property.serializedObject.targetObject.GetType(), property.propertyPath);
			if (!subject.TryGetComponent(typeFromPath, out var component))
			{
				Debug.LogError($"Couldn't get component of type {typeFromPath} in {subject.name}");
				return;
			}
			Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, $"Set {property.serializedObject.targetObject.name}.{property.name} to ");
			property.objectReferenceValue = component;
			property.serializedObject.ApplyModifiedProperties();
		}

	    private static Type GetTypeFromPath(Type rootType, string propertyPath)
	    {
		    if (rootType is null)
			    return null;

		    var currentType = rootType;
	        string[] parts = propertyPath.Split('.');

	        foreach (string rawPart in parts)
	        {
	            string part = rawPart;

	            if (IsArray() || IsIndex())
		            continue;

	            FieldInfo field = currentType.GetField(part, BindingFlags.Instance
	                                                         | BindingFlags.Public
	                                                         | BindingFlags.NonPublic);

	            if (field is not null)
	            {
	                currentType = field.FieldType;

	                if (currentType.IsArray)
	                    currentType = currentType.GetElementType();
	                else if (IsList())
	                    currentType = currentType.GetGenericArguments()[0];

	                continue;
	            }

	            PropertyInfo prop = currentType.GetProperty(
	                part,
	                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
	            );

	            if (prop is null)
		            return null;

	            currentType = prop.PropertyType;
	            if (currentType.IsArray)
		            currentType = currentType.GetElementType();
	            else if (IsList())
		            currentType = currentType.GetGenericArguments()[0];

	            continue;

	            bool IsArray()
			        => part == "Array";

		        bool IsIndex()
			        => part.StartsWith("data[");

		        bool IsList()
			        => currentType.IsGenericType
			           && currentType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>);
	        }

	        return currentType;
	    }

		private static void SetToNone(object data)
		{
			if (data is not SerializedProperty property)
				return;
			Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, $"Set {property.serializedObject.targetObject.name}.{property.name} to None");
			property.objectReferenceValue = null;
			property.serializedObject.ApplyModifiedProperties();
		}

		private static void ResetValue(object data)
		{
			if (data is not SerializedProperty property)
				return;
			switch (property.propertyType)
			{
				case SerializedPropertyType.Generic:
					property.boxedValue = null;
					break;
				case SerializedPropertyType.Integer:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Boolean:
					property.boolValue = false;
					break;
				case SerializedPropertyType.Float:
					property.floatValue = 0;
					break;
				case SerializedPropertyType.String:
					property.stringValue = "";
					break;
				case SerializedPropertyType.Color:
					property.colorValue = Color.white;
					break;
				case SerializedPropertyType.ObjectReference:
					property.objectReferenceValue = null;
					break;
				case SerializedPropertyType.LayerMask:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Enum:
					property.enumValueIndex = 0;
					break;
				case SerializedPropertyType.Vector2:
					property.vector2Value = new Vector2(0, 0);
					break;
				case SerializedPropertyType.Vector3:
					property.vector3Value = new Vector3(0, 0, 0);
					break;
				case SerializedPropertyType.Vector4:
					property.vector4Value = new Vector4(0, 0, 0, 0);
					break;
				case SerializedPropertyType.Rect:
					property.rectValue = new Rect(0, 0, 0, 0);
					break;
				case SerializedPropertyType.ArraySize:
					property.arraySize = 0;
					break;
				case SerializedPropertyType.Character:
					property.stringValue = "";
					break;
				case SerializedPropertyType.AnimationCurve:
					property.animationCurveValue = AnimationCurve.Constant(0, 1, 1);
					break;
				case SerializedPropertyType.Bounds:
					property.boundsValue = new Bounds(Vector3.zero, Vector3.zero);
					break;
				case SerializedPropertyType.Gradient:
					property.gradientValue = new Gradient();
					break;
				case SerializedPropertyType.Quaternion:
					property.quaternionValue = Quaternion.identity;
					break;
				case SerializedPropertyType.ExposedReference:
					property.exposedReferenceValue = null;
					break;
				case SerializedPropertyType.Vector2Int:
					property.vector2IntValue = new Vector2Int(0, 0);
					break;
				case SerializedPropertyType.Vector3Int:
					property.vector3IntValue = new Vector3Int(0, 0, 0);
					break;
				case SerializedPropertyType.RectInt:
					property.rectIntValue = new RectInt(0, 0, 0, 0);
					break;
				case SerializedPropertyType.BoundsInt:
					property.boundsIntValue = new BoundsInt(Vector3Int.zero, Vector3Int.zero);
					break;
				case SerializedPropertyType.ManagedReference:
					property.managedReferenceValue = null;
					break;
				case SerializedPropertyType.Hash128:
					property.hash128Value = new Hash128();
					break;
			}
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}