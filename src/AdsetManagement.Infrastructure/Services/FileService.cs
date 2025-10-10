using AdsetManagement.Domain.Interfaces;

namespace AdsetManagement.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _webRootPath;

    public FileService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string> SaveFileAsync(string fileName, Stream fileStream, int vehicleId)
    {
        var uploadPath = Path.Combine(_webRootPath, "uploads", "vehicles", vehicleId.ToString());
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return $"/uploads/vehicles/{vehicleId}/{fileName}";
    }

    public async Task<bool> DeleteFileAsync(string imageUrl)
    {
        var filePath = Path.Combine(_webRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }
}