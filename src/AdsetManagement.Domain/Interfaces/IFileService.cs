namespace AdsetManagement.Domain.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(string fileName, Stream fileStream, int vehicleId);
    Task<bool> DeleteFileAsync(string imageUrl);
}