#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using CLabs.Tickets.Internal;

namespace CLabs.Tickets
{
    /// <summary>
    /// Static host for the engine frame-loop + timing bindings that Ticket's
    /// core call sites need. Engine adapters populate the static function
    /// pointers once at startup (e.g. [RuntimeInitializeOnLoadMethod] on Unity)
    /// via <see cref="RegisterEngineBindings"/>. Core code then dispatches
    /// through the function pointers, producing direct-call codegen instead of
    /// interface dispatch, preserving UniTask's zero-overhead hot path.
    ///
    /// This class is an acknowledged exception to the CLabs "no static Runtime
    /// classes" convention. Ticket's compiler-generated state machines cannot
    /// receive a constructor parameter, so a static lookup is unavoidable on
    /// the AsyncMethodBuilder hot path. Function pointers get us back to
    /// upstream-parity perf for that lookup.
    ///
    /// <b>Leak safety:</b> function pointers (<c>delegate*&lt;...&gt;</c>) are
    /// raw IL call targets, not reference types, so assigning them does not root
    /// any managed object and cannot capture closures. All registered targets
    /// must be <c>static</c> methods, so there is no instance lifetime tie
    /// between this class and the adapter assembly. The one exception is
    /// <see cref="MainThreadSynchronizationContext"/>, which is a real managed
    /// reference held for the process lifetime. This is intentional (the
    /// main-thread sync context IS the process on the engines we target) but
    /// call <see cref="UnregisterEngineBindings"/> if you need to drop the
    /// reference explicitly (tests, engine switching, etc.). On Unity domain
    /// reload all static fields on this class reset automatically and the
    /// adapter's <c>[RuntimeInitializeOnLoadMethod]</c> re-registers cleanly.
    /// </summary>
    public static unsafe class TicketRuntime
    {
        // Frame-loop scheduling
        static delegate*<PlayerLoopTiming, IPlayerLoopItem, void> addActionFn;
        static delegate*<PlayerLoopTiming, Action, void> addContinuationFn;

        // Thread identity
        static delegate*<int> mainThreadIdFn;
        static delegate*<bool> isMainThreadFn;

        // Engine metadata
        static delegate*<string> applicationDataPathFn;
        static delegate*<int> frameCountFn;
        static delegate*<float> deltaTimeFn;
        static delegate*<float> unscaledDeltaTimeFn;
        static delegate*<bool> isPlayingFn;

        // Engine-object detection (for zero-alloc WaitUntilValueChanged /
        // EveryValueChanged on Unity / Godot targets)
        static delegate*<object, bool> isEngineObjectFn;
        static delegate*<object, bool> isEngineObjectAliveFn;

        // Main-thread SynchronizationContext (reference type, not a function
        // pointer target; adapter assigns this field directly)
        static SynchronizationContext mainThreadSyncContext;

        public static void AddAction(PlayerLoopTiming timing, IPlayerLoopItem item)
        {
            if (addActionFn == null) ThrowNotRegistered();
            addActionFn(timing, item);
        }

        public static void AddContinuation(PlayerLoopTiming timing, Action continuation)
        {
            if (addContinuationFn == null) ThrowNotRegistered();
            addContinuationFn(timing, continuation);
        }

        public static int MainThreadId => mainThreadIdFn == null ? 0 : mainThreadIdFn();

        public static bool IsMainThread => isMainThreadFn == null ? true : isMainThreadFn();

        public static string ApplicationDataPath => applicationDataPathFn == null ? null : applicationDataPathFn();

        public static int FrameCount => frameCountFn == null ? 0 : frameCountFn();

        public static float DeltaTime => deltaTimeFn == null ? 0f : deltaTimeFn();

        public static float UnscaledDeltaTime => unscaledDeltaTimeFn == null ? 0f : unscaledDeltaTimeFn();

        public static bool IsPlaying => isPlayingFn == null ? true : isPlayingFn();

        public static SynchronizationContext MainThreadSynchronizationContext => mainThreadSyncContext;

        /// <summary>
        /// True when an engine adapter has registered an engine-object detector
        /// (i.e. Unity or Godot is hosting). Used by WaitUntilValueChanged /
        /// EveryValueChanged to pick the zero-alloc direct-ref promise path
        /// instead of the WeakReference fallback.
        /// </summary>
        public static bool HasEngineObjectDetector => isEngineObjectFn != null;

