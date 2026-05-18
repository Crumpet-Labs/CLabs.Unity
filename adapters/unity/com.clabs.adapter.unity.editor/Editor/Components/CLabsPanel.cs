using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsPanel : VisualElement {
        public ScrollView Content { get; }

        public CLabsPanel(string title) {
            AddToClassList("clabs-panel");

            var header = new Label(title);
            header.AddToClassList("clabs-panel__header");

            Content = new ScrollView(ScrollViewMode.Vertical);
            Content.AddToClassList("clabs-panel__list");

            Add(header);
            Add(Content);
        }

        public static CLabsPanel Create(string title) {
            return new CLabsPanel(title);
        }
    }
}
