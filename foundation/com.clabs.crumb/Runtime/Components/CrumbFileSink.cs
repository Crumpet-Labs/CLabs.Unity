using System;
using System.IO;
using System.Linq;

namespace CLabs.Crumb {
    /// <summary>
    /// An <see cref="ICrumbSink"/> that appends timestamped log lines to a rolling <c>current.log</c>, rotating to a
    /// timestamped file once the size cap is reached and pruning the oldest rotated files. Thread-safe; file paths,
    /// size cap, and retention come from <see cref="ICrumbConfiguration"/>.
    /// </summary>
    public sealed class CrumbFileSink : ICrumbSink, IDisposable {
        private readonly ICrumbConfiguration m_Configuration;
        private readonly object m_Lock = new();

        private StreamWriter m_Writer;
        private string m_CurrentFilePath;
        private long m_CurrentFileSize;

        public CrumbFileSink(ICrumbConfiguration configuration) {
            m_Configuration = configuration;

            if (configuration.FileLoggingEnabled) {
                m_Configuration.InitializeWriter(ref m_Writer, ref m_CurrentFilePath, ref m_CurrentFileSize);
            }
        }

        public void Write(string level, string typeName, string message) {
            if (false == m_Configuration.FileLoggingEnabled) return;

            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{typeName}] {message}";

            lock (m_Lock) {
                if (m_Writer == null) {
                    m_Configuration.InitializeWriter(ref m_Writer, ref m_CurrentFilePath, ref m_CurrentFileSize);
                }

                m_Writer.WriteLine(line);
                m_Writer.Flush();
                m_CurrentFileSize += line.Length + Environment.NewLine.Length;

                if (m_CurrentFileSize >= m_Configuration.MaxFileSizeBytes) {
                    m_Configuration.Rotate(ref m_Writer, ref m_CurrentFilePath);
                    m_Configuration.InitializeWriter(ref m_Writer, ref m_CurrentFilePath, ref m_CurrentFileSize);
                }
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

    internal static class CrumbFileSinkInternals {
        internal static void InitializeWriter(this ICrumbConfiguration configuration, ref StreamWriter writer, ref string currentFilePath, ref long currentFileSize) {
            var directory = configuration.LogDirectory;

            if (false == Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            currentFilePath = Path.Combine(directory, "current.log");
            var append = File.Exists(currentFilePath);
            var stream = new FileStream(currentFilePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream) { AutoFlush = false };
            currentFileSize = append ? new FileInfo(currentFilePath).Length : 0;
        }

        internal static void Rotate(this ICrumbConfiguration configuration, ref StreamWriter writer, ref string currentFilePath) {
            writer?.Dispose();
            writer = null;

            var directory = configuration.LogDirectory;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var rotatedPath = Path.Combine(directory, $"log_{timestamp}.log");
            var attempt = 2;
            while (File.Exists(rotatedPath)) {
                rotatedPath = Path.Combine(directory, $"log_{timestamp}_{attempt++}.log");
            }

            File.Move(currentFilePath, rotatedPath);

            directory.PruneOldFiles(configuration);
        }

        internal static void PruneOldFiles(this string directory, ICrumbConfiguration configuration) {
            var logFiles = Directory.GetFiles(directory, "log_*.log")
                .OrderByDescending(f => f)
                .Skip(configuration.MaxFileCount)
                .ToArray();

            foreach (var file in logFiles) {
                try {
                    File.Delete(file);
                }
                catch {
                }
            }
        }
    }
}
