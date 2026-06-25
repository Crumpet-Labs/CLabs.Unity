namespace CLabs.Fork {
    public readonly struct MigrationResult {
        private readonly bool m_Success;
        private readonly string m_MigratedJson;
        private readonly int m_FinalVersion;
        private readonly string m_ErrorMessage;
        
        public MigrationResult(bool success, string migratedJson = null, int finalVersion = 0, string errorMessage = null) {
            m_Success = success;
            m_MigratedJson = migratedJson;
            m_FinalVersion = finalVersion;
            m_ErrorMessage = errorMessage;
        }

        public bool Success => m_Success;
        public string MigratedJson => m_MigratedJson;
        public int FinalVersion => m_FinalVersion;
        public string ErrorMessage => m_ErrorMessage;

        public static MigrationResult Ok(string json, int version) {
            return new(true, json, version);
        }
        
        public static MigrationResult Fail(string error) {
            return new(false, errorMessage: error);
        }
    }
}
