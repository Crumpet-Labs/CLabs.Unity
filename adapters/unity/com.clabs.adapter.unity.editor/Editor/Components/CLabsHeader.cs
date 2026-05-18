using UnityEngine.UIElements;

namespace CLabs.Editor {
    public static class CLabsHeader {
        public static VisualElement Create(string title) {
            var header = new VisualElement();
            header.AddToClassList("clabs-header");

            var brand = new Label("CRUMPET LABS");
            brand.AddToClassList("clabs-header__brand");

            var separator = new Label("\u00B7");
            separator.AddToClassList("clabs-header__separator");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("clabs-header__title");

            header.Add(brand);
            header.Add(separator);
            header.Add(titleLabel);
            return header;
        }
    }
}
