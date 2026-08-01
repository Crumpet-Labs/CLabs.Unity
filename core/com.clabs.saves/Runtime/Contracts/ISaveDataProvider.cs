using CLabs.Tickets;

namespace CLabs.Saves {
    public interface ISaveDataProvider {
        string RootPath { get; }
        Ticket<bool> WriteAsync(string relativePath, byte[] data);
        Ticket<byte[]> ReadAsync(string relativePath);
        Ticket<bool> DeleteAsync(string relativePath);
        Ticket<bool> ExistsAsync(string relativePath);
    }
}
