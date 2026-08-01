using System;

namespace CLabs.Crumb {
    /// <summary>A bit set of enabled log levels. A logger emits a line only when the flag for that line's level is set.</summary>
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
