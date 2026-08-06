using System;
using System.Collections.Generic;

namespace CLabs.Crumb {
    /// <summary>
    /// An <see cref="ICrumbSink"/> that keeps the most recent log lines in a bounded in-memory ring, for an
    /// in-editor console or a runtime debug view. Thread-safe. Compose it alongside another sink (e.g. the Unity
    /// console) with <see cref="CompositeCrumbSink"/>. Pollers read <see cref="Version"/> to detect changes cheaply
    /// and <see cref="Snapshot"/> to read the entries.
    /// </summary>
    public sealed class BufferedCrumbSink : ICrumbSink {
        private readonly int m_Capacity;
        private readonly Queue<CrumbEntry> m_Entries;
        private readonly object m_Lock = new();

        private int m_Version;

        public BufferedCrumbSink(int capacity = 500) {
            m_Capacity = capacity < 1 ? 1 : capacity;
            m_Entries = new Queue<CrumbEntry>(m_Capacity);
        }

        /// <summary>Increments on every write and clear, so a poller can detect changes without copying the buffer.</summary>
        public int Version {
            get {
                lock (m_Lock) {
                    return m_Version;
                }
            }
        }

        public void Write(string level, string typeName, string message) {
            lock (m_Lock) {
                if (m_Entries.Count >= m_Capacity) {
                    m_Entries.Dequeue();
                }

                m_Entries.Enqueue(new CrumbEntry(level, typeName, message, DateTime.Now));
                m_Version++;
            }
        }

        /// <summary>A copy of the buffered entries, oldest first.</summary>
        public IReadOnlyList<CrumbEntry> Snapshot() {
            lock (m_Lock) {
                return m_Entries.ToArray();
            }
        }

        public void Clear() {
            lock (m_Lock) {
                m_Entries.Clear();
                m_Version++;
            }
        }
    }
}
