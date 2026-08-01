namespace CLabs.Belfry {
    internal sealed class BellTower : IBellTower {
        private readonly IBelfry m_Belfry;
        private readonly IPealFactory m_PealFactory;

        public BellTower(IBelfry belfry, IPealFactory pealFactory) {
            m_Belfry = belfry;
            m_PealFactory = pealFactory;
        }

        public BellRope Rope(object key, IPealConfig pealConfig = null) {
            var peal = pealConfig != null
                ? m_PealFactory.CreatePeal(pealConfig)
                : null;
            return new BellRope(key, m_Belfry, peal);
        }
    }
}
