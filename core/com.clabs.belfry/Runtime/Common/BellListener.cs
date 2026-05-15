using System;

namespace CLabs.Belfry {
    public sealed class BellListener<T> : IBellListener where T : struct {
        public BellListener(BellMessage<T> handler, int priority = 0) {
            Delegate = handler;
            Priority = priority;
        }

        public Type MessageType => typeof(T);
        public Delegate Delegate { get; }
        public int Priority { get; }
    }
}
