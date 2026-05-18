using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace CLabs.Fork {
    public sealed class Sha256IntegrityValidator : ISaveIntegrityValidator {
        public string GenerateChecksum(byte[] data) {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return hash.ToHexString();
        }

        public IntegrityResult Validate(byte[] rawFile) {
            if (rawFile == null || rawFile.Length == 0) {
                return IntegrityResult.Fail(IntegrityFailureReason.EmptyFile);
            }

            SaveEnvelope envelope;

            try {
                var json = Encoding.UTF8.GetString(rawFile);
                envelope = JsonConvert.DeserializeObject<SaveEnvelope>(json);
            }
            catch {
                return IntegrityResult.Fail(IntegrityFailureReason.UnreadableFormat);
            }

            if (envelope == null) {
                return IntegrityResult.Fail(IntegrityFailureReason.IncompleteFile);
            }

            if (string.IsNullOrEmpty(envelope.Checksum)) {
                return IntegrityResult.Fail(IntegrityFailureReason.MissingChecksum);
            }

            if (string.IsNullOrEmpty(envelope.DataJson)) {
                return IntegrityResult.Fail(IntegrityFailureReason.IncompleteFile);
            }

            var dataBytes = Encoding.UTF8.GetBytes(envelope.DataJson);
            var computed = GenerateChecksum(dataBytes);

            if (!string.Equals(computed, envelope.Checksum, StringComparison.OrdinalIgnoreCase)) {
                return IntegrityResult.Fail(IntegrityFailureReason.ChecksumMismatch);
            }

            return IntegrityResult.Valid();
        }
    }
}
