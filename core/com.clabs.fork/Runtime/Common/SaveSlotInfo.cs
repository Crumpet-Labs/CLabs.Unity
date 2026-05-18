using System;

namespace CLabs.Fork {
    [Serializable]
    public sealed class SaveSlotInfo {
        public string SlotId;
        public string CurrentFile;
        public string BackupFile;
        public DateTime LastSaveTime;
        public double TotalPlayTimeSeconds;
        public int SchemaVersion;
        public bool IsAutoSave;
    }
}
