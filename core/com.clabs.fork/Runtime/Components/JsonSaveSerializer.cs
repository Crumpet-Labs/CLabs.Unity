using System;
using System.Text;
using Newtonsoft.Json;

namespace CLabs.Fork {
    public sealed class JsonSaveSerializer : ISaveSerializer {
        private readonly int m_CurrentSchemaVersion;

        public JsonSaveSerializer(IForkConfiguration configuration) {
            m_CurrentSchemaVersion = configuration.CurrentSchemaVersion;
        }

        public int CurrentSchemaVersion => m_CurrentSchemaVersion;

        public byte[] Serialize<T>(T data) where T : class {
            var dataJson = JsonConvert.SerializeObject(data);

            var envelope = new SaveEnvelope {
                SchemaVersion = m_CurrentSchemaVersion,
                Timestamp = DateTime.UtcNow.ToString("o"),
                DataJson = dataJson
            };

            var envelopeJson = JsonConvert.SerializeObject(envelope);
            return Encoding.UTF8.GetBytes(envelopeJson);
        }

        public SaveDeserializeResult<T> Deserialize<T>(byte[] data) where T : class {
            if (data == null || data.Length == 0) {
                return SaveDeserializeResult<T>.Fail("Data is null or empty");
            }

            try {
                var envelopeJson = Encoding.UTF8.GetString(data);
                var envelope = JsonConvert.DeserializeObject<SaveEnvelope>(envelopeJson);

                if (envelope == null) {
                    return SaveDeserializeResult<T>.Fail("Failed to deserialize save envelope");
                }

                var result = JsonConvert.DeserializeObject<T>(envelope.DataJson);

                if (result == null) {
                    return SaveDeserializeResult<T>.Fail("Failed to deserialize save data");
                }

                return SaveDeserializeResult<T>.Ok(result, envelope.SchemaVersion);
            }
            catch (Exception ex) {
                return SaveDeserializeResult<T>.Fail($"Deserialization error: {ex.Message}");
            }
        }
    }
}
