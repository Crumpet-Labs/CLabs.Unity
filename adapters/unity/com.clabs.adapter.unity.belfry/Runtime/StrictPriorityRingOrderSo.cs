using CLabs.Belfry;
using UnityEngine;

namespace CLabs.Adapters {
    [CreateAssetMenu(menuName = "Belfry/Ring Orders/Strict Priority")]
    public sealed class StrictPriorityRingOrderSo : RingOrderSo {
        private StrictPriorityRingOrder m_RingOrder = new();
        protected override IRingOrder RingOrder => m_RingOrder;
        private void OnEnable() => m_RingOrder = new StrictPriorityRingOrder();
    }
}
