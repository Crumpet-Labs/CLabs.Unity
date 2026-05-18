namespace CLabs.Crumb {
    public sealed class CrumbConfiguration : ICrumbConfiguration {
        public bool FileLoggingEnabled { get; }
        public string LogDirectory { get; }
        public long MaxFileSizeBytes { get; }
        public int MaxFileCount { get; }
        public CrumbFilters DefaultFilters { get; }

        public CrumbConfiguration(
            string logDirectory = "Logs",
            bool fileLoggingEnabled = true,
            long maxFileSizeBytes = 5_242_880,
            int maxFileCount = 5,
            CrumbFilters defaultFilters = CrumbFilters.All) {
            LogDirectory = logDirectory;
            FileLoggingEnabled = fileLoggingEnabled;
            MaxFileSizeBytes = maxFileSizeBytes;
            MaxFileCount = maxFileCount;
            DefaultFilters = defaultFilters;
        }
    }
}
