using CLabs.Utility;

namespace CLabs.Adapters {
    public static class ColorExtensions {
        /// <summary>Convert an engine-agnostic <see cref="Color"/> to a Unity colour.</summary>
        public static UnityEngine.Color ToUnityColor(this Color color) =>
            new(color.R, color.G, color.B, color.A);

        /// <summary>Convert a Unity colour to the engine-agnostic <see cref="Color"/>.</summary>
        public static Color ToCLabsColor(this UnityEngine.Color color) =>
            new(color.r, color.g, color.b, color.a);

        /// <summary>Convert a Unity 32-bit colour to the engine-agnostic <see cref="Color"/>.</summary>
        public static Color ToCLabsColor(this UnityEngine.Color32 color) =>
            Color.FromRgb255(color.r, color.g, color.b, color.a);
    }
}
