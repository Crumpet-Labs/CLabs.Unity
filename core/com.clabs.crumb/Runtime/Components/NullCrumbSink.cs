namespace CLabs.Crumb {
    public sealed class NullCrumbSink : ICrumbSink {
        public void Write(string level, string typeName, string message) { }
    }
}
