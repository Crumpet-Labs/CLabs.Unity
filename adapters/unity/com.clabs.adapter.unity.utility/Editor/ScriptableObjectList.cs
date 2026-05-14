using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Utility.Unity.Editor {
    public static class ScriptableObjectListUtils {
        public static ScriptableObjectList<T> ToObjectList<T>(this ListView view, int height) where T : ScriptableObject {
            return new ScriptableObjectList<T>(view, height);
        }
    }
    
    public class ScriptableObjectList<T> where T : ScriptableObject {
        public readonly List<T> objects;
        public readonly ListView list;

        public ScriptableObjectList(ListView list, int height) {
            objects = new List<T>();
            this.list = list;
            
            list.itemsSource = objects;
            list.fixedItemHeight = height;
            list.makeItem = () => {
                var field = new ObjectField { objectType = typeof(T) };

                field.RegisterValueChangedCallback((ev) => {
                    objects[list.itemsSource.Count-1] = (T)ev.newValue;
                });

                return field;
            };
            list.bindItem = (e, i) => { ((ObjectField)e).value = objects[i]; };
            list.showAddRemoveFooter = true;

            list.onAdd = (view) => {
                var count = view.itemsSource.Count;
                view.itemsSource.Add(ScriptableObject.CreateInstance<T>());
                view.RefreshItems();
                view.ScrollToItem(count);
            };

            list.onRemove = (view) => {
                var count = view.itemsSource.Count;

                if (count == 0)
                    return;
                
                view.itemsSource.RemoveAt(--count);
                view.RefreshItems();
            };
        }
    }
}