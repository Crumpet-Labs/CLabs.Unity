using Buttr.Core;

namespace CLabs.Crumb {
    public static class CrumbPackage {
        public static IConfigurableCollection UseCrumbPackage(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<ICrumbConfiguration>().WithFactory(() => new CrumbConfiguration()))
                .Register(builder.Resolvers.AddSingleton<ICrumbSink>().WithFactory(() => new ConsoleCrumbSink()))
                .Register(builder.Resolvers.AddSingleton<CrumbFileSink>())
                .Register(builder.Resolvers.AddSingleton<CrumbRegistry>())
                .Register(builder.Resolvers.AddTransient<CrumbLogger>());
        }
    }
}
