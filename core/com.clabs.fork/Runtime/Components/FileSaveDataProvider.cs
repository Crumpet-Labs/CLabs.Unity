using System.IO;
using CLabs.Tickets;

namespace CLabs.Fork {
    public sealed class FileSaveDataProvider : ISaveDataProvider {
        private readonly string m_RootPath;

        public FileSaveDataProvider(IForkConfiguration configuration) {
            m_RootPath = configuration.RootPath;
        }

        public string RootPath => m_RootPath;

        public async Ticket<bool> WriteAsync(string relativePath, byte[] data) {
            var fullPath = GetFullPath(relativePath);
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(fullPath, data);
            return true;
        }

        public async Ticket<byte[]> ReadAsync(string relativePath) {
            var fullPath = GetFullPath(relativePath);

            if (!File.Exists(fullPath)) return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public Ticket<bool> DeleteAsync(string relativePath) {
            var fullPath = GetFullPath(relativePath);

            if (File.Exists(fullPath)) {
                File.Delete(fullPath);
            }

            return Ticket.FromResult(true);
        }

        public Ticket<bool> ExistsAsync(string relativePath) {
            var fullPath = GetFullPath(relativePath);
            return Ticket.FromResult(File.Exists(fullPath));
        }

        private string GetFullPath(string relativePath) {
            return Path.Combine(m_RootPath, relativePath);
        }
    }
}
