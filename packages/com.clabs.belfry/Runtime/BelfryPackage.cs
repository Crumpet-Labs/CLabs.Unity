using Buttr.Core;

namespace CLabs.Belfry {
    public static class BelfryPackage {
        public static IConfigurableCollection UseBelfry(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<IBelfry, Belfry>())
                .Register(builder.Resolvers.AddSingleton<IPealFactory, PealFactory>())
                .Register(builder.Resolvers.AddSingleton<IBellTower, BellTower>());
        }
    }
}
