using CLabs.Crumb;
using UnityEngine;

namespace CLabs.Adapters {
    /// <summary>The Unity <see cref="ICrumbSink"/>: routes log lines to <c>Debug.Log</c> / <c>LogWarning</c> / <c>LogError</c> by level. The CrumbApplicationLoader registers it in place of the console sink.</summary>
    public sealed class UnityCrumbSink : ICrumbSink {
        public void Write(string level, string typeName, string message) {
            var formatted = $"[{level}] [{typeName}] {message}";
            switch (level) {
                case "WRN":
                    Debug.LogWarning(formatted);
                    break;
                case "ERR":
                case "FTL":
                    Debug.LogError(formatted);
                    break;
                default:
                    Debug.Log(formatted);
                    break;
            }
        }
    }
}
