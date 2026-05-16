using System;
using System.Collections.Generic;
using System.Threading;
using CLabs.Tickets;

namespace CLabs.Belfry {
    internal interface IBelfry {
        IDisposable SubscribeBell(BellChannel channel, Delegate handler, int priority = 0);
        IDisposable SubscribeBell(IReadOnlyList<BellBinding> bindings);
        void PublishBell<T>(BellChannel channel, in T message) where T : struct;
        IReadOnlyList<BellBinding> GetBellBindings(BellChannel channel);

        IDisposable SubscribeToll(BellChannel channel, Delegate handler, int priority = 0);
        IDisposable SubscribeToll(IReadOnlyList<BellBinding> bindings);
        Ticket PublishToll<T>(BellChannel channel, T message, CancellationToken ct) where T : struct;
        IReadOnlyList<BellBinding> GetTollBindings(BellChannel channel);
    }
}
