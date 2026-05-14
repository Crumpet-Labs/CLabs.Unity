using UnityEditor;
using UnityEditor.AddressableAssets;

namespace CLabs.Utility.Unity.Editor {
    public static class AddressableUtilities {
        public static string GetAssetAddress(this UnityEngine.Object @object) {
            var assetPath = AssetDatabase.GetAssetPath(@object);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings?.FindAssetEntry(guid);
            return entry?.address ?? string.Empty;
        }
    }
}