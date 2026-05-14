#if UNITY_EDITOR
using System.Collections.Generic;
using CLabs.Adapters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Utility.Unity.Editor {
[CustomPropertyDrawer(typeof(CLabs.Utility.SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer {
    
    private const string KeysName = "m_Keys";
    private const string ValuesName = "m_Values";

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var root = new VisualElement();
        root.style.marginBottom = 10;

        // 1. Create Header
        var foldout = new Foldout {
            text = property.displayName,
            value = true
        };
        foldout.Q<Toggle>().style.marginBottom = 2;
        root.Add(foldout);

        // 2. The Container
        var listContainer = new VisualElement();
        listContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f).ToUnityColor();
        listContainer.style.borderTopLeftRadius = 5;
        listContainer.style.borderTopRightRadius = 5;
        listContainer.style.borderBottomLeftRadius = 5;
        listContainer.style.borderBottomRightRadius = 5;
        listContainer.style.paddingTop = 5;
        listContainer.style.paddingBottom = 5;
        foldout.Add(listContainer);

        void RefreshList() {
            listContainer.Clear();

            var keysProp = property.FindPropertyRelative(KeysName);
            var valuesProp = property.FindPropertyRelative(ValuesName);

            // Safety: Ensure arrays are synced
            if (keysProp.arraySize != valuesProp.arraySize) {
                valuesProp.arraySize = keysProp.arraySize;
                property.serializedObject.ApplyModifiedProperties();
            }

            // Update Header Text
            foldout.text = $"{property.displayName} ({keysProp.arraySize} items)";

            var duplicateIndices = GetDuplicateKeyIndices(keysProp);

            // -- HEADERS --
            if (keysProp.arraySize > 0) {
                var headerRow = new VisualElement();
                headerRow.style.flexDirection = FlexDirection.Row;
                headerRow.style.paddingLeft = 5;
                headerRow.style.paddingRight = 25;
                headerRow.style.marginBottom = 5;

                var labelKey = new Label("Key") { style = { flexGrow = 1, width = Length.Percent(45), unityFontStyleAndWeight = FontStyle.Bold } };
                var labelVal = new Label("Value") { style = { flexGrow = 1, width = Length.Percent(45), unityFontStyleAndWeight = FontStyle.Bold } };

                headerRow.Add(labelKey);
                headerRow.Add(labelVal);
                listContainer.Add(headerRow);
            }

            // -- ROWS --
            for (int i = 0; i < keysProp.arraySize; i++) {
                int index = i;
                var keyProp = keysProp.GetArrayElementAtIndex(i);
                var valProp = valuesProp.GetArrayElementAtIndex(i);

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.FlexStart;
                row.style.marginBottom = 2;
                row.style.paddingLeft = 5;
                row.style.paddingRight = 5;
                row.style.paddingTop = 5;
                row.style.paddingBottom = 5;

                row.style.backgroundColor = ((i % 2 == 0)
                    ? new Color(0, 0, 0, 0.1f)
                    : new Color(0, 0, 0, 0.2f)
                ).ToUnityColor();

                if (duplicateIndices.Contains(i)) {
                    row.style.backgroundColor = new Color(0.5f, 0.1f, 0.1f, 0.5f).ToUnityColor();
                    row.tooltip = "Duplicate Key detected!";
                }

                // REMOVED .BindProperty() - PropertyField constructor handles it
                var keyField = new PropertyField(keyProp, "") {
                    style = { flexGrow = 1, width = Length.Percent(45), marginRight = 5 }
                };

                var valField = new PropertyField(valProp, "") {
                    style = { flexGrow = 1, width = Length.Percent(45) }
                };

                var deleteBtn = new Button(() => {
                    var k = property.FindPropertyRelative(KeysName);
                    var v = property.FindPropertyRelative(ValuesName);
                    k.DeleteArrayElementAtIndex(index);
                    v.DeleteArrayElementAtIndex(index);
                    property.serializedObject.ApplyModifiedProperties();
                    RefreshList();
                }) { text = "X" };

                deleteBtn.style.width = 20;
                deleteBtn.style.backgroundColor = Color.Transparent.ToUnityColor();
                deleteBtn.style.borderBottomWidth = 0;
                deleteBtn.style.borderTopWidth = 0;
                deleteBtn.style.borderRightWidth = 0;
                deleteBtn.style.borderLeftWidth = 0;
                deleteBtn.style.color = new Color(1, 0.4f, 0.4f).ToUnityColor();

                row.Add(keyField);
                row.Add(valField);
                row.Add(deleteBtn);

                listContainer.Add(row);
            }

            // -- ADD BUTTON --
            var addButton = new Button(() => {
                var k = property.FindPropertyRelative(KeysName);
                var v = property.FindPropertyRelative(ValuesName);
                k.InsertArrayElementAtIndex(k.arraySize);
                v.InsertArrayElementAtIndex(v.arraySize);
                property.serializedObject.ApplyModifiedProperties();
                RefreshList();
            }) { text = "+ Add Entry" };
            addButton.style.marginTop = 5;
            listContainer.Add(addButton);
        }

        RefreshList();
        return root;
    }

    private HashSet<int> GetDuplicateKeyIndices(SerializedProperty keysProp) {
        var duplicates = new HashSet<int>();
        var seenKeys = new Dictionary<object, int>();

        for (int i = 0; i < keysProp.arraySize; i++) {
            var prop = keysProp.GetArrayElementAtIndex(i);
            object keyVal = GetValue(prop);
            if (keyVal != null) {
                if (seenKeys.ContainsKey(keyVal)) {
                    duplicates.Add(i);
                    duplicates.Add(seenKeys[keyVal]); 
                } else {
                    seenKeys.Add(keyVal, i);
                }
            }
        }
        return duplicates;
    }

    private object GetValue(SerializedProperty prop) {
        switch (prop.propertyType) {
            case SerializedPropertyType.Integer: return prop.intValue;
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Float: return prop.floatValue;
            case SerializedPropertyType.Boolean: return prop.boolValue;
            case SerializedPropertyType.Enum: return prop.enumValueIndex;
            case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue;
            case SerializedPropertyType.Vector2: return prop.vector2Value;
            case SerializedPropertyType.Vector3: return prop.vector3Value;
            case SerializedPropertyType.Color: return prop.colorValue;
            case SerializedPropertyType.Generic: return prop.boxedValue;
            default: return null; 
        }
    }
}
}
#endif