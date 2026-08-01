using System;

namespace CLabs.Crumb {
    /// <summary>
    /// A per-type logger: filters by level (<see cref="CrumbFilters"/>) and writes each surviving line to the
    /// console sink and the file sink. Resolve one via <c>Application&lt;CrumbLogger&gt;.Get()</c> and
    /// <see cref="Initialize"/> it with the owning type, which becomes the log tag and the registry key.
    /// </summary>
    public sealed class CrumbLogger : IDisposable {
        private readonly CrumbRegistry m_Registry;
        private readonly CrumbFileSink m_FileSink;
        private readonly ICrumbSink m_ConsoleSink;

        private IDisposable m_Registration;
        private bool m_Enabled;
        private CrumbFilters m_Filters;
        private Type m_Type;

        public CrumbLogger(CrumbRegistry registry, CrumbFileSink fileSink, ICrumbSink consoleSink, ICrumbConfiguration configuration) {
            m_Registry = registry;
            m_FileSink = fileSink;
            m_ConsoleSink = consoleSink;
            m_Enabled = true;
            m_Filters = configuration.DefaultFilters;
        }

        public bool Enabled {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public CrumbFilters Filters {
            get => m_Filters;
            set => m_Filters = value;
        }

        public Type Type => m_Type;

        /// <summary>Bind this logger to its owning type (the log tag) and register it so it can be resolved and toggled by type.</summary>
        public void Initialize(Type type) {
            m_Type = type;
            m_Registration = m_Registry.Register(type, this);
        }

        public void Verbose(string message) {
            if (false == m_Enabled || false == m_Filters.HasFlag(CrumbFilters.Verbose)) return;
            Write("VRB", message);
        }

        public void Info(string message) {
            if (false == m_Enabled || false == m_Filters.HasFlag(CrumbFilters.Info)) return;
            Write("INF", message);
        }

        public void Warn(string message) {
            if (false == m_Enabled || false == m_Filters.HasFlag(CrumbFilters.Warning)) return;
            Write("WRN", message);
        }

        public void Error(string message) {
            if (false == m_Enabled || false == m_Filters.HasFlag(CrumbFilters.Error)) return;
            Write("ERR", message);
        }

        /// <summary>Log a fatal error. The exception's stack trace is appended when supplied; a null exception logs the message alone.</summary>
        public void Fatal(string message, Exception exception) {
            if (false == m_Enabled || false == m_Filters.HasFlag(CrumbFilters.Fatal)) return;
            var detail = exception != null ? $"{Environment.NewLine}{exception.StackTrace}" : string.Empty;
            Write("FTL", $"{message}{detail}");
        }

        private void Write(string level, string message) {
            var typeName = m_Type != null ? m_Type.Name : "Uninitialized";
            m_ConsoleSink.Write(level, typeName, message);
            m_FileSink.Write(level, typeName, message);
        }

        public void Dispose() {
            m_Registration?.Dispose();
            m_Registration = null;
        }
    }
}
