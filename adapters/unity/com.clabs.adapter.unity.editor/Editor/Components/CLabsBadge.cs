using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public enum BadgeKind { Accent, Success, Warning, Error, Neutral }

    public static class CLabsBadge {
        public static Label Create(string text, BadgeKind kind = BadgeKind.Neutral) {
            var label = new Label(text);
            label.AddToClassList("clabs-badge");
            label.AddToClassList(ModifierClass(kind));
            return label;
        }

        public static Label Create(string text, Color background) {
            var label = new Label(text);
            label.AddToClassList("clabs-badge");
            label.style.backgroundColor = background;
            label.style.color = ContrastColor(background);
            return label;
        }

        private static string ModifierClass(BadgeKind kind) {
            switch (kind) {
                case BadgeKind.Accent:  return "clabs-badge--accent";
                case BadgeKind.Success: return "clabs-badge--success";
                case BadgeKind.Warning: return "clabs-badge--warning";
                case BadgeKind.Error:   return "clabs-badge--error";
                default:                return "clabs-badge--neutral";
            }
        }

        public static Color ContrastColor(Color background) {
            float luminance = 0.299f * background.r + 0.587f * background.g + 0.114f * background.b;
            return luminance > 0.5f ? Color.black : Color.white;
        }
    }
}
