using UnityEditor;
using UnityEngine.UIElements;

namespace CLabs.Utility.Editor
{
    [CustomPropertyDrawer(typeof(EditorButton))]
    public class EditorButtonUIE : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var serializedObjectTargetObject = property.serializedObject.targetObject;
            
            var container = new VisualElement();

            container.Add(new Button(() =>
            {
                property.serializedObject.targetObject.GetType().GetMethod("Invoke")?.Invoke(property.serializedObject.targetObject, null);
            })
            {
                text = (string)property.serializedObject.targetObject.GetType().GetProperty("Name")?.GetValue(property.serializedObject.targetObject)
            });
            
            return container;
        }
    }
}