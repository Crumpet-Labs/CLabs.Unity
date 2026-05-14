namespace CLabs.Belfry {
    public sealed class PealFactory : IPealFactory {
        public IPeal CreatePeal(IPealConfig config) {
            return new Peal(config);
        }
    }
}
