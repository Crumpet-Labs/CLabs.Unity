namespace CLabs.Crumb {
    /// <summary>Logging configuration: the file-sink toggle, directory, size/retention caps, and the default per-logger level filter. The Unity adapter supplies a ScriptableObject-backed implementation.</summary>
    public interface ICrumbConfiguration {
        bool FileLoggingEnabled { get; }
        string LogDirectory { get; }
        long MaxFileSizeBytes { get; }
        int MaxFileCount { get; }
        CrumbFilters DefaultFilters { get; }
    }
}
