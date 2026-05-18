using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CLabs.Fork {
    public sealed class SaveSlotRegistry {
        private readonly Dictionary<string, SaveSlotInfo> m_Slots = new();

        public SaveSlotInfo GetSlot(string slotId) {
            return m_Slots.TryGetValue(slotId, out var info) ? info : null;
        }

        public SaveSlotInfo[] GetAllSlots() {
            return m_Slots.Values.ToArray();
        }

        public bool HasSlot(string slotId) {
            return m_Slots.ContainsKey(slotId);
        }

        public void RegisterSlot(string slotId, SaveSlotInfo info) {
            m_Slots[slotId] = info;
        }

        public void UpdateSlot(string slotId, SaveSlotInfo info) {
            m_Slots[slotId] = info;
        }

        public void RemoveSlot(string slotId) {
            m_Slots.Remove(slotId);
        }

        public byte[] ToBytes() {
            var json = JsonConvert.SerializeObject(m_Slots);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public static SaveSlotRegistry FromBytes(byte[] data) {
            var registry = new SaveSlotRegistry();

            if (data == null || data.Length == 0) return registry;

            var json = System.Text.Encoding.UTF8.GetString(data);
            var slots = JsonConvert.DeserializeObject<Dictionary<string, SaveSlotInfo>>(json);

            if (slots != null) {
                foreach (var kvp in slots) {
                    registry.m_Slots[kvp.Key] = kvp.Value;
                }
            }

            return registry;
        }
    }
}
