using Buttr.Core;
using CLabs.Belfry;
using CLabs.Spoon;

namespace CLabs.Bridges
{
    public static class SpoonBelfryPackage
    {
        public static IConfigurableCollection AddSpoonBelfryBridge<TState>(
            this ApplicationBuilder builder,
            object bellKey) where TState : struct
        {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<SpoonBelfryMediator<TState>>()
                    .WithFactory(() => new SpoonBelfryMediator<TState>(
                        Application<IBellTower>.Get(),
                        Application<IStore<TState>>.Get(),
                        bellKey)));
        }
    }
}
