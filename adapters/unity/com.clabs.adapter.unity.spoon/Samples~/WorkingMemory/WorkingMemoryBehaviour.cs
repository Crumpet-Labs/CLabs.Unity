using System;
using CLabs.Spoon;
using UnityEngine;

namespace CLabs.Spoon.Samples {
    /// <summary>
    /// Stands up a KitchenState store and drives it. In Play mode press:
    /// S = serve a crumpet (+5 coin), H = heat the oven (+40°C), N = new day.
    /// Every change is logged. The store is plain C#, so no scene or DI is required to learn the loop.
    /// </summary>
    public sealed class WorkingMemoryBehaviour : MonoBehaviour {
        private Store<KitchenState> m_Store;
        private IDisposable m_Subscription;

        private void Awake() {
            m_Store = new Store<KitchenState>(new KitchenReducer(), new MiddlewareCollection<KitchenState>());
            m_Subscription = m_Store.Subscribe(OnStateChanged);
        }

        private void OnDestroy() => m_Subscription?.Dispose();

        private void Update() {
            if (Input.GetKeyDown(KeyCode.S)) m_Store.Dispatch(new ServedCrumpet(5));
            if (Input.GetKeyDown(KeyCode.H)) m_Store.Dispatch(new HeatedOven(40));
            if (Input.GetKeyDown(KeyCode.N)) m_Store.Dispatch(new NewDay());
        }

        private void OnStateChanged(in KitchenState s) =>
            Debug.Log($"[Working Memory] Day {s.Day} · {s.Coins} coin · Hob {s.OvenTemp}°C · safe to serve: {s.IsSafeToServe}");
    }
}
