using Buttr.Core;
using CLabs.Tickets;
using UnityEngine;

namespace CLabs.Adapters
{
    /// <summary>
    /// Buttr package entry point for the Unity Ticket adapter. Registers the
    /// engine bindings that <see cref="TicketRuntime"/> forwards through on
    /// the async/await hot path. All bindings are static method function
    /// pointers — no delegate instances, no boxing, no per-call allocation.
    /// </summary>
    public static class TicketUnityPackage
    {
        /// <summary>
        /// Buttr application-builder extension. No-op at present — Unity
        /// bindings are registered unconditionally during Unity startup via
        /// <see cref="InitializeTicketRuntime"/> so that core await paths work
        /// before (or without) a Buttr container. Kept for convention and for
        /// future hooks (e.g. registering optional services).
        /// </summary>
        public static IConfigurableCollection UseTicketUnityPackage(this ApplicationBuilder builder)
        {
            return new ConfigurableCollection();
        }

        /// <summary>
        /// Unity-side static initializer. Runs at <c>BeforeSceneLoad</c> which
        /// is guaranteed to execute after <c>PlayerLoopHelper.Init</c>
        /// (<c>AfterAssembliesLoaded</c>), so the Unity synchronization context
        /// and main-thread id are already populated when we capture them.
        /// Idempotent across Unity domain reloads — static fields reset, this
        /// method re-runs, bindings re-register.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static unsafe void InitializeTicketRuntime()
        {
            TicketRuntime.RegisterEngineBindings(
                addAction: &PlayerLoopHelper.AddAction,
                addContinuation: &PlayerLoopHelper.AddContinuation,
                mainThreadId: &TicketUnityBindings.GetMainThreadId,
                isMainThread: &TicketUnityBindings.GetIsMainThread,
                applicationDataPath: &TicketUnityBindings.GetApplicationDataPath,
                frameCount: &TicketUnityBindings.GetFrameCount,
                deltaTime: &TicketUnityBindings.GetDeltaTime,
                unscaledDeltaTime: &TicketUnityBindings.GetUnscaledDeltaTime,
                isPlaying: &TicketUnityBindings.GetIsPlaying,
                isEngineObject: &TicketUnityBindings.IsEngineObject,
                isEngineObjectAlive: &TicketUnityBindings.IsEngineObjectAlive,
                mainThreadSynchronizationContext: PlayerLoopHelper.UnitySynchronizationContext);

            // Register Unity math types as "contains no managed references" so
            // TaskPool<T> and friends can skip the reference-tracking path for
            // them. This restores upstream UniTask's optimization for
            // Vector/Color/Rect/etc. after the Phase B engine separation.
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Vector2));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Vector3));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Vector4));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Color));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Rect));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Bounds));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Quaternion));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Vector2Int));
            TicketRuntime.RegisterWellKnownNoReferenceType(typeof(Vector3Int));
        }
    }

    /// <summary>
    /// Static wrapper methods that serve as function-pointer targets for
    /// <see cref="TicketRuntime"/>. Kept separate from <c>PlayerLoopHelper</c>
    /// so that <c>PlayerLoopHelper</c>'s upstream UniTask surface stays
    /// untouched, and so that properties (<c>Time.frameCount</c>,
    /// <c>Application.isPlaying</c>, etc.) can be wrapped in static methods
    /// that <c>delegate*&lt;...&gt;</c> can target.
    /// </summary>
    internal static class TicketUnityBindings
    {
        public static int GetMainThreadId() => PlayerLoopHelper.MainThreadId;
        public static bool GetIsMainThread() => PlayerLoopHelper.IsMainThread;
        public static string GetApplicationDataPath() => PlayerLoopHelper.ApplicationDataPath;
        public static int GetFrameCount() => Time.frameCount;
        public static float GetDeltaTime() => Time.deltaTime;
        public static float GetUnscaledDeltaTime() => Time.unscaledDeltaTime;
        public static bool GetIsPlaying() => UnityEngine.Application.isPlaying;

        public static bool IsEngineObject(object target) => target is Object;

        public static bool IsEngineObjectAlive(object target)
        {
            // Uses Unity's overloaded == operator — a destroyed Unity.Object
            // compares equal to null even though the managed reference is
            // non-null. That's the whole reason engine-object detection
            // needs to live on the engine side.
            var unityObject = target as Object;
            return unityObject != null;
        }
    }
}
