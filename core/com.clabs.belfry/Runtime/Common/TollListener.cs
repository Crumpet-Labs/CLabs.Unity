using System;

namespace CLabs.Belfry {
    public sealed class TollListener<T> : ITollListener where T : struct {
        public TollListener(TollMessage<T> handler, int priority = 0) {
            Delegate = handler;
            Priority = priority;
        }

        public Type MessageType => typeof(T);
        public Delegate Delegate { get; }
        public int Priority { get; }
    }
}
