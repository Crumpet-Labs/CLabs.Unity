using UnityEngine;

namespace CLabs.Utility {
    public static class GameObjectUtils {
        public static T ForceComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();

            if (component == null) {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
        
        public static bool TryDestroyComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            var exists = component != null;
            if (exists) {
                Object.Destroy(component);
            }
            
            return exists;
        }
        
        public static void SetLayer(this GameObject obj, int layer)
            => obj.layer = layer;
    }
}