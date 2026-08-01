using System;

namespace CLabs.Crumb {
    /// <summary>A captured log line: its level tag, the owning type's name, the message, and when it was written.</summary>
    public readonly struct CrumbEntry {
        private readonly string m_Level;
        private readonly string m_TypeName;
        private readonly string m_Message;
        private readonly DateTime m_Timestamp;

        public CrumbEntry(string level, string typeName, string message, DateTime timestamp) {
            m_Level = level;
            m_TypeName = typeName;
            m_Message = message;
            m_Timestamp = timestamp;
        }

        public string Level => m_Level;
        public string TypeName => m_TypeName;
        public string Message => m_Message;
        public DateTime Timestamp => m_Timestamp;
    }
}
