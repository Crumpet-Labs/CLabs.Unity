using UnityEditor;
using UnityEngine;

namespace CLabs.Adapters {
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public sealed class RequireInterfaceDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var attr = (RequireInterfaceAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference) {
                EditorGUI.HelpBox(position, "[RequireInterface] requires an Object reference field.", MessageType.Error);
                return;
            }

            label.tooltip = $"Requires: {attr.InterfaceType.Name}";

            EditorGUI.BeginProperty(position, label, property);

            var newObj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), false);

            if (newObj == null || attr.InterfaceType.IsAssignableFrom(newObj.GetType()))
                property.objectReferenceValue = newObj;

            EditorGUI.EndProperty();
        }
    }
}
