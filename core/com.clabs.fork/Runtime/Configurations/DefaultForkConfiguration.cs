namespace CLabs.Fork {
    /// <summary>
    /// Plain-C# default <see cref="IForkConfiguration"/>. Used when no adapter or consumer
    /// override is registered. Resolves <c>RootPath</c> to a relative <c>"Saves"</c> directory
    /// and stamps new saves with schema version 1.
    /// </summary>
    public sealed class DefaultForkConfiguration : IForkConfiguration {
        public DefaultForkConfiguration(string rootPath = "Saves", int currentSchemaVersion = 1) {
            RootPath = rootPath;
            CurrentSchemaVersion = currentSchemaVersion;
        }

        public string RootPath { get; }
        public int CurrentSchemaVersion { get; }
    }
}
