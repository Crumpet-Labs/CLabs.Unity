using System;

namespace CLabs.Spoon {
    public interface IStore<TState> where TState : struct {
        TState State { get; }
        void Dispatch(IAction action);
        IDisposable Subscribe(SpoonObserver<TState> observer);
        void Restore(TState state);
    }
}