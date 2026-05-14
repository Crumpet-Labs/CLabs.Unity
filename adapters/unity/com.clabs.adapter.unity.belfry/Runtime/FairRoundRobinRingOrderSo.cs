using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    [CreateAssetMenu(menuName = "Belfry/Ring Orders/Fair Round Robin")]
    public sealed class FairRoundRobinRingOrderSo : RingOrderSo {
        private FairRoundRobinRingOrder m_RingOrder = new();
        protected override IRingOrder RingOrder => m_RingOrder;
        private void OnEnable() => m_RingOrder = new FairRoundRobinRingOrder();
    }
}
