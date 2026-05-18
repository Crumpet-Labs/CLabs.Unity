namespace CLabs.Bridges
{
    public readonly struct SpoonStateChangedMessage<TState> where TState : struct
    {
        public SpoonStateChangedMessage(TState state) { State = state; }
        public TState State { get; }
    }
}
