using Buttr.Core;

namespace CLabs.Spoon {
    public static class SpoonPackage {
        public static IConfigurableCollection AddSpoonStore<TState, TReducer>(
            this ApplicationBuilder builder,
            params IMiddleware<TState>[] middleware)
            where TState : struct
            where TReducer : class, IReducer<TState> {
            var col = new ConfigurableCollection();
            col.Register(builder.Resolvers.AddSingleton<IReducer<TState>, TReducer>());
            col.Register(builder.Resolvers.AddSingleton<MiddlewareCollection<TState>>()
                .WithFactory(() => new MiddlewareCollection<TState>(middleware)));
            col.Register(builder.Resolvers.AddSingleton<IStore<TState>, Store<TState>>());
            return col;
        }
    }
}