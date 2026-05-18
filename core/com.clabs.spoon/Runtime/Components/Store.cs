using System;

namespace CLabs.Spoon {
    public sealed class Store<TState> : IStore<TState> where TState : struct {
        private readonly IReducer<TState> m_Reducer;
        private readonly SpoonDispatch m_DispatchEntry;

        private SpoonObserver<TState>[] m_Observers = Array.Empty<SpoonObserver<TState>>();
        private bool m_Dispatching;

        public TState State { get; private set; }

        public Store(IReducer<TState> reducer, MiddlewareCollection<TState> middleware) {
            m_Reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
            State = m_Reducer.InitialState;
            m_DispatchEntry = BuildPipeline(middleware ?? new MiddlewareCollection<TState>());
        }

        public void Dispatch(IAction action) {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (m_Dispatching)
                throw new InvalidOperationException(
                    "Re-entrant Dispatch detected. Reducers and middleware must not call Dispatch. " +
                    "Schedule follow-up actions via your own async method instead.");

            m_Dispatching = true;
            try { m_DispatchEntry(action); }
            finally { m_Dispatching = false; }
        }

        public void Restore(TState state) {
            if (m_Dispatching)
                throw new InvalidOperationException(
                    "Restore called during an active Dispatch. Move the Restore call outside the dispatch cycle.");

            State = state;
            NotifyObservers();
        }

        public IDisposable Subscribe(SpoonObserver<TState> observer) {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            var oldArray = m_Observers;
            var newArray = new SpoonObserver<TState>[oldArray.Length + 1];
            Array.Copy(oldArray, newArray, oldArray.Length);
            newArray[oldArray.Length] = observer;
            m_Observers = newArray;

            return new Subscription(this, observer);
        }

        private void Unsubscribe(SpoonObserver<TState> observer) {
            var oldArray = m_Observers;
            var idx = Array.IndexOf(oldArray, observer);
            if (idx < 0) return;

            if (oldArray.Length == 1) {
                m_Observers = Array.Empty<SpoonObserver<TState>>();
                return;
            }

            var newArray = new SpoonObserver<TState>[oldArray.Length - 1];
            Array.Copy(oldArray, 0, newArray, 0, idx);
            Array.Copy(oldArray, idx + 1, newArray, idx, oldArray.Length - idx - 1);
            m_Observers = newArray;
        }

        private SpoonDispatch BuildPipeline(MiddlewareCollection<TState> collection) {
            SpoonDispatch terminal = action => {
                State = m_Reducer.Reduce(State, action);
                NotifyObservers();
            };

            var middleware = collection.Middleware;
            var next = terminal;
            for (int i = middleware.Count - 1; i >= 0; i--) {
                var mw = middleware[i];
                var downstream = next;
                next = action => mw.Invoke(this, action, downstream);
            }
            return next;
        }

        private void NotifyObservers() {
            var snapshot = m_Observers;
            var state = State;
            for (int i = 0; i < snapshot.Length; i++) snapshot[i](in state);
        }

        private sealed class Subscription : IDisposable {
            private Store<TState> m_Store;
            private SpoonObserver<TState> m_Observer;

            public Subscription(Store<TState> store, SpoonObserver<TState> observer) {
                m_Store = store;
                m_Observer = observer;
            }

            public void Dispose() {
                if (m_Observer == null) return;
                m_Store.Unsubscribe(m_Observer);
                m_Store = null;
                m_Observer = null;
            }
        }
    }
}
