namespace CLabs.Crumb {
    public sealed class CrumbConfiguration : ICrumbConfiguration {
        private readonly bool m_FileLoggingEnabled;
        private readonly string m_LogDirectory;
        private readonly long m_MaxFileSizeBytes;
        private readonly int m_MaxFileCount;
        private readonly CrumbFilters m_DefaultFilters;
        
        public CrumbConfiguration(
            string logDirectory = "Logs",
            bool fileLoggingEnabled = true,
            long maxFileSizeBytes = 5_242_880,
            int maxFileCount = 5,
            CrumbFilters defaultFilters = CrumbFilters.All) {
            m_LogDirectory = logDirectory;
            m_FileLoggingEnabled = fileLoggingEnabled;
            m_MaxFileSizeBytes = maxFileSizeBytes;
            m_MaxFileCount = maxFileCount;
            m_DefaultFilters = defaultFilters;
        }
        
        public bool FileLoggingEnabled => m_FileLoggingEnabled; 
        public string LogDirectory => m_LogDirectory; 
        public long MaxFileSizeBytes => m_MaxFileSizeBytes; 
        public int MaxFileCount => m_MaxFileCount; 
        public CrumbFilters DefaultFilters => m_DefaultFilters; 
    }
}
