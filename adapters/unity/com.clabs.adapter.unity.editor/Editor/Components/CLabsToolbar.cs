using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsToolbar : VisualElement {
        public ToolbarSearchField SearchField { get; }

        private CLabsToolbar(Action<string> onSearch) {
            AddToClassList("clabs-toolbar");

            SearchField = new ToolbarSearchField();
            SearchField.AddToClassList("clabs-toolbar__search");

            if (onSearch != null) {
                SearchField.RegisterValueChangedCallback(evt => onSearch(evt.newValue));
            }

            Add(SearchField);
        }

        public CLabsToolbar AddButton(string label, Action onClick) {
            var button = new Button(onClick) { text = label };
            button.AddToClassList("clabs-toolbar__button");
            Add(button);
            return this;
        }

        public static CLabsToolbar Create(Action<string> onSearch = null) {
            return new CLabsToolbar(onSearch);
        }
    }
}