        /// <summary>
        /// Returns true if <paramref name="target"/> is an engine-tracked
        /// object (e.g. Unity Object, Godot Node). Returns false if no
        /// detector is registered. Callers should gate on
        /// <see cref="HasEngineObjectDetector"/> first when the distinction
        /// between "not engine" and "no detector" matters.
        /// </summary>
        public static bool IsEngineObject(object target)
        {
            return isEngineObjectFn != null && isEngineObjectFn(target);
        }

        /// <summary>
        /// Returns true if <paramref name="target"/> is an engine-tracked
        /// object that is still alive. Behaviour is undefined (but safe) when
        /// called on non-engine objects. Returns true if no detector is
        /// registered (assume alive).
        /// </summary>
        public static bool IsEngineObjectAlive(object target)
        {
            return isEngineObjectAliveFn == null || isEngineObjectAliveFn(target);
        }

        /// <summary>
        /// Register the engine bindings. Called once per process during adapter
        /// init. Pass null for any binding the engine does not support, and Ticket
        /// will fall back to safe defaults (IsPlaying=true, FrameCount=0, etc.).
        /// </summary>
        public static void RegisterEngineBindings(
            delegate*<PlayerLoopTiming, IPlayerLoopItem, void> addAction,
            delegate*<PlayerLoopTiming, Action, void> addContinuation,
            delegate*<int> mainThreadId,
            delegate*<bool> isMainThread,
            delegate*<string> applicationDataPath,
            delegate*<int> frameCount,
            delegate*<float> deltaTime,
            delegate*<float> unscaledDeltaTime,
            delegate*<bool> isPlaying,
            delegate*<object, bool> isEngineObject,
            delegate*<object, bool> isEngineObjectAlive,
            SynchronizationContext mainThreadSynchronizationContext)
        {
            addActionFn = addAction;
            addContinuationFn = addContinuation;
            mainThreadIdFn = mainThreadId;
            isMainThreadFn = isMainThread;
            applicationDataPathFn = applicationDataPath;
            frameCountFn = frameCount;
            deltaTimeFn = deltaTime;
            unscaledDeltaTimeFn = unscaledDeltaTime;
            isPlayingFn = isPlaying;
            isEngineObjectFn = isEngineObject;
            isEngineObjectAliveFn = isEngineObjectAlive;
            mainThreadSyncContext = mainThreadSynchronizationContext;
        }

        /// <summary>
        /// Register an engine value type as containing no managed references
        /// (e.g. Unity's <c>Vector3</c>, Godot's <c>Vector3</c>). Ticket's
        /// pooled promise/channel infrastructure uses this hint to pick
        /// faster reference-tracking-free code paths for those types. Call
        /// from adapter static init, once per type, before any Ticket code
        /// executes (the result is cached per-T on first access).
        /// </summary>
        public static void RegisterWellKnownNoReferenceType(Type type)
        {
            if (type == null) return;
            RuntimeHelpersAbstraction.AdditionalWellKnownTypes.Add(type);
        }

        /// <summary>
        /// Clear every registered binding. The AddAction / AddContinuation
        /// paths will throw until another adapter registers. Use from tests
        /// that set up and tear down a synthetic engine, or when an
        /// application explicitly shuts down an engine adapter. No-op if
        /// nothing is currently registered.
        /// </summary>
        public static void UnregisterEngineBindings()
        {
            addActionFn = null;
            addContinuationFn = null;
            mainThreadIdFn = null;
            isMainThreadFn = null;
            applicationDataPathFn = null;
            frameCountFn = null;
            deltaTimeFn = null;
            unscaledDeltaTimeFn = null;
            isPlayingFn = null;
            isEngineObjectFn = null;
            isEngineObjectAliveFn = null;
            mainThreadSyncContext = null;
        }

        static void ThrowNotRegistered()
        {
            throw new InvalidOperationException(
                "TicketRuntime has no engine bindings registered. On Unity, ensure " +
                "com.clabs.adapter.unity.ticket is installed (its [RuntimeInitializeOnLoadMethod] " +
                "registers bindings automatically) or call the adapter's UseTicketUnityPackage() " +
                "extension during ApplicationBuilder startup.");
        }
    }
}
