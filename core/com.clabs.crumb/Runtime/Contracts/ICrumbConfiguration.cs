namespace CLabs.Crumb {
    public interface ICrumbConfiguration {
        bool FileLoggingEnabled { get; }
        string LogDirectory { get; }
        long MaxFileSizeBytes { get; }
        int MaxFileCount { get; }
        CrumbFilters DefaultFilters { get; }
    }
}
