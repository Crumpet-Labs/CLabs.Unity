using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    internal sealed class Belfry : IBelfry {
        private readonly Dictionary<BellChannel, List<BellBinding>> m_BellListeners = new();
        private readonly Dictionary<BellChannel, List<BellBinding>> m_TollListeners = new();
        private int m_NextSequence;

        public IDisposable SubscribeBell(BellChannel channel, Delegate handler, int priority = 0) {
            var binding = new BellBinding(channel, handler, priority, m_NextSequence++);
            InsertSorted(m_BellListeners, binding);
            return new LaneSubscription(m_BellListeners, new[] { binding });
        }

        public IDisposable SubscribeBell(IReadOnlyList<BellBinding> bindings) {
            var sequenced = new BellBinding[bindings.Count];
            for (var i = 0; i < bindings.Count; i++) {
                sequenced[i] = bindings[i].WithSequence(m_NextSequence++);
                InsertSorted(m_BellListeners, sequenced[i]);
            }
            return new LaneSubscription(m_BellListeners, sequenced);
        }

        public void PublishBell<T>(BellChannel channel, in T message) where T : struct {
            if (false == m_BellListeners.TryGetValue(channel, out var list)) return;

            var snapshot = list.ToArray();
            for (var i = 0; i < snapshot.Length; i++) {
                if (snapshot[i].Handler is BellMessage<T> bell) bell.Invoke(in message);
            }
        }

        public IReadOnlyList<BellBinding> GetBellBindings(BellChannel channel) {
            if (m_BellListeners.TryGetValue(channel, out var list))
                return list.ToArray();
            return Array.Empty<BellBinding>();
        }

        public IDisposable SubscribeToll(BellChannel channel, Delegate handler, int priority = 0) {
            var binding = new BellBinding(channel, handler, priority, m_NextSequence++);
            InsertSorted(m_TollListeners, binding);
            return new LaneSubscription(m_TollListeners, new[] { binding });
        }

        public IDisposable SubscribeToll(IReadOnlyList<BellBinding> bindings) {
            var sequenced = new BellBinding[bindings.Count];
            for (var i = 0; i < bindings.Count; i++) {
                sequenced[i] = bindings[i].WithSequence(m_NextSequence++);
                InsertSorted(m_TollListeners, sequenced[i]);
            }
            return new LaneSubscription(m_TollListeners, sequenced);
        }

        public async Ticket PublishToll<T>(BellChannel channel, T message, CancellationToken ct) where T : struct {
            if (false == m_TollListeners.TryGetValue(channel, out var list)) return;

            var snapshot = list.ToArray();
            for (var i = 0; i < snapshot.Length; i++) {
                ct.ThrowIfCancellationRequested();
                if (snapshot[i].Handler is TollMessage<T> toll) await toll.Invoke(message);
            }
        }

        public IReadOnlyList<BellBinding> GetTollBindings(BellChannel channel) {
            if (m_TollListeners.TryGetValue(channel, out var list))
                return list.ToArray();
            return Array.Empty<BellBinding>();
        }

        private static void InsertSorted(Dictionary<BellChannel, List<BellBinding>> lane, BellBinding binding) {
            if (false == lane.TryGetValue(binding.Channel, out var list)) {
                list = new List<BellBinding>();
                lane[binding.Channel] = list;
            }

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

        private sealed class LaneSubscription : IDisposable {
            private readonly Dictionary<BellChannel, List<BellBinding>> m_Lane;
            private readonly BellBinding[] m_Bindings;

            public LaneSubscription(Dictionary<BellChannel, List<BellBinding>> lane, BellBinding[] bindings) {
                m_Lane = lane;
                m_Bindings = bindings;
            }

            public void Dispose() {
                foreach (var binding in m_Bindings) {
                    if (m_Lane.TryGetValue(binding.Channel, out var list))
                        list.Remove(binding);
                }
            }
        }
    }
}
