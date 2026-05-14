#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using UnityEngine;
using System.Threading;
using CLabs.Tickets.Internal;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.LowLevel;
using PlayerLoopType = UnityEngine.PlayerLoop;
#else
using UnityEngine.Experimental.LowLevel;
using PlayerLoopType = UnityEngine.Experimental.PlayerLoop;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CLabs.Tickets
{
    public static class TicketLoopRunners
    {
        public struct TicketLoopRunnerInitialization { };
        public struct TicketLoopRunnerEarlyUpdate { };
        public struct TicketLoopRunnerFixedUpdate { };
        public struct TicketLoopRunnerPreUpdate { };
        public struct TicketLoopRunnerUpdate { };
        public struct TicketLoopRunnerPreLateUpdate { };
        public struct TicketLoopRunnerPostLateUpdate { };

        // Last

        public struct TicketLoopRunnerLastInitialization { };
        public struct TicketLoopRunnerLastEarlyUpdate { };
        public struct TicketLoopRunnerLastFixedUpdate { };
        public struct TicketLoopRunnerLastPreUpdate { };
        public struct TicketLoopRunnerLastUpdate { };
        public struct TicketLoopRunnerLastPreLateUpdate { };
        public struct TicketLoopRunnerLastPostLateUpdate { };

        // Yield

        public struct TicketLoopRunnerYieldInitialization { };
        public struct TicketLoopRunnerYieldEarlyUpdate { };
        public struct TicketLoopRunnerYieldFixedUpdate { };
        public struct TicketLoopRunnerYieldPreUpdate { };
        public struct TicketLoopRunnerYieldUpdate { };
        public struct TicketLoopRunnerYieldPreLateUpdate { };
        public struct TicketLoopRunnerYieldPostLateUpdate { };

        // Yield Last

        public struct TicketLoopRunnerLastYieldInitialization { };
        public struct TicketLoopRunnerLastYieldEarlyUpdate { };
        public struct TicketLoopRunnerLastYieldFixedUpdate { };
        public struct TicketLoopRunnerLastYieldPreUpdate { };
        public struct TicketLoopRunnerLastYieldUpdate { };
        public struct TicketLoopRunnerLastYieldPreLateUpdate { };
        public struct TicketLoopRunnerLastYieldPostLateUpdate { };

#if UNITY_2020_2_OR_NEWER
        public struct TicketLoopRunnerTimeUpdate { };
        public struct TicketLoopRunnerLastTimeUpdate { };
        public struct TicketLoopRunnerYieldTimeUpdate { };
        public struct TicketLoopRunnerLastYieldTimeUpdate { };
#endif
    }

    // PlayerLoopTiming has been extracted to Common/PlayerLoopTiming.cs (Phase B).

    [Flags]
    public enum InjectPlayerLoopTimings
    {
        /// <summary>
        /// Preset: All loops(default).
        /// </summary>
        All =
            Initialization | LastInitialization |
            EarlyUpdate | LastEarlyUpdate |
            FixedUpdate | LastFixedUpdate |
            PreUpdate | LastPreUpdate |
            Update | LastUpdate |
            PreLateUpdate | LastPreLateUpdate |
            PostLateUpdate | LastPostLateUpdate
#if UNITY_2020_2_OR_NEWER
            | TimeUpdate | LastTimeUpdate,
#else
            ,
#endif

        /// <summary>
        /// Preset: All without last except LastPostLateUpdate.
        /// </summary>
        Standard =
            Initialization |
            EarlyUpdate |
            FixedUpdate |
            PreUpdate |
            Update |
            PreLateUpdate |
            PostLateUpdate | LastPostLateUpdate
#if UNITY_2020_2_OR_NEWER
            | TimeUpdate
#endif
            ,

        /// <summary>
        /// Preset: Minimum pattern, Update | FixedUpdate | LastPostLateUpdate
        /// </summary>
        Minimum =
            Update | FixedUpdate | LastPostLateUpdate,

        // PlayerLoopTiming

        Initialization = 1,
        LastInitialization = 2,

        EarlyUpdate = 4,
        LastEarlyUpdate = 8,

        FixedUpdate = 16,
        LastFixedUpdate = 32,

        PreUpdate = 64,
        LastPreUpdate = 128,

        Update = 256,
        LastUpdate = 512,

        PreLateUpdate = 1024,
        LastPreLateUpdate = 2048,

        PostLateUpdate = 4096,
        LastPostLateUpdate = 8192

#if UNITY_2020_2_OR_NEWER
        ,
        // Unity 2020.2 added TimeUpdate https://docs.unity3d.com/2020.2/Documentation/ScriptReference/PlayerLoop.TimeUpdate.html
        TimeUpdate = 16384,
        LastTimeUpdate = 32768
#endif
    }

    // IPlayerLoopItem has been extracted to Contracts/IPlayerLoopItem.cs (Phase B).

    public static class PlayerLoopHelper
    {
        static readonly ContinuationQueue ThrowMarkerContinuationQueue = new ContinuationQueue(PlayerLoopTiming.Initialization);
        static readonly PlayerLoopRunner ThrowMarkerPlayerLoopRunner = new PlayerLoopRunner(PlayerLoopTiming.Initialization);

        public static SynchronizationContext UnitySynchronizationContext => unitySynchronizationContext;
        public static int MainThreadId => mainThreadId;
        internal static string ApplicationDataPath => applicationDataPath;

        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId;

        static int mainThreadId;
        static string applicationDataPath;
        static SynchronizationContext unitySynchronizationContext;
        static ContinuationQueue[] yielders;
        static PlayerLoopRunner[] runners;
        internal static bool IsEditorApplicationQuitting { get; private set; }
        static PlayerLoopSystem[] InsertRunner(PlayerLoopSystem loopSystem,
            bool injectOnFirst,
            Type loopRunnerYieldType, ContinuationQueue cq,
            Type loopRunnerType, PlayerLoopRunner runner)
        {

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingEditMode)
                {
                    IsEditorApplicationQuitting = true;
                    // run rest action before clear.
                    if (runner != null)
                    {
                        runner.Run();
                        runner.Clear();
                    }
                    if (cq != null)
                    {
                        cq.Run();
                        cq.Clear();
                    }
                    IsEditorApplicationQuitting = false;
                }
            };
#endif

            var yieldLoop = new PlayerLoopSystem
            {
                type = loopRunnerYieldType,
                updateDelegate = cq.Run
            };

            var runnerLoop = new PlayerLoopSystem
            {
                type = loopRunnerType,
                updateDelegate = runner.Run
            };

            // Remove items from previous initializations.
            var source = RemoveRunner(loopSystem, loopRunnerYieldType, loopRunnerType);
            var dest = new PlayerLoopSystem[source.Length + 2];

            Array.Copy(source, 0, dest, injectOnFirst ? 2 : 0, source.Length);
            if (injectOnFirst)
            {
                dest[0] = yieldLoop;
                dest[1] = runnerLoop;
            }
            else
            {
                dest[dest.Length - 2] = yieldLoop;
                dest[dest.Length - 1] = runnerLoop;
            }

            return dest;
        }

        static PlayerLoopSystem[] RemoveRunner(PlayerLoopSystem loopSystem, Type loopRunnerYieldType, Type loopRunnerType)
        {
            return loopSystem.subSystemList
                .Where(ls => ls.type != loopRunnerYieldType && ls.type != loopRunnerType)
                .ToArray();
        }

        static PlayerLoopSystem[] InsertTicketSynchronizationContext(PlayerLoopSystem loopSystem)
        {
            var loop = new PlayerLoopSystem
            {
                type = typeof(TicketSynchronizationContext),
                updateDelegate = TicketSynchronizationContext.Run
            };

            // Remove items from previous initializations.
            var source = loopSystem.subSystemList
                .Where(ls => ls.type != typeof(TicketSynchronizationContext))
                .ToArray();

            var dest = new System.Collections.Generic.List<PlayerLoopSystem>(source);

            var index = dest.FindIndex(x => x.type.Name == "ScriptRunDelayedTasks");
            if (index == -1)
            {
                index = dest.FindIndex(x => x.type.Name == "TicketLoopRunnerUpdate");
            }

            dest.Insert(index + 1, loop);

            return dest.ToArray();
        }

