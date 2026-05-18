using System;

namespace CLabs.Crumb {
    [Flags]
    public enum CrumbFilters {
        None    = 0,
        Verbose = 1,
        Info    = 2,
        Warning = 4,
        Error   = 8,
        Fatal   = 16,
        All     = Verbose | Info | Warning | Error | Fatal
    }
}
