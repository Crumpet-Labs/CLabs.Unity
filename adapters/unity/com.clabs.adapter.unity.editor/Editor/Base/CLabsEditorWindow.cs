using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CLabs.Editor {
    public abstract class CLabsEditorWindow : EditorWindow {
        private const string ThemePath = "Packages/com.clabs.adapter.unity.editor/Editor/Styles/CrumpetLabs.uss";

        protected abstract string WindowTitle { get; }

        /// <summary>
        /// Optional path to a window-specific stylesheet, e.g. a Packages/ asset path for UPM installs.
        /// When the path does not resolve (DLL consumers have no package folder), the sheet is located
        /// by filename via an AssetDatabase search, so shipping the .uss anywhere in the project suffices.
        /// </summary>
        protected virtual string PackageStyleSheetPath => null;

        public void CreateGUI() {
            var root = rootVisualElement;

            LoadStyleSheets(root);
            root.Add(CLabsHeader.Create(WindowTitle));
            OnCreateContent(root);

            EditorApplication.update += OnEditorTick;
        }

        private void OnDestroy() {
            EditorApplication.update -= OnEditorTick;
            OnWindowDestroy();
        }

        protected abstract void OnCreateContent(VisualElement root);
        protected virtual void OnEditorUpdate() { }
        protected virtual void OnWindowDestroy() { }

        private void OnEditorTick() {
            OnEditorUpdate();
        }

        private void LoadStyleSheets(VisualElement root) {
            var theme = LoadStyleSheet(ThemePath);
            if (theme == null) {
                theme = FindStyleSheetByName("CrumpetLabs");
            }

            if (theme != null) {
                root.styleSheets.Add(theme);
            }

            var packagePath = PackageStyleSheetPath;
            if (!string.IsNullOrEmpty(packagePath)) {
                var packageSheet = LoadStyleSheet(packagePath);
                if (packageSheet == null) {
                    packageSheet = FindStyleSheetByName(Path.GetFileNameWithoutExtension(packagePath));
                }

                if (packageSheet != null) {
                    root.styleSheets.Add(packageSheet);
                }
            }
        }

        private static StyleSheet LoadStyleSheet(string path) {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        }

        private static StyleSheet FindStyleSheetByName(string name) {
            var guids = AssetDatabase.FindAssets($"t:StyleSheet {name}");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith($"{name}.uss")) {
                    return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                }
            }
            return null;
        }
    }
}
