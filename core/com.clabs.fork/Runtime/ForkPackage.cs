using Buttr.Core;
using CLabs.Crumb;

namespace CLabs.Fork {
    public static class ForkPackage {
        public static IConfigurableCollection UseForkPackage(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<IForkConfiguration>().WithFactory(() => new DefaultForkConfiguration()))
                .Register(builder.Resolvers.AddSingleton<SaveSlotRegistry>())
                .Register(builder.Resolvers.AddSingleton<ISaveDataProvider, FileSaveDataProvider>())
                .Register(builder.Resolvers.AddSingleton<ISaveSerializer, JsonSaveSerializer>())
                .Register(builder.Resolvers.AddSingleton<ISaveIntegrityValidator, Sha256IntegrityValidator>())
                .Register(builder.Resolvers.AddSingleton<IForkService, ForkService>());
        }
    }
}
