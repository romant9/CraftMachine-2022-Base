using PlayId.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace PlayId.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(PasswordAttribute))]
    public class PasswordDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
    }
}