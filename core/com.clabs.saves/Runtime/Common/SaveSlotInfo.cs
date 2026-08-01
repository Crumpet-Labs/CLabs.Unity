using System;

namespace CLabs.Saves {
    /// <summary>Registry record for one save slot: which files back it (current + rolled-over backup) and its metadata.</summary>
    [Serializable]
    public sealed class SaveSlotInfo {
        private string m_SlotId;
        private string m_CurrentFile;
        private string m_BackupFile;
        private DateTime m_LastSaveTime;
        private double m_TotalPlayTimeSeconds;
        private int m_SchemaVersion;
        private bool m_IsAutoSave;

        public string SlotId { get => m_SlotId; set => m_SlotId = value; }
        public string CurrentFile { get => m_CurrentFile; set => m_CurrentFile = value; }
        public string BackupFile { get => m_BackupFile; set => m_BackupFile = value; }
        public DateTime LastSaveTime { get => m_LastSaveTime; set => m_LastSaveTime = value; }
        public double TotalPlayTimeSeconds { get => m_TotalPlayTimeSeconds; set => m_TotalPlayTimeSeconds = value; }
        public int SchemaVersion { get => m_SchemaVersion; set => m_SchemaVersion = value; }
        public bool IsAutoSave { get => m_IsAutoSave; set => m_IsAutoSave = value; }
    }
}
