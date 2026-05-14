using CLabs.Utility;
using UnityEngine;

namespace CLabs.Adapters {
    public static class MonoBehaviourExtensions {
        public static OwnerId GetOwnerId(this MonoBehaviour behaviour)
            => (int)behaviour.GetEntityId();

        public static OwnerId GetOwnerId(this GameObject gameObject)
            => (int)gameObject.GetEntityId();
    }
}
