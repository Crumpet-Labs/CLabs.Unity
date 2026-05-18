using Buttr.Core;
using CLabs.Crumb;

namespace CLabs.Fork {
    public static class ForkPackage {
        public static IConfigurableCollection UseForkPackage(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<IForkConfiguration>()
                    .WithFactory(() => new DefaultForkConfiguration()))
                .Register(builder.Resolvers.AddSingleton<SaveSlotRegistry>())
                .Register(builder.Resolvers.AddSingleton<ISaveDataProvider>()
                    .WithFactory(() => new FileSaveDataProvider(Application<IForkConfiguration>.Get())))
                .Register(builder.Resolvers.AddSingleton<ISaveSerializer>()
                    .WithFactory(() => new JsonSaveSerializer(Application<IForkConfiguration>.Get())))
                .Register(builder.Resolvers.AddSingleton<ISaveIntegrityValidator>()
                    .WithFactory(() => new Sha256IntegrityValidator()))
                .Register(builder.Resolvers.AddSingleton<IForkService>()
                    .WithFactory(() => new ForkService(
                        Application<ISaveDataProvider>.Get(),
                        Application<ISaveSerializer>.Get(),
                        Application<ISaveIntegrityValidator>.Get(),
                        Application<CrumbLogger>.Get())));
        }
    }
}
