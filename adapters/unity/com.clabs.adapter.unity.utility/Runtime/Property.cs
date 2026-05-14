using System;
using UnityEngine;

namespace CLabs.Utility {
    [Serializable]
    public class Property<T> {
        [SerializeField, ReadOnly] private T m_Value;
        [SerializeField, ReadOnly] private bool m_IsDirty;

        public Property() { }

        public Property(T value) {
            m_Value = value;
        }
        
        public T Value {
            get { return m_Value; }
            set {
                m_Value = value;
                m_IsDirty = true;
            }
        }

        public bool IsDirty {
            get { return m_IsDirty; }
        }

        public void DirtyFlagConsumed() {
            m_IsDirty = false;
        }
    }
}