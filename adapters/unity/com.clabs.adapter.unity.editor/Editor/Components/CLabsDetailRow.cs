using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsDetailRow : VisualElement {
        public Label LabelElement { get; }
        public VisualElement ValueContainer { get; }

        private CLabsDetailRow(string label, int labelWidth) {
            AddToClassList("clabs-detail-row");

            LabelElement = new Label(label);
            LabelElement.AddToClassList("clabs-detail-row__label");
            LabelElement.style.width = labelWidth;
            Add(LabelElement);

            ValueContainer = new VisualElement();
            ValueContainer.AddToClassList("clabs-detail-row__value");
            Add(ValueContainer);
        }

        public CLabsDetailRow SetValue(string text) {
            ValueContainer.Clear();
            ValueContainer.Add(new Label(text));
            return this;
        }

        public CLabsDetailRow SetValue(VisualElement element) {
            ValueContainer.Clear();
            if (element != null) ValueContainer.Add(element);
            return this;
        }

        public CLabsDetailRow AddValue(VisualElement element) {
            if (element != null) ValueContainer.Add(element);
            return this;
        }

        public static CLabsDetailRow Create(string label, int labelWidth = 90) {
            return new CLabsDetailRow(label, labelWidth);
        }
    }
}
