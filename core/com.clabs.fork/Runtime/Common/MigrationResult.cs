namespace CLabs.Fork {
    public readonly struct MigrationResult {
        public MigrationResult(bool success, string migratedJson = null, int finalVersion = 0, string errorMessage = null) {
            Success = success;
            MigratedJson = migratedJson;
            FinalVersion = finalVersion;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public string MigratedJson { get; }
        public int FinalVersion { get; }
        public string ErrorMessage { get; }

        public static MigrationResult Ok(string json, int version) => new(true, json, version);
        public static MigrationResult Fail(string error) => new(false, errorMessage: error);
    }
}
