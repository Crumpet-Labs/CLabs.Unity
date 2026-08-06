using System;
using UnityEngine.LowLevel;

namespace CLabs.Utility {
    public static class PlayerLoopInjector {
        private static readonly PlayerLoopFanOut s_FanOut = new(InsertSubsystem, RemoveSubsystem);

        /// <summary>
        /// Hook <paramref name="customUpdate"/> into Unity's PlayerLoop Update phase. The type parameter
        /// <typeparamref name="T"/> names a shared channel: every caller using the same <typeparamref name="T"/>
        /// shares a single PlayerLoop subsystem, and disposing the returned handle removes only that callback,
        /// siblings on the same channel keep running. The subsystem is removed once the last subscriber disposes.
        /// </summary>
        public static IDisposable InjectUpdate<T>(Action customUpdate) =>
            s_FanOut.Subscribe(typeof(T), customUpdate);

        private static void InsertSubsystem(Type channelKey, Action onUpdate) {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            for (var i = 0; i < playerLoop.subSystemList.Length; i++) {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                    continue;

                var newSystem = new PlayerLoopSystem {
                    type = channelKey,
                    updateDelegate = () => onUpdate()
                };

                var subSystems = playerLoop.subSystemList[i].subSystemList;
                Array.Resize(ref subSystems, subSystems.Length + 1);
                subSystems[^1] = newSystem;
                playerLoop.subSystemList[i].subSystemList = subSystems;
                break;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static void RemoveSubsystem(Type channelKey) {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            for (var i = 0; i < playerLoop.subSystemList.Length; i++) {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                    continue;

                var subSystems = playerLoop.subSystemList[i].subSystemList;
                subSystems = Array.FindAll(subSystems, sys => sys.type != channelKey);
                playerLoop.subSystemList[i].subSystemList = subSystems;
                break;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }
    }
}
