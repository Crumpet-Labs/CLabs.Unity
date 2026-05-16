using System;

namespace CLabs.Belfry {
    internal readonly struct BellBinding : IEquatable<BellBinding> {
        public BellChannel Channel { get; }
        public Delegate Handler { get; }
        public int Priority { get; }
        public int Sequence { get; }

        public BellBinding(BellChannel channel, Delegate handler, int priority = 0) {
            Channel = channel;
            Handler = handler;
            Priority = priority;
            Sequence = 0;
        }

        internal BellBinding(BellChannel channel, Delegate handler, int priority, int sequence) {
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
