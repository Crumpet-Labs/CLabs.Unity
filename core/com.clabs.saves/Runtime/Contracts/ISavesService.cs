using CLabs.Tickets;

namespace CLabs.Saves {
    /// <summary>
    /// Save-slot persistence: write-then-swap saves with integrity validation + automatic backup fallback, and a
    /// schema-migration pipeline that upgrades older saves on load. Each slot keeps a current + a rolled-over
    /// backup file. Register migration steps before loading so out-of-date saves migrate transparently.
    /// </summary>
    public interface ISavesService {
        /// <summary>Serialize, checksum, and atomically write <paramref name="data"/> to the slot, rolling the previous save to backup.</summary>
        Ticket<SaveResult> SaveAsync<T>(string slotId, T data) where T : class;

        /// <summary>Load the slot, validating integrity and falling back to its backup (and migrating older schemas) as needed.</summary>
        Ticket<SaveLoadResult<T>> LoadAsync<T>(string slotId) where T : class;

        /// <summary>All known save slots from the registry.</summary>
        SaveSlotInfo[] GetAvailableSlots();

        /// <summary>The registry record for a slot, or null if it has no save.</summary>
        SaveSlotInfo GetSlot(string slotId);

        /// <summary>Delete the slot's current + backup files and drop it from the registry.</summary>
        Ticket<bool> DeleteSlotAsync(string slotId);

        /// <summary>Register a schema-migration step applied to older saves on load.</summary>
        void RegisterMigrationStep(ISaveMigrationStep step);
    }
}
