using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsEmptyState : VisualElement {
        public Label MessageLabel { get; }

        private VisualElement[] m_BoundSiblings;

        private CLabsEmptyState(string message) {
            AddToClassList("clabs-empty");

            MessageLabel = new Label(message);
            Add(MessageLabel);
        }

        public CLabsEmptyState SetMessage(string message) {
            MessageLabel.text = message;
            return this;
        }

        public CLabsEmptyState SetVisible(bool visible) {
            style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (m_BoundSiblings != null) {
                var siblingDisplay = visible ? DisplayStyle.None : DisplayStyle.Flex;
                foreach (var sibling in m_BoundSiblings) {
                    if (sibling != null) sibling.style.display = siblingDisplay;
                }
            }
            return this;
        }

        public CLabsEmptyState BindTo(params VisualElement[] siblings) {
            m_BoundSiblings = siblings;
            return this;
        }

        public static CLabsEmptyState Create(string message) {
            return new CLabsEmptyState(message);
        }
    }
}
