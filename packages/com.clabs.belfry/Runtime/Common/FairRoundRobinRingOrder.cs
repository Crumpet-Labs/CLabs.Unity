using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public sealed class FairRoundRobinRingOrder : IRingOrder {
        private readonly Dictionary<int, Queue<Func<CancellationToken, Ticket>>> m_PriorityQueues = new();
        private readonly List<int> m_ActivePriorities = new();
        private int m_CurrentPriorityIndex;

        public void Enqueue(Func<CancellationToken, Ticket> action, int priority) {
            if (false == m_PriorityQueues.TryGetValue(priority, out var queue)) {
                queue = new Queue<Func<CancellationToken, Ticket>>();
                m_PriorityQueues[priority] = queue;
                m_ActivePriorities.Add(priority);
                m_ActivePriorities.Sort((a, b) => b.CompareTo(a));
            }
            queue.Enqueue(action);
        }

        public bool TryDequeue(out Func<CancellationToken, Ticket> action) {
            action = null;
            if (m_ActivePriorities.Count == 0) return false;

            var attempts = 0;
            
            while (attempts < m_ActivePriorities.Count) {
                m_CurrentPriorityIndex = (m_CurrentPriorityIndex + 1) % m_ActivePriorities.Count;
                var priority = m_ActivePriorities[m_CurrentPriorityIndex];
                var queue = m_PriorityQueues[priority];

                if (queue.Count > 0) {
                    action = queue.Dequeue();

                    if (queue.Count != 0) return true;

                    m_PriorityQueues.Remove(priority);
                    m_ActivePriorities.RemoveAt(m_CurrentPriorityIndex);
                    m_CurrentPriorityIndex = Math.Max(0, m_CurrentPriorityIndex - 1);

                    return true;
                }

                attempts++;
            }

            return false;
        }

        public int Count => m_PriorityQueues.Values.Sum(q => q.Count);

        public void Clear() {
            m_PriorityQueues.Clear();
            m_ActivePriorities.Clear();
            m_CurrentPriorityIndex = 0;
        }
    }
}
