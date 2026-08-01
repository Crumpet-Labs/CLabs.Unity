using System;

namespace CLabs.Belfry {
    internal readonly struct BellChannel : IEquatable<BellChannel> {
        private readonly object m_Scope;
        private readonly Type m_MessageType;

        public BellChannel(object scope, Type messageType) {
            m_Scope = scope;
            m_MessageType = messageType;
        }
        
        public object Scope => m_Scope;
        public Type MessageType => m_MessageType;
        
        public bool Equals(BellChannel other)
            => Equals(m_Scope, other.Scope) && other.MessageType == m_MessageType;
        
        public override bool Equals(object obj)
            => obj is BellChannel other && Equals(other);
        
        public override int GetHashCode()
            => (m_Scope, m_MessageType).GetHashCode();
        
        public static bool operator ==(BellChannel left, BellChannel right)
            => left.Equals(right);
        
        public static bool operator !=(BellChannel left, BellChannel right)
            => !left.Equals(right);
    }
}