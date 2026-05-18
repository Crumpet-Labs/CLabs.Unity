using System;

namespace CLabs.Fork {
    [Serializable]
    public sealed class SaveEnvelope {
        public int SchemaVersion;
        public string Timestamp;
        public double TotalPlayTimeSeconds;
        public string DataJson;
        public string Checksum;
    }
}
