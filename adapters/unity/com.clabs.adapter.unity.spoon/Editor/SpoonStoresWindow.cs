using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Buttr.Core;
using CLabs.Editor;
using CLabs.Spoon;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Spoon.Unity.Editor
{
    /// <summary>
    /// Editor window that lists every <c>IStore&lt;TState&gt;</c> registered in the active
    /// Buttr container and renders the selected store's current state field-by-field.
    /// Live during Play mode only.
    /// </summary>
    public sealed class SpoonStoresWindow : CLabsEditorWindow
    {
        protected override string WindowTitle => "Spoon Stores";

        private CLabsPanel m_StoresPanel;
        private CLabsPanel m_StatePanel;
        private Label m_EmptyState;
        private CLabsStatusBar m_StatusBar;

        // Discovered stores: state type -> (store instance, cached State PropertyInfo).
        // Built once per Play session via Application<object>.All() in Buttr.
        private Dictionary<Type, StoreEntry> m_Stores;
        private Type m_Selected;
        private int m_LastStoreCount;
        private readonly Dictionary<Type, VisualElement> m_StoreRows = new();

        [MenuItem("Window/Crumpet Labs/Spoon Stores")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpoonStoresWindow>();
            window.titleContent = new GUIContent("Spoon Stores");
            window.minSize = new Vector2(550, 350);
        }

        protected override void OnCreateContent(VisualElement root)
        {
            var panels = new VisualElement();
            panels.AddToClassList("clabs-panels");

            m_StoresPanel = CLabsPanel.Create("Stores");
            m_StatePanel = CLabsPanel.Create("State");

            panels.Add(m_StoresPanel);
            panels.Add(m_StatePanel);

            m_EmptyState = new Label("No stores registered — enter Play mode with a Buttr application that registered Spoon stores.");
            m_EmptyState.AddToClassList("clabs-empty");

            m_StatusBar = CLabsStatusBar.Create();

            root.Add(panels);
            root.Add(m_EmptyState);
            root.Add(m_StatusBar);
        }

        protected override void OnEditorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                if (m_LastStoreCount != 0 || m_Stores != null) ClearAll();
                return;
            }

            DiscoverStoresIfNeeded();

            var currentCount = m_Stores?.Count ?? 0;
            if (currentCount != m_LastStoreCount)
            {
                RebuildStoresList();
                m_LastStoreCount = currentCount;
            }

            if (m_Selected != null) RebuildState();
        }

        // ── Discovery ──

        /// <summary>
        /// Walks the Buttr application's registration list once per Play session via
        /// <see cref="Application{T}.All"/> with <c>T = object</c>, which yields every
        /// non-hidden registration's resolved instance. Any instance whose concrete type
        /// implements <see cref="IStore{TState}"/> for some <c>TState</c> is recorded
        /// keyed by that state type.
        /// </summary>
        private void DiscoverStoresIfNeeded()
        {
            if (m_Stores != null) return;

            var found = new Dictionary<Type, StoreEntry>();

            try
            {
                foreach (var instance in Application<object>.All())
                {
                    if (instance == null) continue;

                    var concrete = instance.GetType();
                    foreach (var iface in concrete.GetInterfaces())
                    {
                        if (!iface.IsGenericType) continue;
                        if (iface.GetGenericTypeDefinition() != typeof(IStore<>)) continue;

                        var stateType = iface.GetGenericArguments()[0];
                        var stateProp = iface.GetProperty("State");
                        if (stateProp == null) continue;

                        // Multiple registrations of the same TState would last-write-wins,
                        // matching Buttr's behaviour for duplicate registrations.
                        found[stateType] = new StoreEntry(instance, stateProp);
                    }
                }
            }
            catch
            {
                // Registry walk failed (likely container mid-build or torn down).
                // Leave m_Stores null so we retry next tick.
                return;
            }

            m_Stores = found;
        }

        // ── Stores list ──

        private void RebuildStoresList()
        {
            m_StoresPanel.Content.Clear();
            m_StoreRows.Clear();

            var hasAny = m_Stores != null && m_Stores.Count > 0;
            m_StoresPanel.style.display = hasAny ? DisplayStyle.Flex : DisplayStyle.None;
            m_StatePanel.style.display = hasAny ? DisplayStyle.Flex : DisplayStyle.None;
            m_EmptyState.style.display = hasAny ? DisplayStyle.None : DisplayStyle.Flex;

            if (!hasAny) return;

            foreach (var stateType in m_Stores.Keys.OrderBy(t => t.Name))
            {
                var row = new VisualElement();
                row.AddToClassList("clabs-row");

                var label = new Label(FormatTypeName(stateType));
                label.AddToClassList("clabs-row__name");
                row.Add(label);

                var captured = stateType;
                row.RegisterCallback<ClickEvent>(_ => SelectState(captured));

                m_StoreRows[stateType] = row;
                m_StoresPanel.Content.Add(row);
            }

            if (m_Selected == null || !m_StoreRows.ContainsKey(m_Selected))
            {
                SelectState(m_StoreRows.Keys.FirstOrDefault());
            }
        }

        private void SelectState(Type stateType)
        {
            foreach (var row in m_StoreRows.Values)
                row.RemoveFromClassList("clabs-row--selected");

            if (stateType != null && m_StoreRows.TryGetValue(stateType, out var selected))
                selected.AddToClassList("clabs-row--selected");

            m_Selected = stateType;
            RebuildState();
        }

        // ── State panel ──

        private void RebuildState()
        {
            m_StatePanel.Content.Clear();
            if (m_Selected == null) return;

            if (m_Stores == null || !m_Stores.TryGetValue(m_Selected, out var entry))
            {
                m_StatePanel.Content.Add(new Label("Store no longer in the registry."));
                return;
            }

            object state;
            try { state = entry.StateProperty.GetValue(entry.Instance); }
            catch { state = null; }

            if (state == null)
            {
                m_StatePanel.Content.Add(new Label("State is null."));
                return;
            }

            var stateType = state.GetType();

            foreach (var field in stateType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object value;
                try { value = field.GetValue(state); }
                catch { value = "<field threw>"; }
                AddRow(field.Name, FormatValue(value));
            }

            foreach (var prop in stateType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;
                object value;
                try { value = prop.GetValue(state); }
                catch { value = "<getter threw>"; }
                AddRow(prop.Name, FormatValue(value));
            }

            m_StatusBar.Text.text = $"{m_LastStoreCount} store(s) · selected: {FormatTypeName(m_Selected)}";
        }

        private void AddRow(string label, string value)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.paddingTop = 2;
            row.style.paddingBottom = 2;

            var name = new Label(label);
            name.style.unityFontStyleAndWeight = FontStyle.Bold;
            row.Add(name);

            var val = new Label(value);
            val.style.unityTextAlign = TextAnchor.MiddleRight;
            row.Add(val);

            m_StatePanel.Content.Add(row);
        }

        // ── Helpers ──

        private static string FormatTypeName(Type t)
        {
            if (!t.IsGenericType) return t.Name;
            var args = string.Join(", ", t.GetGenericArguments().Select(FormatTypeName));
            var raw = t.Name;
            var tickIndex = raw.IndexOf('`');
            var name = tickIndex < 0 ? raw : raw.Substring(0, tickIndex);
            return $"{name}<{args}>";
        }

        private static string FormatValue(object value)
        {
            if (value == null) return "null";
            return value.ToString();
        }

        private void ClearAll()
        {
            m_Stores = null;
            m_Selected = null;
            m_LastStoreCount = 0;
            m_StoresPanel.Content.Clear();
            m_StatePanel.Content.Clear();
            m_StoreRows.Clear();
            m_StatusBar.Text.text = string.Empty;
            m_EmptyState.style.display = DisplayStyle.Flex;
        }

        private readonly struct StoreEntry
        {
            public readonly object Instance;
            public readonly PropertyInfo StateProperty;

            public StoreEntry(object instance, PropertyInfo stateProperty)
            {
                Instance = instance;
                StateProperty = stateProperty;
            }
        }
    }
}
