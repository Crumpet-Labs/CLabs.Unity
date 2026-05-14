using System;
using System.Collections.Generic;
using UnityEngine;

namespace CLabs.Utility {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver {
        [SerializeField] private List<TKey> m_Keys = new();
        [SerializeField] private List<TValue> m_Values = new();
    
        private Dictionary<TKey, TValue> m_Dictionary = new();
    
        public Dictionary<TKey, TValue> Dictionary => m_Dictionary;
        public IEnumerable<TKey> Keys => m_Dictionary.Keys;
        public IEnumerable<TValue> Values => m_Dictionary.Values;
        
        public void OnBeforeSerialize() {
            m_Keys.Clear();
            m_Values.Clear();
        
            foreach (var kvp in m_Dictionary) {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }
    
        public void OnAfterDeserialize() {
            m_Dictionary.Clear();
        
            for (int i = 0; i < Mathf.Min(m_Keys.Count, m_Values.Count); i++) {
                m_Dictionary[m_Keys[i]] = m_Values[i];
            }
        }
    
        public void Add(TKey key, TValue value) => m_Dictionary.Add(key, value);
        public bool Remove(TKey key) => m_Dictionary.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value);
        public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);
        public void Clear() => m_Dictionary.Clear();
        public int Count => m_Dictionary.Count;
    
        public TValue this[TKey key] {
            get => m_Dictionary[key];
            set => m_Dictionary[key] = value;
        }
    }
}