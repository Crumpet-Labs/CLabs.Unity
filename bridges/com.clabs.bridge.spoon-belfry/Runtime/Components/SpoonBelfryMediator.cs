using System;
using CLabs.Belfry;
using CLabs.Spoon;

namespace CLabs.Bridges {
    public sealed class SpoonBelfryMediator<TState> : IDisposable where TState : struct {
        private readonly IDisposable m_Subscription;

        public SpoonBelfryMediator(IBellTower tower, IStore<TState> store, object key) {
            if (tower == null) throw new ArgumentNullException(nameof(tower));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var rope = tower.Rope(key);
            m_Subscription = store.Subscribe((in TState state) => {
                var msg = new SpoonStateChangedMessage<TState>(state);
                rope.RingBell(in msg);
            });
        }

        public void Dispose() => m_Subscription?.Dispose();
    }
}