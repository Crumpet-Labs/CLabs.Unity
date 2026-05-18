namespace CLabs.Fork {
    public readonly struct IntegrityResult {
        public IntegrityResult(bool isValid, IntegrityFailureReason reason = IntegrityFailureReason.None) {
            IsValid = isValid;
            Reason = reason;
        }

        public bool IsValid { get; }
        public IntegrityFailureReason Reason { get; }

        public static IntegrityResult Valid() => new(true);
        public static IntegrityResult Fail(IntegrityFailureReason reason) => new(false, reason);
    }

    public enum IntegrityFailureReason {
        None,
        ChecksumMismatch,
        MissingChecksum,
        IncompleteFile,
        EmptyFile,
        UnreadableFormat
    }
}
