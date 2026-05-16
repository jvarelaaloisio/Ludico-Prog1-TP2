using VarelaAloisio.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace VarelaAloisio.Core.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializeReadOnlyAttribute))]
    public class SerializeReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isReadOnly = attribute is SerializeReadOnlyAttribute;

            EditorGUI.BeginDisabledGroup(isReadOnly);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
