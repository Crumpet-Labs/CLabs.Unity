using System;
using System.Collections.Generic;

namespace CLabs.Belfry {
    public interface IBelfry {
        IDisposable Subscribe(in BellBinding binding, int priority = 0);
        IDisposable Subscribe(IReadOnlyList<BellBinding> bindings);
        void Publish<T>(in BellChannel channel, in T message) where T : struct;
        IReadOnlyList<BellBinding> GetBindings(in BellChannel channel);
    }
}
