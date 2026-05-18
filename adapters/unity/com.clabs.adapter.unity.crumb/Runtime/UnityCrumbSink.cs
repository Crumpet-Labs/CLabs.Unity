using CLabs.Crumb;
using UnityEngine;

namespace CLabs.Adapters {
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
