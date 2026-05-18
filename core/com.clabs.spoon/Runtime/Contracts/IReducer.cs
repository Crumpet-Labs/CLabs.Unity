namespace CLabs.Spoon {
    public interface IReducer<TState> where TState : struct {
        TState InitialState { get; }
        TState Reduce(TState state, IAction action);
    }
}
