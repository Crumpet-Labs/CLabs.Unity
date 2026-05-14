using UnityEngine;

namespace CLabs.Utility {
    public static class TransformUtils {
        public static Transform ForceGameObject(string name, bool dontDestroyOnLoad = false) {
            var root = GameObject.Find(name);

            if (root == null) {
                root = new GameObject(name) {
                    transform = {
                        localPosition = Vector3.zero,
                        localRotation = Quaternion.identity
                    }
                };
            }
            
            if(dontDestroyOnLoad) Object.DontDestroyOnLoad(root);

            return root.transform;
        }
        
        public static T ForceComponent<T>(this Transform transform) where T : Component {
            return transform.GetComponent<T>() ?? transform.gameObject.AddComponent<T>();
        }
        
        public static bool TryDestroyComponent<T>(this Transform transform) where T : Component {
            var component = transform.GetComponent<T>();
            var exists = component != null;
            if (exists) {
                Object.Destroy(component);
            }
            
            return exists;
        }
    }
}