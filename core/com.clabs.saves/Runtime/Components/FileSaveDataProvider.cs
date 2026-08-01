using System.IO;
using CLabs.Tickets;

namespace CLabs.Saves {
    public sealed class FileSaveDataProvider : ISaveDataProvider {
        private readonly string m_RootPath;

        public FileSaveDataProvider(ISavesConfiguration configuration) {
            m_RootPath = configuration.RootPath;
        }

        public string RootPath => m_RootPath;

        public async Ticket<bool> WriteAsync(string relativePath, byte[] data) {
            var fullPath = m_RootPath.GetFullPath(relativePath);
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(fullPath, data);
            return true;
        }

        public async Ticket<byte[]> ReadAsync(string relativePath) {
            var fullPath = m_RootPath.GetFullPath(relativePath);

            if (!File.Exists(fullPath)) return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public Ticket<bool> DeleteAsync(string relativePath) {
            var fullPath = m_RootPath.GetFullPath(relativePath);

            if (File.Exists(fullPath)) {
                File.Delete(fullPath);
            }

            return Ticket.FromResult(true);
        }

        public Ticket<bool> ExistsAsync(string relativePath) {
            var fullPath = m_RootPath.GetFullPath(relativePath);
            return Ticket.FromResult(File.Exists(fullPath));
        }
    }

    internal static class FileSaveDataProviderInternals {
        public static string GetFullPath(this string rootPath, string relativePath) {
            return Path.Combine(rootPath, relativePath);
        }
    }
}