#if UNITY_2020_1_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static void Init()
        {
            // capture default(unity) sync-context.
            unitySynchronizationContext = SynchronizationContext.Current;
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                applicationDataPath = Application.dataPath;
            }
            catch { }

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
            // When domain reload is disabled, re-initialization is required when entering play mode; 
            // otherwise, pending tasks will leak between play mode sessions.
            var domainReloadDisabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled &&
                UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload);
            if (!domainReloadDisabled && runners != null) return;
#else
            if (runners != null) return; // already initialized
#endif

            var playerLoop =
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif

            Initialize(ref playerLoop);
        }


#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        static void InitOnEditor()
        {
            // Execute the play mode init method
            Init();

            // register an Editor update delegate, used to forcing playerLoop update
            EditorApplication.update += ForceEditorPlayerLoopUpdate;
        }

        private static void ForceEditorPlayerLoopUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                // Not in Edit mode, don't interfere
                return;
            }

            // EditorApplication.QueuePlayerLoopUpdate causes performance issue, don't call directly.
            // EditorApplication.QueuePlayerLoopUpdate();

            if (yielders != null)
            {
                foreach (var item in yielders)
                {
                    if (item != null) item.Run();
                }
            }

            if (runners != null)
            {
                foreach (var item in runners)
                {
                    if (item != null) item.Run();
                }
            }

            TicketSynchronizationContext.Run();
        }

