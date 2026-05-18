namespace CLabs.Fork {
    public readonly struct SaveResult {
        public SaveResult(bool success, string filePath = null, SaveFailureReason reason = SaveFailureReason.None) {
            Success = success;
            FilePath = filePath;
            Reason = reason;
        }

        public bool Success { get; }
        public string FilePath { get; }
        public SaveFailureReason Reason { get; }

        public static SaveResult Ok(string filePath) => new(true, filePath);
        public static SaveResult Fail(SaveFailureReason reason) => new(false, null, reason);
    }

    public enum SaveFailureReason {
        None,
        SaveAlreadyInProgress,
        SerializationFailed,
        WriteFailed,
        PostWriteValidationFailed,
        Unknown
    }
}
