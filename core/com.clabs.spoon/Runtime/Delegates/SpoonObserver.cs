namespace CLabs.Spoon {
    public delegate void SpoonObserver<TState>(in TState state) where TState : struct;
}
