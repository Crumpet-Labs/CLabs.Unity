using CLabs.Tickets;

namespace CLabs.Fork {
    public interface IForkService {
        Ticket<SaveResult> SaveAsync<T>(string slotId, T data) where T : class;
        Ticket<SaveLoadResult<T>> LoadAsync<T>(string slotId) where T : class;
        SaveSlotInfo[] GetAvailableSlots();
        SaveSlotInfo GetSlot(string slotId);
        Ticket<bool> DeleteSlotAsync(string slotId);
        void RegisterMigrationStep(ISaveMigrationStep step);
    }
}
