using System;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public interface IRingOrder {
        void Enqueue(Func<CancellationToken, Ticket> action, int priority);
        bool TryDequeue(out Func<CancellationToken, Ticket> action);
        int Count { get; }
        void Clear();
    }
}
