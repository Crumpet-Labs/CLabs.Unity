namespace CLabs.Crumb {
    public interface ICrumbSink {
        void Write(string level, string typeName, string message);
    }
}
