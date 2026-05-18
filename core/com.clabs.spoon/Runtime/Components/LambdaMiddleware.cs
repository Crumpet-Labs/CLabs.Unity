using System;

namespace CLabs.Spoon {
    public sealed class LambdaMiddleware<TState> : IMiddleware<TState> where TState : struct {
        private readonly Action<IStore<TState>, IAction, SpoonDispatch> m_Fn;

        public LambdaMiddleware(Action<IStore<TState>, IAction, SpoonDispatch> fn) {
            m_Fn = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        public void Invoke(IStore<TState> store, IAction action, SpoonDispatch next)
            => m_Fn(store, action, next);
    }
}