#endif

        private static int FindLoopSystemIndex(PlayerLoopSystem[] playerLoopList, Type systemType)
        {
            for (int i = 0; i < playerLoopList.Length; i++)
            {
                if (playerLoopList[i].type == systemType)
                {
                    return i;
                }
            }

            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }

        static void InsertLoop(PlayerLoopSystem[] copyList, InjectPlayerLoopTimings injectTimings, Type loopType, InjectPlayerLoopTimings targetTimings,
            int index, bool injectOnFirst, Type loopRunnerYieldType, Type loopRunnerType, PlayerLoopTiming playerLoopTiming)
        {
            var i = FindLoopSystemIndex(copyList, loopType);
            if ((injectTimings & targetTimings) == targetTimings)
            {
                copyList[i].subSystemList = InsertRunner(copyList[i], injectOnFirst,
                    loopRunnerYieldType, yielders[index] = new ContinuationQueue(playerLoopTiming),
                    loopRunnerType, runners[index] = new PlayerLoopRunner(playerLoopTiming));
            }
            else
            {
                copyList[i].subSystemList = RemoveRunner(copyList[i], loopRunnerYieldType, loopRunnerType);
            }
        }

        public static void Initialize(ref PlayerLoopSystem playerLoop, InjectPlayerLoopTimings injectTimings = InjectPlayerLoopTimings.All)
        {
#if UNITY_2020_2_OR_NEWER
            yielders = new ContinuationQueue[16];
            runners = new PlayerLoopRunner[16];
#else
            yielders = new ContinuationQueue[14];
            runners = new PlayerLoopRunner[14];
#endif

            var copyList = playerLoop.subSystemList.ToArray();

            // Initialization
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Initialization),
                InjectPlayerLoopTimings.Initialization, 0, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldInitialization), typeof(TicketLoopRunners.TicketLoopRunnerInitialization), PlayerLoopTiming.Initialization);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Initialization),
                InjectPlayerLoopTimings.LastInitialization, 1, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldInitialization), typeof(TicketLoopRunners.TicketLoopRunnerLastInitialization), PlayerLoopTiming.LastInitialization);

            // EarlyUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.EarlyUpdate),
                InjectPlayerLoopTimings.EarlyUpdate, 2, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldEarlyUpdate), typeof(TicketLoopRunners.TicketLoopRunnerEarlyUpdate), PlayerLoopTiming.EarlyUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.EarlyUpdate),
                InjectPlayerLoopTimings.LastEarlyUpdate, 3, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldEarlyUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastEarlyUpdate), PlayerLoopTiming.LastEarlyUpdate);

            // FixedUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.FixedUpdate),
                InjectPlayerLoopTimings.FixedUpdate, 4, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldFixedUpdate), typeof(TicketLoopRunners.TicketLoopRunnerFixedUpdate), PlayerLoopTiming.FixedUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.FixedUpdate),
                InjectPlayerLoopTimings.LastFixedUpdate, 5, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldFixedUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastFixedUpdate), PlayerLoopTiming.LastFixedUpdate);

            // PreUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreUpdate),
                InjectPlayerLoopTimings.PreUpdate, 6, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldPreUpdate), typeof(TicketLoopRunners.TicketLoopRunnerPreUpdate), PlayerLoopTiming.PreUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreUpdate),
                InjectPlayerLoopTimings.LastPreUpdate, 7, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldPreUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastPreUpdate), PlayerLoopTiming.LastPreUpdate);

            // Update
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Update),
                InjectPlayerLoopTimings.Update, 8, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldUpdate), typeof(TicketLoopRunners.TicketLoopRunnerUpdate), PlayerLoopTiming.Update);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Update),
                InjectPlayerLoopTimings.LastUpdate, 9, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastUpdate), PlayerLoopTiming.LastUpdate);

            // PreLateUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreLateUpdate),
                InjectPlayerLoopTimings.PreLateUpdate, 10, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldPreLateUpdate), typeof(TicketLoopRunners.TicketLoopRunnerPreLateUpdate), PlayerLoopTiming.PreLateUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreLateUpdate),
                InjectPlayerLoopTimings.LastPreLateUpdate, 11, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldPreLateUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastPreLateUpdate), PlayerLoopTiming.LastPreLateUpdate);

            // PostLateUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PostLateUpdate),
                InjectPlayerLoopTimings.PostLateUpdate, 12, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldPostLateUpdate), typeof(TicketLoopRunners.TicketLoopRunnerPostLateUpdate), PlayerLoopTiming.PostLateUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PostLateUpdate),
                InjectPlayerLoopTimings.LastPostLateUpdate, 13, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldPostLateUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastPostLateUpdate), PlayerLoopTiming.LastPostLateUpdate);

