namespace CLabs.Belfry {
    public interface IPealConfig {
        bool IsCritical(int priority);
        IRingOrder Strategy { get; }
    }
}
