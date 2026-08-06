using System;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsRow : VisualElement {
        public Label NameLabel { get; }
        public bool Selected { get; private set; }

        private CLabsRow(string name, Action onClick) {
            AddToClassList("clabs-row");

            NameLabel = new Label(name);
            NameLabel.AddToClassList("clabs-row__name");
            base.Add(NameLabel);

            if (onClick != null) {
                RegisterCallback<ClickEvent>(_ => onClick());
            }
        }

        // Fluent shadow of VisualElement.Add: returns this so callers can chain badges.
        public new CLabsRow Add(VisualElement element) {
            base.Add(element);
            return this;
        }

        public CLabsRow SetSelected(bool selected) {
            Selected = selected;
            if (selected) AddToClassList("clabs-row--selected");
            else RemoveFromClassList("clabs-row--selected");
            return this;
        }

        public CLabsRow SetDisabled(bool disabled) {
            if (disabled) AddToClassList("clabs-row--disabled");
            else RemoveFromClassList("clabs-row--disabled");
            return this;
        }

        public static CLabsRow Create(string name, Action onClick = null) {
            return new CLabsRow(name, onClick);
        }
    }
}
