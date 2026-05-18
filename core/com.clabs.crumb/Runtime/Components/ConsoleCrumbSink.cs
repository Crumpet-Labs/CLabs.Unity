namespace CLabs.Crumb {
    public sealed class ConsoleCrumbSink : ICrumbSink {
        public void Write(string level, string typeName, string message)
            => System.Console.WriteLine($"[{level}] [{typeName}] {message}");
    }
}
