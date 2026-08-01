namespace CLabs.Crumb {
    /// <summary>A destination for formatted log lines. The console sink is overridable at registration; the file sink is always attached.</summary>
    public interface ICrumbSink {
        void Write(string level, string typeName, string message);
    }
}
