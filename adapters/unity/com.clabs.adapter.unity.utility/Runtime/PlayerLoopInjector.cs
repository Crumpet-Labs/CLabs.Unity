using System;
using UnityEngine.LowLevel;

namespace CLabs.Utility {
    public static class PlayerLoopInjector {
        public static IDisposable InjectUpdate<T>(Action customUpdate) {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        
            for (var i = 0; i < playerLoop.subSystemList.Length; i++) {
                if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                    continue;

                var newSystem = new PlayerLoopSystem {
                    type = typeof(T),
                    updateDelegate = () => customUpdate?.Invoke()
                };

                var subSystems = playerLoop.subSystemList[i].subSystemList;
                Array.Resize(ref subSystems, subSystems.Length + 1);
                subSystems[^1] = newSystem;
                playerLoop.subSystemList[i].subSystemList = subSystems;
                break;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);

            return new LoopSubscription<T>();
        }
        
        private sealed class LoopSubscription<T> : IDisposable {
            public void Dispose() {
                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

                for (var i = 0; i < playerLoop.subSystemList.Length; i++) {
                    if (playerLoop.subSystemList[i].type != typeof(UnityEngine.PlayerLoop.Update))
                        continue;

                    var subSystems = playerLoop.subSystemList[i].subSystemList;
                    subSystems = Array.FindAll(subSystems, sys => sys.type != typeof(T));
                    playerLoop.subSystemList[i].subSystemList = subSystems;
                    break;
                }

                PlayerLoop.SetPlayerLoop(playerLoop);
            }
        }
    }
}