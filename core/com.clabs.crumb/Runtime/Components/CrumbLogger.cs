using System;

namespace CLabs.Crumb {
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

        public void Initialize(Type type) {
            m_Type = type;
            m_Registration = m_Registry.Register(type, this);
        }

        public void Verbose(string message) {
            if (!m_Enabled || !m_Filters.HasFlag(CrumbFilters.Verbose)) return;
            Write("VRB", message);
        }

        public void Info(string message) {
            if (!m_Enabled || !m_Filters.HasFlag(CrumbFilters.Info)) return;
            Write("INF", message);
        }

        public void Warn(string message) {
            if (!m_Enabled || !m_Filters.HasFlag(CrumbFilters.Warning)) return;
            Write("WRN", message);
        }

        public void Error(string message) {
            if (!m_Enabled || !m_Filters.HasFlag(CrumbFilters.Error)) return;
            Write("ERR", message);
        }

        public void Fatal(string message, Exception exception) {
            if (!m_Enabled || !m_Filters.HasFlag(CrumbFilters.Fatal)) return;
            Write("FTL", $"{message}{Environment.NewLine}{exception.StackTrace}");
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
