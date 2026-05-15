using System;

namespace CLabs.Belfry {
    public interface IBellListener {
        Type MessageType { get; }
        Delegate Delegate { get; }
        int Priority { get; }
    }
}
