using System;
using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Belfry.Samples {
    /// <summary>Front of house hears the bell and runs the order out. Knows nothing about the kitchen.</summary>
    public sealed partial class FrontOfHouse : MonoBehaviour {
        [Inject] private IBellTower i_Tower;
        private IDisposable m_Subscription;

        private void OnEnable() =>
            m_Subscription = i_Tower.Rope(PassBellKeys.Service).OnBell<CrumpetReady>(OnCrumpetReady);

        private void OnDisable() => m_Subscription?.Dispose();

        private void OnCrumpetReady(in CrumpetReady order) =>
            Debug.Log($"[Front of House] Running {order.Count} crumpet(s) out to {order.Table}.");
    }
}
