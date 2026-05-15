using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public sealed class StrictPriorityRingOrder : IRingOrder {
        private readonly Dictionary<int, Queue<Func<CancellationToken, Ticket>>> m_PriorityQueues = new();
        private readonly List<int> m_ActivePriorities = new();

        public void Enqueue(Func<CancellationToken, Ticket> action, int priority) {
            if (false == m_PriorityQueues.TryGetValue(priority, out var queue)) {
                queue = new Queue<Func<CancellationToken, Ticket>>();
                m_PriorityQueues[priority] = queue;
                m_ActivePriorities.Add(priority);
                m_ActivePriorities.Sort((a, b) => a.CompareTo(b));
            }
            queue.Enqueue(action);
        }

        public bool TryDequeue(out Func<CancellationToken, Ticket> action) {
            action = null;
            if (m_ActivePriorities.Count == 0) return false;

            for (var i = m_ActivePriorities.Count - 1; i >= 0; i--) {
                var priority = m_ActivePriorities[i];
                var queue = m_PriorityQueues[priority];

                if (queue.Count <= 0) {
                    m_PriorityQueues.Remove(priority);
                    m_ActivePriorities.RemoveAt(i);
                    continue;
                }

                action = queue.Dequeue();

                if (queue.Count == 0) {
                    m_PriorityQueues.Remove(priority);
                    m_ActivePriorities.RemoveAt(i);
                }

                return true;
            }

            return false;
        }

        public int Count => m_PriorityQueues.Values.Sum(q => q.Count);

        public void Clear() {
            m_PriorityQueues.Clear();
            m_ActivePriorities.Clear();
        }
    }
}
