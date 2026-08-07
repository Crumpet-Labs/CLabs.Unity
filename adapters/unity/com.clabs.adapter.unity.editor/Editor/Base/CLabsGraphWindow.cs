using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public abstract class CLabsGraphWindow : EditorWindow {
        private const string ThemePath = "Packages/com.clabs.adapter.unity.editor/Editor/Styles/CrumpetLabs.uss";

        protected abstract string WindowTitle { get; }
        protected abstract GraphView OnCreateGraphView();

        protected virtual void OnCreateToolbar(VisualElement toolbar) { }
        protected virtual void OnGraphDestroy() { }

        protected GraphView Graph { get; private set; }
        protected CLabsStatusBar StatusBar { get; private set; }
        protected CLabsPanel PropertyPanel { get; private set; }

        public void CreateGUI() {
            var root = rootVisualElement;

            LoadStyleSheets(root);
            root.Add(CLabsHeader.Create(WindowTitle));

            var toolbar = new VisualElement();
            toolbar.AddToClassList("clabs-graph-toolbar");
            OnCreateToolbar(toolbar);
            root.Add(toolbar);

            var splitContainer = new VisualElement();
            splitContainer.AddToClassList("clabs-graph-split");

            Graph = OnCreateGraphView();
            Graph.AddToClassList("clabs-graph-view");

            PropertyPanel = CLabsPanel.Create("Properties");
            PropertyPanel.AddToClassList("clabs-graph-properties");

            var emptyLabel = new Label("Select a node or edge");
            emptyLabel.AddToClassList("clabs-graph-properties__empty");
            PropertyPanel.Content.Add(emptyLabel);

            splitContainer.Add(Graph);
            splitContainer.Add(PropertyPanel);
            root.Add(splitContainer);

            StatusBar = CLabsStatusBar.Create();
            root.Add(StatusBar);
        }

        private void OnDestroy() {
            OnGraphDestroy();
        }

        private void LoadStyleSheets(VisualElement root) {
            var theme = LoadStyleSheet(ThemePath);
            if (theme == null) theme = FindStyleSheetByName("CrumpetLabs");
            if (theme != null) root.styleSheets.Add(theme);
        }

        private static StyleSheet LoadStyleSheet(string path) {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        }

        protected static StyleSheet FindStyleSheetByName(string name) {
            var guids = AssetDatabase.FindAssets($"t:StyleSheet {name}");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith($"{name}.uss"))
                    return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            }
            return null;
        }
    }
}
