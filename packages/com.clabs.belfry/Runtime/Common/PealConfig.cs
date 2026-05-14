using System.Collections.Generic;

namespace CLabs.Belfry {
    public sealed class PealConfig : IPealConfig {
        private readonly HashSet<int> m_CriticalPriorities;

        public IRingOrder Strategy { get; }

        public PealConfig(IRingOrder strategy, IEnumerable<int> criticalPriorities = null) {
            Strategy = strategy;
            m_CriticalPriorities = criticalPriorities != null
                ? new HashSet<int>(criticalPriorities)
                : new HashSet<int>();
        }

        public bool IsCritical(int priority) => m_CriticalPriorities.Contains(priority);
    }
}
