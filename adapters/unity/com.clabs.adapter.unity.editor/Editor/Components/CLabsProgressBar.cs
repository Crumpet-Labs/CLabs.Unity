using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public sealed class CLabsProgressBar : VisualElement {
        public Label ValueLabel { get; }

        private readonly VisualElement m_Fill;

        private CLabsProgressBar(bool showLabel, int height) {
            AddToClassList("clabs-progress");
            style.height = height;

            m_Fill = new VisualElement();
            m_Fill.AddToClassList("clabs-progress__fill");
            m_Fill.style.width = Length.Percent(0f);
            Add(m_Fill);

            if (showLabel) {
                ValueLabel = new Label();
                ValueLabel.AddToClassList("clabs-progress__label");
                Add(ValueLabel);
            }
        }

        public CLabsProgressBar SetProgress(float normalized) {
            float clamped = Mathf.Clamp01(normalized);
            m_Fill.style.width = Length.Percent(clamped * 100f);
            return this;
        }

        public CLabsProgressBar SetProgress(float current, float max) {
            float ratio = max > 0f ? current / max : 0f;
            return SetProgress(ratio);
        }

        public CLabsProgressBar SetLabel(string text) {
            if (ValueLabel != null) ValueLabel.text = text;
            return this;
        }

        public CLabsProgressBar SetFillColor(Color color) {
            m_Fill.style.backgroundColor = color;
            return this;
        }

        public static CLabsProgressBar Create(bool showLabel = false, int height = 6) {
            return new CLabsProgressBar(showLabel, height);
        }
    }
}
