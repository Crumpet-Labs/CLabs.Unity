namespace CLabs.Crumb {
    /// <summary>The default <see cref="ICrumbSink"/>: writes each log line to <see cref="System.Console"/>. The Unity adapter overrides this with a sink that routes to <c>Debug.Log</c>.</summary>
    public sealed class ConsoleCrumbSink : ICrumbSink {
        public void Write(string level, string typeName, string message) {
            System.Console.WriteLine($"[{level}] [{typeName}] {message}");
        }
    }
}
