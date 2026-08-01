using System;
using System.Collections.Generic;

namespace CLabs.Belfry {
    /// <summary>
    /// The bindings registered on one channel, kept sorted by priority then subscription order, with a
    /// copy-on-write snapshot. <see cref="Snapshot"/> rebuilds the array only after the listener set changes,
    /// so publishing on an unchanged channel allocates nothing. The returned array is treated as immutable —
    /// mutations replace the reference rather than editing in place, so an in-flight dispatch that captured an
    /// earlier snapshot is never disturbed by a subscribe/unsubscribe during that dispatch.
    /// </summary>
    internal sealed class ListenerLane {
        private readonly List<BellBinding> m_Bindings = new();
        private BellBinding[] m_Snapshot = Array.Empty<BellBinding>();
        private bool m_Dirty;

        public void Insert(BellBinding binding) {
            var lo = 0;
            var hi = m_Bindings.Count;
            while (lo < hi) {
                var mid = (lo + hi) >> 1;
                if (m_Bindings[mid].ComparePriority(binding) <= 0) lo = mid + 1;
                else hi = mid;
            }

            m_Bindings.Insert(lo, binding);
            m_Dirty = true;
        }

        public void Remove(BellBinding binding) {
            if (m_Bindings.Remove(binding)) m_Dirty = true;
        }

        public BellBinding[] Snapshot() {
            if (m_Dirty) {
                m_Snapshot = m_Bindings.ToArray();
                m_Dirty = false;
            }

            return m_Snapshot;
        }
    }

    internal static class ListenerLaneExtensions {
        public static int ComparePriority(this BellBinding a, BellBinding b) {
            var p = b.Priority.CompareTo(a.Priority);
            return p != 0 ? p : a.Sequence.CompareTo(b.Sequence);
        }
    }
}
