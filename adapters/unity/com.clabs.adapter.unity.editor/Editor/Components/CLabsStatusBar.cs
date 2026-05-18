using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsStatusBar : VisualElement {
        public Label Text { get; }

        private CLabsStatusBar() {
            AddToClassList("clabs-status");

            Text = new Label();
            Text.AddToClassList("clabs-status__label");

            Add(Text);
        }

        public CLabsStatusBar AddDot(bool enabled) {
            var dot = new VisualElement();
            dot.AddToClassList("clabs-status__dot");
            dot.AddToClassList(enabled ? "clabs-status__dot--enabled" : "clabs-status__dot--disabled");
            Insert(childCount - 1, dot);
            return this;
        }

        public static CLabsStatusBar Create() {
            return new CLabsStatusBar();
        }
    }
}
