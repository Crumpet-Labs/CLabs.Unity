namespace CLabs.Belfry {
    public delegate void BellMessage<T>(in T message) where T : struct;
}
