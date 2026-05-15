using System;
using System.Collections.Generic;

namespace CLabs.Belfry {
    public sealed class Belfry : IBelfry {
        private readonly Dictionary<BellChannel, List<BellBinding>> m_Listeners = new();
        private int m_NextSequence;

        public IDisposable Subscribe(in BellBinding binding, int priority = 0) {
            InsertSorted(binding);
            return new BellSubscription(this, new[] { binding });
        }

        public IDisposable Subscribe(IReadOnlyList<BellBinding> bindings) {
            var sequenced = new BellBinding[bindings.Count];
            for (var i = 0; i < bindings.Count; i++) {
                sequenced[i] = bindings[i].WithSequence(m_NextSequence++);
                InsertSorted(sequenced[i]);
            }
            return new BellSubscription(this, sequenced);
        }

        public void Publish<T>(in BellChannel channel, in T message) where T : struct {
            if (m_Listeners.TryGetValue(channel, out var list)) {
                // Snapshot to tolerate (un)subscribe-during-dispatch re-entrancy.
                var snapshot = list.ToArray();
                foreach (var binding in snapshot) {
                    if (binding.Handler is BellMessage<T> bell) bell.Invoke(in message);
                }
            }
        }

        public IReadOnlyList<BellBinding> GetBindings(in BellChannel channel) {
            if (m_Listeners.TryGetValue(channel, out var list))
                return list.ToArray();
            return Array.Empty<BellBinding>();
        }

        private void InsertSorted(BellBinding binding) {
            if (false == m_Listeners.TryGetValue(binding.Channel, out var list)) {
                list = new List<BellBinding>();
                m_Listeners[binding.Channel] = list;
            }

            // Binary search for insertion point: priority desc, sequence asc within ties (FIFO).
            var lo = 0;
            var hi = list.Count;
            while (lo < hi) {
                var mid = (lo + hi) >> 1;
                if (ComparePriority(list[mid], binding) <= 0) lo = mid + 1;
                else hi = mid;
            }
            list.Insert(lo, binding);
        }

        private static int ComparePriority(BellBinding a, BellBinding b) {
            var p = b.Priority.CompareTo(a.Priority);
            return p != 0 ? p : a.Sequence.CompareTo(b.Sequence);
        }

        private sealed class BellSubscription : IDisposable {
            private readonly Belfry m_Belfry;
            private readonly BellBinding[] m_Bindings;

            public BellSubscription(Belfry belfry, BellBinding[] bindings) {
                m_Belfry = belfry;
                m_Bindings = bindings;
            }

            public void Dispose() {
                foreach (var binding in m_Bindings) {
                    if (m_Belfry.m_Listeners.TryGetValue(binding.Channel, out var list))
                        list.Remove(binding);
                }
            }
        }
    }
}
