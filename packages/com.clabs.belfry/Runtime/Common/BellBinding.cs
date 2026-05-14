using System;

namespace CLabs.Belfry {
    /// <summary>
    /// A stored listener binding: channel + handler + priority + sequence.
    /// Sequence is assigned by the Belfry on Subscribe and used as a FIFO tie-breaker
    /// when two bindings on the same channel share a priority. Externally-constructed
    /// bindings have Sequence = 0 until the Belfry stamps them.
    /// </summary>
    public readonly struct BellBinding : IEquatable<BellBinding> {
        public BellChannel Channel { get; }
        public Delegate Handler { get; }
        public int Priority { get; }
        public int Sequence { get; }

        public BellBinding(in BellChannel channel, Delegate handler, int priority = 0) {
            Channel = channel;
            Handler = handler;
            Priority = priority;
            Sequence = 0;
        }

        internal BellBinding(in BellChannel channel, Delegate handler, int priority, int sequence) {
            Channel = channel;
            Handler = handler;
            Priority = priority;
            Sequence = sequence;
        }

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
