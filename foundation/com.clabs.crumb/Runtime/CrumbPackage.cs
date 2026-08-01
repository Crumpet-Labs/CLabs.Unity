using Buttr.Core;

namespace CLabs.Crumb {
    public static class CrumbPackage {
        /// <summary>Register Crumb: the config + console-sink ports (swap the concrete via <c>WithImplementation</c>), the file sink, the type-keyed registry, and a transient per-type <see cref="CrumbLogger"/>.</summary>
        public static IConfigurableCollection UseCrumbPackage(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<ICrumbConfiguration, CrumbConfiguration>().WithFactory(() => new CrumbConfiguration()))
                .Register(builder.Resolvers.AddSingleton<ICrumbSink, ConsoleCrumbSink>())
                .Register(builder.Resolvers.AddSingleton<CrumbFileSink>())
                .Register(builder.Resolvers.AddSingleton<CrumbRegistry>())
                .Register(builder.Resolvers.AddTransient<CrumbLogger>());
        }

        /// <inheritdoc cref="UseCrumbPackage(ApplicationBuilder)"/>
        public static IConfigurableCollection UseCrumbPackage(this IDIBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<ICrumbConfiguration, CrumbConfiguration>().WithFactory(() => new CrumbConfiguration()))
                .Register(builder.Resolvers.AddSingleton<ICrumbSink, ConsoleCrumbSink>())
                .Register(builder.Resolvers.AddSingleton<CrumbFileSink>())
                .Register(builder.Resolvers.AddSingleton<CrumbRegistry>())
                .Register(builder.Resolvers.AddTransient<CrumbLogger>());
        }
    }
}
