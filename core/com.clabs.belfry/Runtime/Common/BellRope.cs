using System;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public readonly struct BellRope {
        private readonly object m_Key;
        private readonly IBelfry m_Belfry;
        private readonly IPeal m_Peal;

        internal BellRope(object key, IBelfry belfry, IPeal peal) {
            m_Key = key;
            m_Belfry = belfry;
            m_Peal = peal;
        }

        public void Ring<T>(in T message) where T : struct
            => m_Belfry.Publish(new BellChannel(m_Key, typeof(T)), in message);

        public Ticket RingAsync<T>(in T message, int priority = 0) where T : struct {
            var captured = message;
            var tcs = new TicketCompletionSource();
            var belfry = m_Belfry;
            var channel = new BellChannel(m_Key, typeof(T));

            m_Peal.Enqueue(ct => {
                belfry.Publish(channel, in captured);
                tcs.TrySetResult();
                return tcs.Task;
            }, priority);
            return tcs.Task;
        }

        public IDisposable On<T>(BellMessage<T> handler, int priority = 0) where T : struct {
            var channel = new BellChannel(m_Key, typeof(T));
            var binding = new BellBinding(channel, handler, priority);
            return m_Belfry.Subscribe(binding);
        }

        public IDisposable On(params IBellListener[] listeners) {
            var bindings = new BellBinding[listeners.Length];
            for (var i = 0; i < listeners.Length; i++)
                bindings[i] = new BellBinding(
                    new BellChannel(m_Key, listeners[i].MessageType),
                    listeners[i].Delegate,
                    listeners[i].Priority);
            return m_Belfry.Subscribe(bindings);
        }
    }
}
