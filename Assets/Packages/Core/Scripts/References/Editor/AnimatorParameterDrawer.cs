using UnityEditor;
using UnityEngine;
using VarelaAloisio.Core;

namespace VarelaAloisio.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(AnimatorParameter))]
	public class AnimatorParameterDrawer : PropertyDrawer
	{
		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.boxedValue is AnimatorParameter animatorParameter)
			{
				string tempValue = EditorGUILayout.TextField(label, animatorParameter.Name);
				if (tempValue == animatorParameter.Name)
					return;
				animatorParameter.Name = tempValue;
				property.boxedValue = animatorParameter;
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			else
				EditorGUILayout.HelpBox($"{nameof(property)}'s boxed value is not of type {nameof(AnimatorParameter)}", MessageType.Error);
		}
	}
}
