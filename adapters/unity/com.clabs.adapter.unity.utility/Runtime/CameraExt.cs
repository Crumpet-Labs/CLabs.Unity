using UnityEngine;

namespace CLabs.Utility {
    public static class CameraExtensions {
        /// <summary>
        /// Removes a specific layer from the camera's culling mask.
        /// </summary>
        /// <param name="camera">The camera to modify.</param>
        /// <param name="layer">The layer to remove (use LayerMask.NameToLayer for the layer index).</param>
        public static void RemoveLayer(this Camera camera, LayerMask layer)
            => camera.cullingMask &= ~(1 << layer);

        /// <summary>
        /// Adds a specific layer to the camera's culling mask.
        /// </summary>
        /// <param name="camera">The camera to modify.</param>
        /// <param name="layer">The layer to add (use LayerMask.NameToLayer for the layer index).</param>
        public static void AddLayer(this Camera camera, LayerMask layer)
            => camera.cullingMask |= (1 << layer);

        /// <summary>
        /// Checks if a specific layer is included in the camera's culling mask.
        /// </summary>
        /// <param name="camera">The camera to check.</param>
        /// <param name="layer">The layer to check (use LayerMask.NameToLayer for the layer index).</param>
        /// <returns>True if the layer is included, false otherwise.</returns>
        public static bool HasLayer(this Camera camera, LayerMask layer)
            => (camera.cullingMask & (1 << layer)) != 0;

        public static void ExcludeLayer(this Camera camera, LayerMask layer) 
            => camera.cullingMask &= ~(1 << layer);
    }
}