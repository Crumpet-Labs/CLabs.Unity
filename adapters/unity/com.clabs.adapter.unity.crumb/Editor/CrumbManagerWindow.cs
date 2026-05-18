using System;
using System.Collections.Generic;
using System.Linq;
using Buttr.Core;
using CLabs.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Crumb.Unity.Editor {
    public sealed class CrumbManagerWindow : CLabsEditorWindow {
        protected override string WindowTitle => "Crumb Manager";
        protected override string PackageStyleSheetPath => "Assets/_Project/Core/Crumb/Editor/CrumbManagerWindow.uss";

        private ScrollView m_ListView;
        private Label m_EmptyState;
        private Label m_StatusLabel;
        private VisualElement m_StatusDot;

        private int m_LastLoggerCount;
        private string m_FilterText = string.Empty;
        private readonly Dictionary<Type, VisualElement> m_Rows = new();

        [MenuItem("Window/Crumpet Labs/Crumb Manager")]
        public static void ShowWindow() {
            var window = GetWindow<CrumbManagerWindow>();
            window.titleContent = new GUIContent("Crumb Manager");
            window.minSize = new Vector2(400, 250);
        }

        protected override void OnCreateContent(VisualElement root) {
            root.Add(BuildToolbar());
            root.Add(BuildLoggerList());
            root.Add(BuildStatusBar());
        }

        // ── Toolbar ──

        private VisualElement BuildToolbar() {
            return CLabsToolbar.Create(text => {
                m_FilterText = text ?? string.Empty;
                ApplyFilter();
            })
            .AddButton("All On", () => SetAllEnabled(true))
            .AddButton("All Off", () => SetAllEnabled(false));
        }

        // ── Logger List ──

        private VisualElement BuildLoggerList() {
            var container = new VisualElement { style = { flexGrow = 1 } };

            m_ListView = new ScrollView(ScrollViewMode.Vertical);
            m_ListView.AddToClassList("logger-list");

            m_EmptyState = new Label("No loggers active — enter Play mode to see registered loggers");
            m_EmptyState.AddToClassList("clabs-empty");

            container.Add(m_ListView);
            container.Add(m_EmptyState);

            return container;
        }

        // ── Status Bar ──

        private VisualElement BuildStatusBar() {
            var bar = new VisualElement();
            bar.AddToClassList("clabs-status");

            m_StatusDot = new VisualElement();
            m_StatusDot.AddToClassList("clabs-status__dot");

            m_StatusLabel = new Label();
            m_StatusLabel.AddToClassList("clabs-status__label");

            bar.Add(m_StatusDot);
            bar.Add(m_StatusLabel);

            UpdateStatusBar();
            return bar;
        }

        // ── Update Loop ──

        protected override void OnEditorUpdate() {
            if (!EditorApplication.isPlaying) {
                if (m_LastLoggerCount != 0)
                    ClearList();
                return;
            }

            var registry = GetRegistry();
            if (registry == null) return;

            var count = registry.Values.Count();
            if (count != m_LastLoggerCount) {
                RebuildList(registry);
                m_LastLoggerCount = count;
            }

            UpdateStatusBar();
        }

        // ── List Management ──

        private void RebuildList(CrumbRegistry registry) {
            m_ListView.Clear();
            m_Rows.Clear();

            foreach (var logger in registry.Values) {
                if (logger.Type == null) continue;

                var row = BuildLoggerRow(logger);
                m_Rows[logger.Type] = row;
                m_ListView.Add(row);
            }

            var hasLoggers = m_Rows.Count > 0;
            m_ListView.style.display = hasLoggers ? DisplayStyle.Flex : DisplayStyle.None;
            m_EmptyState.style.display = hasLoggers ? DisplayStyle.None : DisplayStyle.Flex;

            ApplyFilter();
        }

        private void ClearList() {
            m_ListView.Clear();
            m_Rows.Clear();
            m_LastLoggerCount = 0;
            m_ListView.style.display = DisplayStyle.None;
            m_EmptyState.style.display = DisplayStyle.Flex;
        }

        private VisualElement BuildLoggerRow(CrumbLogger logger) {
            var row = new VisualElement();
            row.AddToClassList("logger-row");
            row.userData = logger;

            var toggle = new Toggle();
            toggle.AddToClassList("logger-row__toggle");
            toggle.value = logger.Enabled;
            toggle.RegisterValueChangedCallback(evt => {
                logger.Enabled = evt.newValue;
                UpdateRowAppearance(row, logger);
            });

            var name = new Label(logger.Type.Name);
            name.AddToClassList("logger-row__name");
            name.tooltip = logger.Type.FullName;

            var filters = new VisualElement();
            filters.AddToClassList("logger-row__filters");

            filters.Add(BuildFilterChip("VRB", CrumbFilters.Verbose, "filter-chip--vrb", logger));
            filters.Add(BuildFilterChip("INF", CrumbFilters.Info, "filter-chip--inf", logger));
            filters.Add(BuildFilterChip("WRN", CrumbFilters.Warning, "filter-chip--wrn", logger));
            filters.Add(BuildFilterChip("ERR", CrumbFilters.Error, "filter-chip--err", logger));
            filters.Add(BuildFilterChip("FTL", CrumbFilters.Fatal, "filter-chip--ftl", logger));

            row.Add(toggle);
            row.Add(name);
            row.Add(filters);

            UpdateRowAppearance(row, logger);
            return row;
        }

        private Button BuildFilterChip(string label, CrumbFilters flag, string levelClass, CrumbLogger logger) {
            var chip = new Button();
            chip.text = label;
            chip.AddToClassList("filter-chip");
            chip.AddToClassList(levelClass);

            if (logger.Filters.HasFlag(flag))
                chip.AddToClassList("filter-chip--active");

            chip.clicked += () => {
                logger.Filters ^= flag;

                if (logger.Filters.HasFlag(flag))
                    chip.AddToClassList("filter-chip--active");
                else
                    chip.RemoveFromClassList("filter-chip--active");
            };

            return chip;
        }

        private void UpdateRowAppearance(VisualElement row, CrumbLogger logger) {
            var nameLabel = row.Q<Label>(className: "logger-row__name");
            if (nameLabel == null) return;

            if (logger.Enabled)
                nameLabel.RemoveFromClassList("logger-row__name--disabled");
            else
                nameLabel.AddToClassList("logger-row__name--disabled");
        }

        // ── Filtering ──

        private void ApplyFilter() {
            foreach (var kvp in m_Rows) {
                var visible = string.IsNullOrEmpty(m_FilterText) ||
                              kvp.Key.Name.Contains(m_FilterText, StringComparison.OrdinalIgnoreCase);
                kvp.Value.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void SetAllEnabled(bool enabled) {
            foreach (var row in m_Rows.Values) {
                if (row.userData is CrumbLogger logger) {
                    logger.Enabled = enabled;
                    var toggle = row.Q<Toggle>();
                    if (toggle != null) toggle.value = enabled;
                    UpdateRowAppearance(row, logger);
                }
            }
        }

        // ── Status Bar ──

        private void UpdateStatusBar() {
            var config = GetConfiguration();
            var fileEnabled = config != null && config.FileLoggingEnabled;
            var path = config != null ? config.LogDirectory : "N/A";

            m_StatusDot.EnableInClassList("clabs-status__dot--enabled", fileEnabled);
            m_StatusDot.EnableInClassList("clabs-status__dot--disabled", !fileEnabled);
            m_StatusLabel.text = $"File logging: {(fileEnabled ? "Enabled" : "Disabled")}  |  {path}";
        }

        // ── Helpers ──

        private static CrumbRegistry GetRegistry() {
            try { return Application<CrumbRegistry>.Get(); }
            catch { return null; }
        }

        private static ICrumbConfiguration GetConfiguration() {
            try { return Application<ICrumbConfiguration>.Get(); }
            catch { return null; }
        }
    }
}
