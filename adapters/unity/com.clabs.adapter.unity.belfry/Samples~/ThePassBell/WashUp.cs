using System;
using Buttr.Injection;
using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Belfry.Samples {
    /// <summary>Wash-up also hears the same bell and resets the proving rings. Independent of Front of House.</summary>
    public sealed partial class WashUp : MonoBehaviour {
        [Inject] private IBellTower i_Tower;
        private IDisposable m_Subscription;

        private void OnEnable() =>
            m_Subscription = i_Tower.Rope(PassBellKeys.Service).OnBell<CrumpetReady>(OnCrumpetReady);

        private void OnDisable() => m_Subscription?.Dispose();

        private void OnCrumpetReady(in CrumpetReady order) =>
            Debug.Log($"[Wash Up] Clearing {order.Count} proving ring(s) for the next batch.");
    }
}
