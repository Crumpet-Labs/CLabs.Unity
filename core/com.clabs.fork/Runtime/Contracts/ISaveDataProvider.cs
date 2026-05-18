using CLabs.Tickets;

namespace CLabs.Fork {
    public interface ISaveDataProvider {
        Ticket<bool> WriteAsync(string relativePath, byte[] data);
        Ticket<byte[]> ReadAsync(string relativePath);
        Ticket<bool> DeleteAsync(string relativePath);
        Ticket<bool> ExistsAsync(string relativePath);
        string RootPath { get; }
    }
}
