using Buttr.Core;

namespace CLabs.Spoon {
    public static class SpoonPackage {
        public static IConfigurableCollection AddSpoonStore<TState, TReducer>(this ApplicationBuilder builder, params IMiddleware<TState>[] middleware)
            where TState : struct 
            where TReducer : class, IReducer<TState> {
            
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<IReducer<TState>, TReducer>())
                .Register(builder.Resolvers.AddSingleton<MiddlewareCollection<TState>>())
                .Register(builder.Resolvers.AddSingleton<IStore<TState>, Store<TState>>())
                .WithFactory<MiddlewareCollection<TState>>(() => new MiddlewareCollection<TState>(middleware));
        }
        
        public static IConfigurableCollection AddSpoonStore<TState, TReducer>(this IDIBuilder builder, params IMiddleware<TState>[] middleware)
            where TState : struct 
            where TReducer : class, IReducer<TState> {
            
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<IReducer<TState>, TReducer>())
                .Register(builder.Resolvers.AddSingleton<MiddlewareCollection<TState>>())
                .Register(builder.Resolvers.AddSingleton<IStore<TState>, Store<TState>>())
                .WithFactory<MiddlewareCollection<TState>>(() => new MiddlewareCollection<TState>(middleware));
        }
    }
}