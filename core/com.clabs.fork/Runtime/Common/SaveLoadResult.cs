namespace CLabs.Fork {
    public readonly struct SaveLoadResult<T> where T : class {
        public SaveLoadResult(SaveLoadStatus status, T data = null, SaveSlotInfo slotInfo = null, string message = null) {
            Status = status;
            Data = data;
            SlotInfo = slotInfo;
            Message = message;
        }

        public bool Success => Status is SaveLoadStatus.Success
            or SaveLoadStatus.SuccessFromBackup
            or SaveLoadStatus.SuccessMigrated
            or SaveLoadStatus.SuccessMigratedFromBackup;

        public SaveLoadStatus Status { get; }
        public T Data { get; }
        public SaveSlotInfo SlotInfo { get; }
        public string Message { get; }

        public static SaveLoadResult<T> Ok(T data, SaveSlotInfo info) =>
            new(SaveLoadStatus.Success, data, info);

        public static SaveLoadResult<T> FromBackup(T data, SaveSlotInfo info) =>
            new(SaveLoadStatus.SuccessFromBackup, data, info, "Primary save was corrupt — loaded from backup");

        public static SaveLoadResult<T> Migrated(T data, SaveSlotInfo info) =>
            new(SaveLoadStatus.SuccessMigrated, data, info, "Save migrated from older version");

        public static SaveLoadResult<T> MigratedFromBackup(T data, SaveSlotInfo info) =>
            new(SaveLoadStatus.SuccessMigratedFromBackup, data, info, "Backup loaded and migrated from older version");

        public static SaveLoadResult<T> Fail(SaveLoadStatus status, string message) =>
            new(status, null, null, message);
    }

    public enum SaveLoadStatus {
        Success,
        SuccessFromBackup,
        SuccessMigrated,
        SuccessMigratedFromBackup,
        NoValidSave,
        MigrationFailed,
        ProviderError
    }
}
