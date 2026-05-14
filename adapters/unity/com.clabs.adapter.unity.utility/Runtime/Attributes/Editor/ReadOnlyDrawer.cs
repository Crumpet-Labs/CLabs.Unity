using UnityEditor;
using UnityEngine;

namespace CLabs.Utility.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        private const string INCOMPATIBLE = "Incompatible";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var valueStr = property.propertyType switch {
                SerializedPropertyType.Integer => property.intValue.ToString(),
                SerializedPropertyType.Boolean => property.boolValue.ToString(),
                SerializedPropertyType.Float => property.floatValue.ToString("0.00000"),
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Vector2 => property.vector2Value.ToString(),
                SerializedPropertyType.Vector3 => property.vector3Value.ToString(),
                _ => INCOMPATIBLE
            };

            if (valueStr.Equals(INCOMPATIBLE)) {
                EditorGUILayout.PropertyField(property, true);
            } else {
                EditorGUI.LabelField(position, label.text, valueStr);
            }
        }

    }
}
