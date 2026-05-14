using System.Collections.Generic;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    [CreateAssetMenu(menuName = "Belfry/Peal Config")]
    public sealed class PealConfigSo : ScriptableObject, IPealConfig {
        [SerializeField] private RingOrderSo m_Strategy;
        [SerializeField] private List<int> m_CriticalPriorities = new();

        private HashSet<int> m_CriticalSet;

        private void OnEnable() => m_CriticalSet = null;

        public IRingOrder Strategy => m_Strategy;

        public bool IsCritical(int priority) {
            m_CriticalSet ??= new HashSet<int>(m_CriticalPriorities);
            return m_CriticalSet.Contains(priority);
        }
    }
}
