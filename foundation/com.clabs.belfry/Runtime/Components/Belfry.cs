using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    internal sealed class Belfry : IBelfry {
        private readonly Dictionary<BellChannel, ListenerLane> m_BellListeners = new();
        private readonly Dictionary<BellChannel, ListenerLane> m_TollListeners = new();

        private int m_NextSequence;

        public IDisposable SubscribeBell(BellChannel channel, Delegate handler, int priority = 0) {
            var binding = new BellBinding(channel, handler, priority, m_NextSequence++);

            m_BellListeners.InsertSorted(binding);

            return new LaneSubscription(m_BellListeners, new[] { binding });
        }

        public IDisposable SubscribeBell(IReadOnlyList<BellBinding> bindings) {
            var sequenced = new BellBinding[bindings.Count];

            for (var i = 0; i < bindings.Count; i++) {
                sequenced[i] = bindings[i].WithSequence(m_NextSequence++);
                m_BellListeners.InsertSorted(sequenced[i]);
            }

            return new LaneSubscription(m_BellListeners, sequenced);
        }

        public void PublishBell<T>(BellChannel channel, in T message) where T : struct {
            if (false == m_BellListeners.TryGetValue(channel, out var lane)) return;

            var snapshot = lane.Snapshot();
            for (var i = 0; i < snapshot.Length; i++) {
                if (snapshot[i].Handler is BellMessage<T> bell) bell.Invoke(in message);
            }
        }

        public IReadOnlyList<BellBinding> GetBellBindings(BellChannel channel) {
            return m_BellListeners.TryGetValue(channel, out var lane)
                ? lane.Snapshot()
                : Array.Empty<BellBinding>();
        }

        public IDisposable SubscribeToll(BellChannel channel, Delegate handler, int priority = 0) {
            var binding = new BellBinding(channel, handler, priority, m_NextSequence++);
            m_TollListeners.InsertSorted(binding);

            return new LaneSubscription(m_TollListeners, new[] { binding });
        }

        public IDisposable SubscribeToll(IReadOnlyList<BellBinding> bindings) {
            var sequenced = new BellBinding[bindings.Count];
            for (var i = 0; i < bindings.Count; i++) {
                sequenced[i] = bindings[i].WithSequence(m_NextSequence++);
                m_TollListeners.InsertSorted(sequenced[i]);
            }

            return new LaneSubscription(m_TollListeners, sequenced);
        }

        public async Ticket PublishToll<T>(BellChannel channel, T message, CancellationToken ct) where T : struct {
            if (false == m_TollListeners.TryGetValue(channel, out var lane)) return;

            var snapshot = lane.Snapshot();
            for (var i = 0; i < snapshot.Length; i++) {
                ct.ThrowIfCancellationRequested();
                if (snapshot[i].Handler is TollMessage<T> toll) await toll.Invoke(message);
            }
        }

        public IReadOnlyList<BellBinding> GetTollBindings(BellChannel channel) {
            return m_TollListeners.TryGetValue(channel, out var lane)
                ? lane.Snapshot()
                : Array.Empty<BellBinding>();
        }

        private sealed class LaneSubscription : IDisposable {
            private readonly Dictionary<BellChannel, ListenerLane> m_Lanes;
            private readonly BellBinding[] m_Bindings;

            public LaneSubscription(Dictionary<BellChannel, ListenerLane> lanes, BellBinding[] bindings) {
                m_Lanes = lanes;
                m_Bindings = bindings;
            }

            public void Dispose() {
                foreach (var binding in m_Bindings) {
                    if (m_Lanes.TryGetValue(binding.Channel, out var lane))
                        lane.Remove(binding);
                }
            }
        }
    }

    internal static class BelfryInternals {
        public static void InsertSorted(this Dictionary<BellChannel, ListenerLane> lanes, BellBinding binding) {
            if (false == lanes.TryGetValue(binding.Channel, out var lane)) {
                lane = new ListenerLane();
                lanes[binding.Channel] = lane;
            }

            lane.Insert(binding);
        }
    }
}
