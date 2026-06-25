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

        public void RingBell<T>(in T message) where T : struct
            => m_Belfry.PublishBell(new BellChannel(m_Key, typeof(T)), in message);

        public IDisposable OnBell<T>(BellMessage<T> handler, int priority = 0) where T : struct
            => m_Belfry.SubscribeBell(new BellChannel(m_Key, typeof(T)), handler, priority);

        public IDisposable OnBell(params IBellListener[] listeners) {
            var bindings = new BellBinding[listeners.Length];
                
            for (var i = 0; i < listeners.Length; i++) {
                bindings[i] = new BellBinding(
                    new BellChannel(m_Key, listeners[i].MessageType),
                    listeners[i].Delegate,
                    listeners[i].Priority
                );
            }
            
            return m_Belfry.SubscribeBell(bindings);
        }

        public Ticket RingToll<T>(in T message, int priority = 0) where T : struct {
            if (m_Peal == null) {
                throw new InvalidOperationException(
                    "RingToll requires a peal-configured rope. Pass an IPealConfig to IBellTower.Rope(key, pealConfig)."
                );
            }

            var captured = message;
            var tcs = new TicketCompletionSource();
            var belfry = m_Belfry;
            var channel = new BellChannel(m_Key, typeof(T));

            m_Peal.Enqueue(async ct => {
                try {
                    await belfry.PublishToll(channel, captured, ct);
                    tcs.TrySetResult();
                } catch (OperationCanceledException) {
                    tcs.TrySetCanceled();
                } catch (Exception ex) {
                    tcs.TrySetException(ex);
                }
            }, priority);

            return tcs.Task;
        }

        public IDisposable OnToll<T>(TollMessage<T> handler, int priority = 0) where T : struct
            => m_Belfry.SubscribeToll(new BellChannel(m_Key, typeof(T)), handler, priority);

        public IDisposable OnToll(params ITollListener[] listeners) {
            var bindings = new BellBinding[listeners.Length];
            
            for (var i = 0; i < listeners.Length; i++) {
                bindings[i] = new BellBinding(
                    new BellChannel(m_Key, listeners[i].MessageType),
                    listeners[i].Delegate,
                    listeners[i].Priority
                );
            }
            
            return m_Belfry.SubscribeToll(bindings);
        }
    }
}
