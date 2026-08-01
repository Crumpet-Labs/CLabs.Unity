using System;

namespace CLabs.Belfry {
    public interface ITollListener {
        Type MessageType { get; }
        Delegate Delegate { get; }
        int Priority { get; }
    }
}
