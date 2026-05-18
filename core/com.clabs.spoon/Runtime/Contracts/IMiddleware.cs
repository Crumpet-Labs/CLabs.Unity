namespace CLabs.Spoon {
    public interface IMiddleware<TState> where TState : struct {
        void Invoke(IStore<TState> store, IAction action, SpoonDispatch next);
    }
}
