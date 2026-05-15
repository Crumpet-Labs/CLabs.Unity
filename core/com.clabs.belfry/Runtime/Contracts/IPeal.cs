using System;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    public interface IPeal {
        void Enqueue(Func<CancellationToken, Ticket> action, int priority = 0);
        int Count { get; }
    }
}