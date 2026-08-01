using Buttr.Core;
using CLabs.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Saves.Unity.Editor {
    public sealed class SavesViewerWindow : CLabsEditorWindow {
        protected override string WindowTitle => "Fork Viewer";

        private CLabsPanel m_SlotsPanel;
        private CLabsPanel m_DetailPanel;
        private Label m_EmptyState;
        private CLabsStatusBar m_StatusBar;

        private SaveSlotInfo m_Selected;

        [MenuItem("Window/Crumpet Labs/Fork Viewer")]
        public static void ShowWindow() {
            var window = GetWindow<SavesViewerWindow>();
            window.titleContent = new GUIContent("Fork Viewer");
            window.minSize = new Vector2(550, 300);
        }

        protected override void OnCreateContent(VisualElement root) {
            var toolbar = new VisualElement();
            toolbar.AddToClassList("clabs-toolbar");

            var refreshButton = new Button(Refresh) { text = "Refresh" };
            refreshButton.AddToClassList("clabs-toolbar__button");

            var openFolderButton = new Button(OpenSaveFolder) { text = "Open Folder" };
            openFolderButton.AddToClassList("clabs-toolbar__button");

            toolbar.Add(refreshButton);
            toolbar.Add(openFolderButton);
            root.Add(toolbar);

            var panels = new VisualElement();
            panels.AddToClassList("clabs-panels");

            m_SlotsPanel = CLabsPanel.Create("Save Slots");
            m_DetailPanel = CLabsPanel.Create("Details");

            panels.Add(m_SlotsPanel);
            panels.Add(m_DetailPanel);

            m_EmptyState = new Label("No save slots found — enter Play Mode");
            m_EmptyState.AddToClassList("clabs-empty");

            m_StatusBar = CLabsStatusBar.Create();

            root.Add(panels);
            root.Add(m_EmptyState);
            root.Add(m_StatusBar);
        }

        protected override void OnEditorUpdate() {
            if (!EditorApplication.isPlaying) {
                m_EmptyState.style.display = DisplayStyle.Flex;
                m_SlotsPanel.style.display = DisplayStyle.None;
                m_DetailPanel.style.display = DisplayStyle.None;
                m_StatusBar.Text.text = "Enter Play Mode";
                return;
            }

            Refresh();
        }

        private void Refresh() {
            var service = Application<ISavesService>.Get();
            var slots = service.GetAvailableSlots();
            var hasSlots = slots.Length > 0;

            m_EmptyState.style.display = hasSlots ? DisplayStyle.None : DisplayStyle.Flex;
            m_SlotsPanel.style.display = hasSlots ? DisplayStyle.Flex : DisplayStyle.None;
            m_DetailPanel.style.display = hasSlots ? DisplayStyle.Flex : DisplayStyle.None;

            m_SlotsPanel.Content.Clear();

            foreach (var slot in slots) {
                var row = new VisualElement();
                row.AddToClassList("clabs-row");
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;

                var name = new Label(slot.SlotId);
                name.AddToClassList("clabs-row__name");
                row.Add(name);

                var badge = new Label(slot.IsAutoSave ? "Auto" : "Manual");
                badge.AddToClassList("clabs-badge");
                badge.AddToClassList(slot.IsAutoSave ? "clabs-badge--accent" : "clabs-badge--neutral");
                row.Add(badge);

                var captured = slot;
                row.RegisterCallback<ClickEvent>(_ => SelectSlot(captured));

                if (m_Selected != null && m_Selected.SlotId == slot.SlotId) {
                    row.AddToClassList("clabs-row--selected");
                }

                m_SlotsPanel.Content.Add(row);
            }

            if (m_Selected != null) RebuildDetails();

            m_StatusBar.Text.text = $"{slots.Length} save slots";
        }

        private void SelectSlot(SaveSlotInfo slot) {
            m_Selected = slot;
            Refresh();
        }

        private void RebuildDetails() {
            m_DetailPanel.Content.Clear();

            if (m_Selected == null) return;

            AddDetail("Slot ID", m_Selected.SlotId);
            AddDetail("Current File", m_Selected.CurrentFile ?? "(none)");
            AddDetail("Backup File", m_Selected.BackupFile ?? "(none)");
            AddDetail("Last Save", m_Selected.LastSaveTime.ToString("yyyy-MM-dd HH:mm:ss"));
            AddDetail("Schema Version", $"v{m_Selected.SchemaVersion}");
            AddDetail("Auto Save", m_Selected.IsAutoSave ? "Yes" : "No");

            var deleteButton = new Button(() => DeleteSelectedSlot()) { text = "Delete Slot" };
            deleteButton.AddToClassList("clabs-toolbar__button");
            deleteButton.style.marginTop = 8;
            m_DetailPanel.Content.Add(deleteButton);
        }

        private void AddDetail(string label, string value) {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingLeft = 4;
            row.style.paddingRight = 4;
            row.style.paddingTop = 2;
            row.style.paddingBottom = 2;

            var lbl = new Label(label + ":");
            lbl.style.width = 120;
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;

            var val = new Label(value);

            row.Add(lbl);
            row.Add(val);
            m_DetailPanel.Content.Add(row);
        }

        private async void DeleteSelectedSlot() {
            if (m_Selected == null) return;

            var slotId = m_Selected.SlotId;

            if (!EditorUtility.DisplayDialog("Delete Save Slot",
                    $"Delete save slot '{slotId}'? This cannot be undone.", "Delete", "Cancel")) {
                return;
            }

            var service = Application<ISavesService>.Get();
            await service.DeleteSlotAsync(slotId);
            m_Selected = null;
            Refresh();
        }

        private static void OpenSaveFolder() {
            string path;
            try { path = Application<ISavesConfiguration>.Get().RootPath; }
            catch { path = UnityEngine.Application.persistentDataPath; }
            EditorUtility.RevealInFinder(path);
        }
    }
}
