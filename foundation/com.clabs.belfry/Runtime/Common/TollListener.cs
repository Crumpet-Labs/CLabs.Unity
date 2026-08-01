using System;

namespace CLabs.Belfry {
    public sealed class TollListener<T> : ITollListener where T : struct {
        private readonly Delegate m_Delegate;
        private readonly int m_Priority;
        
        public TollListener(TollMessage<T> handler, int priority = 0) {
            m_Delegate = handler;
            m_Priority = priority;
        }

        public Type MessageType => typeof(T);
        public Delegate Delegate => m_Delegate;
        public int Priority => m_Priority;
    }
}
