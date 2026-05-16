using UnityEditor;
using UnityEngine;
using VarelaAloisio.Core.Attributes;

namespace VarelaAloisio.Core.Editor
{
	[CustomPropertyDrawer(typeof(ExposeScriptableObjectAttribute))]
	public class ExposeScriptableObjectPropertyDrawer : PropertyDrawer
	{
		private UnityEditor.Editor _editor = null;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(position, property, label, true);
			
			if (property.objectReferenceValue)
			{
				var rect = position;
				rect.y += EditorGUIUtility.singleLineHeight;
				property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, GUIContent.none);
			}
 
			if (property.isExpanded)
			{
				EditorGUI.indentLevel++;
         
				if (!_editor)
					UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
				_editor.OnInspectorGUI();
         
				EditorGUI.indentLevel--;
			}
			else
				EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
		}
	}
}
