using Buttr.Core;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    public static class k {
        public static class BellTowers {
            public const int Publisher = 1;
        }
    }
    
    public readonly struct ExampleMessage {
        public string Message { get; }
        public ExampleMessage(string message) => Message = message;
    }

    public sealed class Publisher : MonoBehaviour {
        private BellRope m_Rope;

        private void Awake() {
            m_Rope = Application<IBellTower>.Get()
                .Rope(k.BellTowers.Publisher);
        }

        [ContextMenu("Send Message")]
        public void SendMessage() => m_Rope.RingBell(new ExampleMessage("Hello from Bell"));
    }
}
