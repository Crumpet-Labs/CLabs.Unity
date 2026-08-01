using System;
using System.Text;
using CLabs.Crumb;
using CLabs.Tickets;
using Newtonsoft.Json;

namespace CLabs.Saves {
    public sealed class SavesService : ISavesService {
        private const string RegistryFile = "_saves_index.json";
        private const string LegacyRegistryFile = "_fork_index.json";

        private readonly ISaveDataProvider m_Provider;
        private readonly ISaveSerializer m_Serializer;
        private readonly ISaveIntegrityValidator m_Validator;
        private readonly CrumbLogger m_Logger;
        private readonly SaveSlotRegistry m_SlotRegistry;
        private readonly SaveMigrationPipeline m_MigrationPipeline;

        private bool m_SaveInProgress;

        public SavesService(
            ISaveDataProvider provider,
            ISaveSerializer serializer,
            ISaveIntegrityValidator validator,
            CrumbLogger logger,
            SaveSlotRegistry slotRegistry,
            SaveMigrationPipeline migrationPipeline
        ) {
            m_Provider = provider;
            m_Serializer = serializer;
            m_Validator = validator;
            m_Logger = logger;
            m_Logger.Initialize(typeof(SavesService));
            m_SlotRegistry = slotRegistry;
            m_MigrationPipeline = migrationPipeline;
        }

        public async Ticket LoadRegistryAsync() {
            var data = await m_Provider.ReadAsync(RegistryFile);
            var migratedFromLegacy = false;

            if (data == null || data.Length == 0) {
                data = await m_Provider.ReadAsync(LegacyRegistryFile);
                migratedFromLegacy = data != null && data.Length > 0;
            }

            if (data != null && data.Length > 0) {
                var loaded = SaveSlotRegistry.FromBytes(data);
                foreach (var slot in loaded.GetAllSlots()) {
                    m_SlotRegistry.RegisterSlot(slot.SlotId, slot);
                }
            }

            if (migratedFromLegacy) {
                await PersistRegistryAsync();
            }
        }

        public void RegisterMigrationStep(ISaveMigrationStep step) {
            m_MigrationPipeline.RegisterStep(step);
        }

        public SaveSlotInfo[] GetAvailableSlots() {
            return m_SlotRegistry.GetAllSlots();
        }

        public SaveSlotInfo GetSlot(string slotId) {
            return m_SlotRegistry.GetSlot(slotId);
        }

        public async Ticket<SaveResult> SaveAsync<T>(string slotId, T data) where T : class {
            if (m_SaveInProgress) {
                return SaveResult.Fail(SaveFailureReason.SaveAlreadyInProgress);
            }

            m_SaveInProgress = true;

            try {
                return await SaveInternalAsync(slotId, data);
            }
            finally {
                m_SaveInProgress = false;
            }
        }

        public async Ticket<SaveLoadResult<T>> LoadAsync<T>(string slotId) where T : class {
            var slotInfo = m_SlotRegistry.GetSlot(slotId);

            if (slotInfo == null) {
                return SaveLoadResult<T>.Fail(SaveLoadStatus.NoValidSave, $"No save slot '{slotId}' found");
            }

            var result = await TryLoadFileAsync<T>(slotInfo.CurrentFile, slotInfo);

            if (result.Success) {
                return result;
            }

            if (false == string.IsNullOrEmpty(slotInfo.BackupFile)) {
                m_Logger.Warn($"Primary save corrupt for '{slotId}', trying backup");
                result = await TryLoadFileAsync<T>(slotInfo.BackupFile, slotInfo);

                if (result.Success) {
                    return result.Status == SaveLoadStatus.SuccessMigrated
                        ? SaveLoadResult<T>.MigratedFromBackup(result.Data, slotInfo)
                        : SaveLoadResult<T>.FromBackup(result.Data, slotInfo);
                }
            }

            return SaveLoadResult<T>.Fail(SaveLoadStatus.NoValidSave,
                $"Both primary and backup saves for '{slotId}' are invalid");
        }

        public async Ticket<bool> DeleteSlotAsync(string slotId) {
            var slotInfo = m_SlotRegistry.GetSlot(slotId);

            if (slotInfo == null) {
                return false;
            }

            if (false == string.IsNullOrEmpty(slotInfo.CurrentFile)) {
                await m_Provider.DeleteAsync(slotInfo.CurrentFile);
            }

            if (false == string.IsNullOrEmpty(slotInfo.BackupFile)) {
                await m_Provider.DeleteAsync(slotInfo.BackupFile);
            }

            m_SlotRegistry.RemoveSlot(slotId);
            await PersistRegistryAsync();

            return true;
        }