#if UNITY_2020_2_OR_NEWER
            // TimeUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.TimeUpdate),
                InjectPlayerLoopTimings.TimeUpdate, 14, true,
                typeof(TicketLoopRunners.TicketLoopRunnerYieldTimeUpdate), typeof(TicketLoopRunners.TicketLoopRunnerTimeUpdate), PlayerLoopTiming.TimeUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.TimeUpdate),
                InjectPlayerLoopTimings.LastTimeUpdate, 15, false,
                typeof(TicketLoopRunners.TicketLoopRunnerLastYieldTimeUpdate), typeof(TicketLoopRunners.TicketLoopRunnerLastTimeUpdate), PlayerLoopTiming.LastTimeUpdate);
#endif

            // Insert TicketSynchronizationContext to Update loop
            var i = FindLoopSystemIndex(copyList, typeof(PlayerLoopType.Update));
            copyList[i].subSystemList = InsertTicketSynchronizationContext(copyList[i]);

            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        public static void AddAction(PlayerLoopTiming timing, IPlayerLoopItem action)
        {
            var runner = runners[(int)timing];
            if (runner == null)
            {
                ThrowInvalidLoopTiming(timing);
            }
            runner.AddAction(action);
        }

        static void ThrowInvalidLoopTiming(PlayerLoopTiming playerLoopTiming)
        {
            throw new InvalidOperationException("Target playerLoopTiming is not injected. Please check PlayerLoopHelper.Initialize. PlayerLoopTiming:" + playerLoopTiming);
        }

        public static void AddContinuation(PlayerLoopTiming timing, Action continuation)
        {
            var q = yielders[(int)timing];
            if (q == null)
            {
                ThrowInvalidLoopTiming(timing);
            }
            q.Enqueue(continuation);
        }

        // Diagnostics helper

#if UNITY_2019_3_OR_NEWER

        /// <summary>Dumps the current Unity PlayerLoop subsystem tree to the console via Debug.Log. Intentional debug utility — logs to console by design.</summary>
        public static void DumpCurrentPlayerLoop()
        {
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"PlayerLoop List");
            foreach (var header in playerLoop.subSystemList)
            {
                sb.AppendFormat("------{0}------", header.type.Name);
                sb.AppendLine();
                
                if (header.subSystemList is null) 
                {
                    sb.AppendFormat("{0} has no subsystems!", header.ToString());
                    sb.AppendLine();
                    continue;
                }

                foreach (var subSystem in header.subSystemList)
                {
                    sb.AppendFormat("{0}", subSystem.type.Name);
                    sb.AppendLine();

                    if (subSystem.subSystemList != null)
                    {
                        UnityEngine.Debug.LogWarning("More Subsystem:" + subSystem.subSystemList.Length);
                    }
                }
            }

            UnityEngine.Debug.Log(sb.ToString());
        }

        public static bool IsInjectedTicketPlayerLoop()
        {
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();

            foreach (var header in playerLoop.subSystemList)
            {
                if (header.subSystemList is null) 
                { 
                    continue;
                }
                
                foreach (var subSystem in header.subSystemList)
                {
                    if (subSystem.type == typeof(TicketLoopRunners.TicketLoopRunnerInitialization))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

#endif

    }
}

