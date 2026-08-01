using System;
using System.Collections.Generic;
using Buttr.Core;
using CLabs.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Crumb.Unity.Editor {
    /// <summary>
    /// A live console for Crumb logs. In Play mode it streams the recent entries from the running app's
    /// <see cref="BufferedCrumbSink"/> (wired by the <c>CrumbApplicationLoader</c>), with colour-coded levels,
    /// per-level + text filters, clear, and auto-scroll. Companion to the Crumb Manager, which toggles loggers.
    /// </summary>
    public sealed class CrumbConsoleWindow : CLabsEditorWindow {
        protected override string WindowTitle => "Crumb Console";

        private static readonly CrumbFilters[] s_Levels =
            { CrumbFilters.Verbose, CrumbFilters.Info, CrumbFilters.Warning, CrumbFilters.Error, CrumbFilters.Fatal };

        private ListView m_ListView;
        private Label m_EmptyState;
        private Label m_StatusLabel;

        private readonly List<CrumbEntry> m_Visible = new();
        private readonly Dictionary<CrumbFilters, Button> m_LevelButtons = new();

        private CrumbFilters m_LevelFilter = CrumbFilters.All;
        private string m_TextFilter = string.Empty;
        private bool m_AutoScroll = true;
        private int m_LastVersion = -1;
        private bool m_WasPlaying;

        [MenuItem("Window/Crumpet Labs/Crumb Console")]
        public static void ShowWindow() {
            var window = GetWindow<CrumbConsoleWindow>();
            window.titleContent = new GUIContent("Crumb Console");
            window.minSize = new Vector2(480, 300);
        }

        protected override void OnCreateContent(VisualElement root) {
            root.Add(BuildToolbar());
            root.Add(BuildLogView());
            root.Add(BuildStatusBar());
            Refresh();
        }

        private VisualElement BuildToolbar() {
            var bar = CLabsToolbar.Create(text => {
                m_TextFilter = text ?? string.Empty;
                Refresh();
            });

            foreach (var level in s_Levels) {
                var levelValue = level;
                var button = new Button { text = ShortTag(level) };
                button.AddToClassList("clabs-badge");
                button.style.color = LevelColor(level);
                button.clicked += () => {
                    m_LevelFilter ^= levelValue;
                    UpdateLevelButton(levelValue);
                    Refresh();
                };
                m_LevelButtons[level] = button;
                UpdateLevelButton(level);
                bar.Add(button);
            }

            bar.AddButton("Clear", () => {
                GetBuffer()?.Clear();
                Refresh();
            });
            return bar;
        }

        private VisualElement BuildLogView() {
            var container = new VisualElement { style = { flexGrow = 1 } };

            m_ListView = new ListView {
                fixedItemHeight = 20,
                selectionType = SelectionType.None,
                makeItem = MakeRow,
                bindItem = BindRow,
                itemsSource = m_Visible,
                style = { flexGrow = 1 }
            };

            m_EmptyState = new Label("No log entries — enter Play mode with a CrumbApplicationLoader active to stream logs here.");
            m_EmptyState.AddToClassList("clabs-empty");

            container.Add(m_ListView);
            container.Add(m_EmptyState);
            return container;
        }

        private VisualElement BuildStatusBar() {
            var bar = new VisualElement();
            bar.AddToClassList("clabs-status");

            m_StatusLabel = new Label();
            m_StatusLabel.AddToClassList("clabs-status__label");

            var autoScroll = new Toggle("Auto-scroll") { value = m_AutoScroll };
            autoScroll.RegisterValueChangedCallback(evt => m_AutoScroll = evt.newValue);

            bar.Add(m_StatusLabel);
            bar.Add(autoScroll);
            return bar;
        }

        private static VisualElement MakeRow() {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, paddingLeft = 4, paddingRight = 4 } };
            row.Add(new Label { name = "time", style = { width = 84, opacity = 0.6f, unityTextAlign = TextAnchor.MiddleLeft } });
            row.Add(new Label { name = "level", style = { width = 34, unityFontStyleAndWeight = FontStyle.Bold, unityTextAlign = TextAnchor.MiddleLeft } });
            row.Add(new Label { name = "type", style = { width = 130, opacity = 0.8f, unityTextAlign = TextAnchor.MiddleLeft } });
            row.Add(new Label { name = "msg", style = { flexGrow = 1, whiteSpace = WhiteSpace.NoWrap, overflow = Overflow.Hidden, unityTextAlign = TextAnchor.MiddleLeft } });
            return row;
        }

        private void BindRow(VisualElement element, int index) {
            if (index < 0 || index >= m_Visible.Count) return;

            var entry = m_Visible[index];
            element.Q<Label>("time").text = entry.Timestamp.ToString("HH:mm:ss.fff");

            var levelLabel = element.Q<Label>("level");
            levelLabel.text = entry.Level;
            levelLabel.style.color = LevelColor(LevelFromTag(entry.Level));

            element.Q<Label>("type").text = entry.TypeName;

            var message = element.Q<Label>("msg");
            message.text = entry.Message;
            message.tooltip = entry.Message;
        }

        protected override void OnEditorUpdate() {
            var playing = EditorApplication.isPlaying;
            if (playing != m_WasPlaying) {
                m_WasPlaying = playing;
                m_LastVersion = -1;
                Refresh();
                return;
            }

            var buffer = GetBuffer();
            if (buffer == null) {
                if (m_LastVersion != -1) {
                    m_LastVersion = -1;
                    Refresh();
                }
                return;
            }

            if (buffer.Version != m_LastVersion) {
                m_LastVersion = buffer.Version;
                Refresh();
            }
        }

        private void Refresh() {
            if (m_ListView == null) return;

            var buffer = GetBuffer();
            m_Visible.Clear();

            if (buffer != null) {
                foreach (var entry in buffer.Snapshot()) {
                    if (PassesFilter(entry)) {
                        m_Visible.Add(entry);
                    }
                }
            }

            var hasEntries = m_Visible.Count > 0;
            m_ListView.style.display = hasEntries ? DisplayStyle.Flex : DisplayStyle.None;
            m_EmptyState.style.display = hasEntries ? DisplayStyle.None : DisplayStyle.Flex;

            m_ListView.RefreshItems();

            if (m_AutoScroll && hasEntries) {
                m_ListView.ScrollToItem(m_Visible.Count - 1);
            }

            UpdateStatusBar(buffer);
        }

        private bool PassesFilter(CrumbEntry entry) {
            if (false == m_LevelFilter.HasFlag(LevelFromTag(entry.Level))) {
                return false;
            }

            if (false == string.IsNullOrEmpty(m_TextFilter)) {
                var inType = entry.TypeName != null && entry.TypeName.Contains(m_TextFilter, StringComparison.OrdinalIgnoreCase);
                var inMessage = entry.Message != null && entry.Message.Contains(m_TextFilter, StringComparison.OrdinalIgnoreCase);
                if (false == inType && false == inMessage) {
                    return false;
                }
            }

            return true;
        }

        private void UpdateLevelButton(CrumbFilters level) {
            if (false == m_LevelButtons.TryGetValue(level, out var button)) return;
            button.style.opacity = m_LevelFilter.HasFlag(level) ? 1f : 0.35f;
        }

        private void UpdateStatusBar(BufferedCrumbSink buffer) {
            if (m_StatusLabel == null) return;

            if (buffer == null) {
                m_StatusLabel.text = EditorApplication.isPlaying
                    ? "No BufferedCrumbSink resolved — is a CrumbApplicationLoader active?"
                    : "Enter Play mode to stream logs";
                return;
            }

            m_StatusLabel.text = $"{m_Visible.Count} shown";
        }

        private static BufferedCrumbSink GetBuffer() {
            if (false == EditorApplication.isPlaying) {
                return null;
            }

            try {
                return Application<BufferedCrumbSink>.Get();
            }
            catch {
                return null;
            }
        }

        private static string ShortTag(CrumbFilters level) {
            return level switch {
                CrumbFilters.Verbose => "VRB",
                CrumbFilters.Info => "INF",
                CrumbFilters.Warning => "WRN",
                CrumbFilters.Error => "ERR",
                CrumbFilters.Fatal => "FTL",
                _ => "?"
            };
        }

        private static CrumbFilters LevelFromTag(string tag) {
            return tag switch {
                "VRB" => CrumbFilters.Verbose,
                "INF" => CrumbFilters.Info,
                "WRN" => CrumbFilters.Warning,
                "ERR" => CrumbFilters.Error,
                "FTL" => CrumbFilters.Fatal,
                _ => CrumbFilters.None
            };
        }

        private static Color LevelColor(CrumbFilters level) {
            return level switch {
                CrumbFilters.Verbose => new Color(0.60f, 0.60f, 0.60f),
                CrumbFilters.Warning => new Color(1.00f, 0.80f, 0.20f),
                CrumbFilters.Error => new Color(1.00f, 0.40f, 0.40f),
                CrumbFilters.Fatal => new Color(1.00f, 0.30f, 0.70f),
                _ => new Color(0.85f, 0.85f, 0.85f)
            };
        }
    }
}
