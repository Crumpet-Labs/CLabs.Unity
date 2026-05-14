using Buttr.Core;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    public readonly struct ExampleMessage {
        public string Message { get; }
        public ExampleMessage(string message) => Message = message;
    }

    public sealed class Publisher : MonoBehaviour {
        private BellRope m_Rope;

        private void Awake() {
            m_Rope = Application<IBellTower>.Get().Rope(GetType());
        }

        [ContextMenu("Send Message")]
        public void SendMessage() => m_Rope.Ring(new ExampleMessage("Hello from Bell"));
    }
}
