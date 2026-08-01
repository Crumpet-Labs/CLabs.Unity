using Buttr.Injection;
using CLabs.Saves;
using CLabs.Tickets;
using UnityEngine;

namespace CLabs.Saves.Samples {
    /// <summary>
    /// Drives three save slots ("forks"). Wire the shipped CLabs/Saves/Application Loader into your
    /// app so ISavesService is injected here, then drive from the inspector context menu.
    /// </summary>
    public sealed partial class SaveBench : MonoBehaviour {
        [Inject] private ISavesService i_Saves;

        [SerializeField] private string m_SlotId = "fork-1";
        [SerializeField] private string m_ChefName = "Sourdough Sam";

        [ContextMenu("Save")]
        public async void Save() {
            var save = new ChefSave { ChefName = m_ChefName, Day = 3, Coins = 120 };
            var result = await i_Saves.SaveAsync(m_SlotId, save);
            Debug.Log(result.Success
                ? $"[Three Forks] Saved {m_ChefName} → {m_SlotId} ({result.FilePath})."
                : $"[Three Forks] Save failed: {result.Reason}.");
        }

        [ContextMenu("Load")]
        public async void Load() {
            var result = await i_Saves.LoadAsync<ChefSave>(m_SlotId);
            if (result.Success)
                Debug.Log($"[Three Forks] Loaded {result.Data.ChefName} · day {result.Data.Day} · {result.Data.Coins} coin (status: {result.Status}).");
            else
                Debug.Log($"[Three Forks] Load failed: {result.Status} — {result.Message}.");
        }

        [ContextMenu("List slots")]
        public void ListSlots() {
            foreach (var slot in i_Saves.GetAvailableSlots())
                Debug.Log($"[Three Forks] Slot {slot.SlotId} · last saved {slot.LastSaveTime:g} · schema v{slot.SchemaVersion}.");
        }

        [ContextMenu("Delete slot")]
        public async void DeleteSlot() {
            var deleted = await i_Saves.DeleteSlotAsync(m_SlotId);
            Debug.Log($"[Three Forks] Delete {m_SlotId}: {deleted}.");
        }
    }
}
