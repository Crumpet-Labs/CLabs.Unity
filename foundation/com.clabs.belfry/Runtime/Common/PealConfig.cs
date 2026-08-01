using System.Collections.Generic;

namespace CLabs.Belfry {
    public sealed class PealConfig : IPealConfig {
        private readonly IRingOrder m_Strategy;
        private readonly HashSet<int> m_CriticalPriorities;

        public PealConfig(IRingOrder strategy, IEnumerable<int> criticalPriorities = null) {
            m_Strategy = strategy;
            m_CriticalPriorities = criticalPriorities != null
                ? new HashSet<int>(criticalPriorities)
                : new HashSet<int>();
        }

        public IRingOrder Strategy => m_Strategy;

        public bool IsCritical(int priority) {
            return m_CriticalPriorities.Contains(priority);
        }
    }
}
