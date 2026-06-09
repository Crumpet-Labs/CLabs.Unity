using System;
using System.IO;
using System.Linq;

namespace CLabs.Crumb {
    public sealed class CrumbFileSink : ICrumbSink, IDisposable {
        private readonly ICrumbConfiguration m_Configuration;
        private readonly object m_Lock = new();

        private StreamWriter m_Writer;
        private string m_CurrentFilePath;
        private long m_CurrentFileSize;

        public CrumbFileSink(ICrumbConfiguration configuration) {
            m_Configuration = configuration;

            if (configuration.FileLoggingEnabled)
                InitializeWriter();
        }

        public void Write(string level, string typeName, string message) {
            if (!m_Configuration.FileLoggingEnabled) return;

            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{typeName}] {message}";

            lock (m_Lock) {
                if (m_Writer == null) InitializeWriter();

                m_Writer.WriteLine(line);
                m_Writer.Flush();
                m_CurrentFileSize += line.Length + Environment.NewLine.Length;

                if (m_CurrentFileSize >= m_Configuration.MaxFileSizeBytes)
                    Rotate();
            }
        }

        private void InitializeWriter() {
            var directory = m_Configuration.LogDirectory;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            m_CurrentFilePath = Path.Combine(directory, "current.log");
            var append = File.Exists(m_CurrentFilePath);
            var stream = new FileStream(m_CurrentFilePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            m_Writer = new StreamWriter(stream) { AutoFlush = false };
            m_CurrentFileSize = append ? new FileInfo(m_CurrentFilePath).Length : 0;
        }

        private void Rotate() {
            m_Writer?.Dispose();
            m_Writer = null;

            var directory = m_Configuration.LogDirectory;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var rotatedPath = Path.Combine(directory, $"log_{timestamp}.log");
            var attempt = 2;
            while (File.Exists(rotatedPath))
                rotatedPath = Path.Combine(directory, $"log_{timestamp}_{attempt++}.log");
            File.Move(m_CurrentFilePath, rotatedPath);

            PruneOldFiles(directory);
            InitializeWriter();
        }

        private void PruneOldFiles(string directory) {
            var logFiles = Directory.GetFiles(directory, "log_*.log")
                .OrderByDescending(f => f)
                .Skip(m_Configuration.MaxFileCount)
                .ToArray();

            foreach (var file in logFiles) {
                try { File.Delete(file); }
                catch { /* silently skip files that can't be deleted */ }
            }
        }

        public void Dispose() {
            lock (m_Lock) {
                m_Writer?.Flush();
                m_Writer?.Dispose();
                m_Writer = null;
            }
        }
    }
}
