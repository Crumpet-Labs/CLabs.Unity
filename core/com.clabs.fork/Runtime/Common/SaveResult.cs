namespace CLabs.Fork {
    public enum SaveFailureReason {
        None,
        SaveAlreadyInProgress,
        SerializationFailed,
        WriteFailed,
        PostWriteValidationFailed,
        Unknown
    }
    
    public readonly struct SaveResult {
        private readonly bool m_Success;
        private readonly string m_FilePath;
        private readonly SaveFailureReason m_Reason;
        
        public SaveResult(bool success, string filePath = null, SaveFailureReason reason = SaveFailureReason.None) {
            m_Success = success;
            m_FilePath = filePath;
            m_Reason = reason;
        }

        public bool Success => m_Success;
        public string FilePath => m_FilePath;
        public SaveFailureReason Reason => m_Reason;

        public static SaveResult Ok(string filePath) {
            return new(true, filePath);
        }
        
        public static SaveResult Fail(SaveFailureReason reason) {
            return new(false, null, reason);
        }
    }
}
