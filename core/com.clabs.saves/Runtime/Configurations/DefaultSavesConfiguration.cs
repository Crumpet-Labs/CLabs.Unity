namespace CLabs.Saves {
    /// <summary>
    /// Plain-C# default <see cref="ISavesConfiguration"/>. Used when no adapter or consumer
    /// override is registered. Resolves <c>RootPath</c> to a relative <c>"Saves"</c> directory
    /// and stamps new saves with schema version 1.
    /// </summary>
    public sealed class DefaultSavesConfiguration : ISavesConfiguration {
        private readonly string m_RootPath;
        private readonly int m_CurrentSchemaVersion;
        
        public DefaultSavesConfiguration(string rootPath = "Saves", int currentSchemaVersion = 1) {
            m_RootPath = rootPath;
            m_CurrentSchemaVersion = currentSchemaVersion;
        }

        public string RootPath => m_RootPath;
        public int CurrentSchemaVersion => m_CurrentSchemaVersion;
    }
}
