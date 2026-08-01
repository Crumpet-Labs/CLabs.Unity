using System;

namespace CLabs.Belfry {
    internal readonly struct BellBinding : IEquatable<BellBinding> {
        
        private readonly BellChannel m_Channel;
        private readonly Delegate m_Handler;
        private readonly int m_Priority;
        private readonly int m_Sequence;

        public BellBinding(BellChannel channel, Delegate handler, int priority = 0) {
            m_Channel = channel;
            m_Handler = handler;
            m_Priority = priority;
            m_Sequence = 0;
        }

        internal BellBinding(BellChannel channel, Delegate handler, int priority, int sequence) {
            m_Channel = channel;
            m_Handler = handler;
            m_Priority = priority;
            m_Sequence = sequence;
        }
        
        public BellChannel Channel => m_Channel;
        public Delegate Handler => m_Handler;
        public int Priority => m_Priority;
        public int Sequence => m_Sequence;

        internal BellBinding WithSequence(int sequence)
            => new BellBinding(Channel, Handler, Priority, sequence);

        public bool Equals(BellBinding other)
            => Channel.Equals(other.Channel)
               && Handler == other.Handler
               && Priority == other.Priority
               && Sequence == other.Sequence;

        public override bool Equals(object obj) => obj is BellBinding other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Channel, Handler, Priority, Sequence);

        public static bool operator ==(BellBinding left, BellBinding right) => left.Equals(right);
        public static bool operator !=(BellBinding left, BellBinding right) => !left.Equals(right);
    }
}
