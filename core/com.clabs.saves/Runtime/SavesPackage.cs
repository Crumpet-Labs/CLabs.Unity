using Buttr.Core;
using CLabs.Crumb;

namespace CLabs.Saves {
    public static class SavesPackage {
        public static IConfigurableCollection UseSavesPackage(this ApplicationBuilder builder) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<ISavesConfiguration, DefaultSavesConfiguration>().WithFactory(() => new DefaultSavesConfiguration()))
                .Register(builder.Resolvers.AddSingleton<SaveSlotRegistry>())
                .Register(builder.Resolvers.AddSingleton<SaveMigrationPipeline>())
                .Register(builder.Resolvers.AddSingleton<ISaveDataProvider, FileSaveDataProvider>())
                .Register(builder.Resolvers.AddSingleton<ISaveSerializer, JsonSaveSerializer>())
                .Register(builder.Resolvers.AddSingleton<ISaveIntegrityValidator, Sha256IntegrityValidator>())
                .Register(builder.Resolvers.AddSingleton<ISavesService, SavesService>());
        }
    }
}
