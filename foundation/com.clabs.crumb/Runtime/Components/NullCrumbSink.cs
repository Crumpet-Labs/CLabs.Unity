namespace CLabs.Crumb {
    /// <summary>A no-op <see cref="ICrumbSink"/>. Register it in place of the default to silence console output.</summary>
    public sealed class NullCrumbSink : ICrumbSink {
        public void Write(string level, string typeName, string message) { }
    }
}
