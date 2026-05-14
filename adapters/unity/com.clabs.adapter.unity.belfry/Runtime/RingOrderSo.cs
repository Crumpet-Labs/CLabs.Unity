using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    public abstract class RingOrderSo : ScriptableObject, IRingOrder {
        protected abstract IRingOrder RingOrder { get; }

        private void OnEnable() => RingOrder?.Clear();

        public void Enqueue(System.Func<System.Threading.CancellationToken, CLabs.Tickets.Ticket> action, int priority)
            => RingOrder.Enqueue(action, priority);

        public bool TryDequeue(out System.Func<System.Threading.CancellationToken, CLabs.Tickets.Ticket> action)
            => RingOrder.TryDequeue(out action);

        public int Count => RingOrder.Count;

        public void Clear() => RingOrder.Clear();
    }
}
