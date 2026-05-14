
using UnityEditor;
using UnityEngine;

namespace CLabs.Utility.Editor {
    [CustomPropertyDrawer(typeof(BoolProperty))]
    public class PropertyDrawer_Bool : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Layout
            float halfWidth = position.width / 2f;
            Rect valueRect = new Rect(position.x, position.y, halfWidth - 5, position.height);
            Rect dirtyRect = new Rect(position.x + halfWidth + 5, position.y, halfWidth - 5, position.height);

            // Fields
            var valueProp = property.FindPropertyRelative("m_Value");
            var dirtyProp = property.FindPropertyRelative("m_IsDirty");

            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
            EditorGUI.LabelField(dirtyRect, valueProp.displayName, dirtyProp.boolValue.ToString());

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}