        private async Ticket<SaveResult> SaveInternalAsync<T>(string slotId, T data) where T : class {
            byte[] serialized;

            try {
                serialized = m_Serializer.Serialize(data);
            }
            catch (Exception ex) {
                m_Logger.Error($"Serialization failed: {ex.Message}");
                return SaveResult.Fail(SaveFailureReason.SerializationFailed);
            }

            serialized = EmbedChecksum(serialized);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var tempFile = $"{slotId}_{timestamp}.sav";
            var attempt = 2;
            while (await m_Provider.ExistsAsync(tempFile)) {
                tempFile = $"{slotId}_{timestamp}_{attempt++}.sav";
            }

            var writeSuccess = await m_Provider.WriteAsync(tempFile, serialized);

            if (false == writeSuccess) {
                return SaveResult.Fail(SaveFailureReason.WriteFailed);
            }

            var readBack = await m_Provider.ReadAsync(tempFile);
            var integrity = m_Validator.Validate(readBack);

            if (false == integrity.IsValid) {
                await m_Provider.DeleteAsync(tempFile);
                m_Logger.Error($"Post-write validation failed: {integrity.Reason}");
                return SaveResult.Fail(SaveFailureReason.PostWriteValidationFailed);
            }

            var slotInfo = m_SlotRegistry.GetSlot(slotId) ?? new SaveSlotInfo { SlotId = slotId };

            if (false == string.IsNullOrEmpty(slotInfo.BackupFile)) {
                await m_Provider.DeleteAsync(slotInfo.BackupFile);
            }

            slotInfo.BackupFile = slotInfo.CurrentFile;
            slotInfo.CurrentFile = tempFile;
            slotInfo.LastSaveTime = DateTime.UtcNow;
            slotInfo.SchemaVersion = m_Serializer.CurrentSchemaVersion;

            m_SlotRegistry.RegisterSlot(slotId, slotInfo);
            await PersistRegistryAsync();

            return SaveResult.Ok(tempFile);
        }

        private async Ticket<SaveLoadResult<T>> TryLoadFileAsync<T>(string filePath, SaveSlotInfo slotInfo) where T : class {
            if (string.IsNullOrEmpty(filePath)) {
                return SaveLoadResult<T>.Fail(SaveLoadStatus.NoValidSave, "File path is empty");
            }

            byte[] rawData;

            try {
                rawData = await m_Provider.ReadAsync(filePath);
            }
            catch (Exception ex) {
                return SaveLoadResult<T>.Fail(SaveLoadStatus.ProviderError, ex.Message);
            }

            if (rawData == null) {
                return SaveLoadResult<T>.Fail(SaveLoadStatus.NoValidSave, $"File not found: {filePath}");
            }

            var integrity = m_Validator.Validate(rawData);

            if (false == integrity.IsValid) {
                return SaveLoadResult<T>.Fail(SaveLoadStatus.NoValidSave,
                    $"Integrity check failed: {integrity.Reason}");
            }

            var deserialized = m_Serializer.Deserialize<T>(rawData);

            if (false == deserialized.Success) {
                return await TryMigrateAndDeserializeAsync<T>(rawData, slotInfo);
            }

            if (deserialized.SchemaVersion < m_Serializer.CurrentSchemaVersion) {
                return await TryMigrateAndDeserializeAsync<T>(rawData, slotInfo);
            }

            return SaveLoadResult<T>.Ok(deserialized.Data, slotInfo);
        }

        private Ticket<SaveLoadResult<T>> TryMigrateAndDeserializeAsync<T>(byte[] rawData, SaveSlotInfo slotInfo) where T : class {
            try {
                var envelopeJson = Encoding.UTF8.GetString(rawData);
                var envelope = JsonConvert.DeserializeObject<SaveEnvelope>(envelopeJson);

                if (envelope == null) {
                    return Ticket.FromResult(
                        SaveLoadResult<T>.Fail(SaveLoadStatus.MigrationFailed, "Cannot read save envelope for migration"));
                }

                var migration = m_MigrationPipeline.Migrate(
                    envelope.DataJson,
                    envelope.SchemaVersion,
                    m_Serializer.CurrentSchemaVersion
                );

                if (false == migration.Success) {
                    return Ticket.FromResult(
                        SaveLoadResult<T>.Fail(SaveLoadStatus.MigrationFailed, migration.ErrorMessage));
                }

                var data = JsonConvert.DeserializeObject<T>(migration.MigratedJson);

                if (data == null) {
                    return Ticket.FromResult(
                        SaveLoadResult<T>.Fail(SaveLoadStatus.MigrationFailed, "Deserialization failed after migration"));
                }

                return Ticket.FromResult(SaveLoadResult<T>.Migrated(data, slotInfo));
            }
            catch (Exception ex) {
                return Ticket.FromResult(
                    SaveLoadResult<T>.Fail(SaveLoadStatus.MigrationFailed, $"Migration error: {ex.Message}"));
            }
        }

        private byte[] EmbedChecksum(byte[] serializedData) {
            var json = Encoding.UTF8.GetString(serializedData);
            var envelope = JsonConvert.DeserializeObject<SaveEnvelope>(json);

            var dataBytes = Encoding.UTF8.GetBytes(envelope.DataJson);
            envelope.Checksum = m_Validator.GenerateChecksum(dataBytes);

            var updatedJson = JsonConvert.SerializeObject(envelope);
            return Encoding.UTF8.GetBytes(updatedJson);
        }

        private async Ticket PersistRegistryAsync() {
            var registryData = m_SlotRegistry.ToBytes();
            await m_Provider.WriteAsync(RegistryFile, registryData);
        }
    }
}
