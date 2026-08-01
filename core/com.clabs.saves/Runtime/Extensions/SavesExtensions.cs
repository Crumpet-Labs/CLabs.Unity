using System.Text;

namespace CLabs.Saves {
    internal static class SavesExtensions {
        public static string ToHexString(this byte[] bytes) {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes) {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
