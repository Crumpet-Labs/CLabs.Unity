using System;
using Buttr.Core;
using CLabs.Spoon;
using UnityEngine;

namespace CLabs.Adapters
{
    public abstract class SpoonView<TState> : MonoBehaviour where TState : struct
    {
        private IStore<TState> m_Store;
        private IDisposable m_Subscription;

        protected IStore<TState> Store => m_Store ??= Application<IStore<TState>>.Get();

        protected TState State => Store.State;

        protected virtual void OnEnable()
        {
            var store = Store;
            if (store == null) return;

            m_Subscription = store.Subscribe(OnStateChanged);
            var snapshot = store.State;
            OnStateChanged(in snapshot);
        }

        protected virtual void OnDisable()
        {
            m_Subscription?.Dispose();
            m_Subscription = null;
        }

        protected abstract void OnStateChanged(in TState state);
    }
}
