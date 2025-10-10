using AdsetManagement.Domain.Entities;

namespace AdsetManagement.Domain.Interfaces;

public interface IVehicleImageRepository
{
    Task<bool> VehicleExistsAsync(int vehicleId);
    Task<int> GetImageCountAsync(int vehicleId);
    Task<VehicleImage> AddImageAsync(VehicleImage vehicleImage);
    Task<List<VehicleImage>> GetImagesByVehicleIdAsync(int vehicleId);
    Task<VehicleImage?> GetImageByIdAsync(int vehicleId, int imageId);
    Task<bool> DeleteImageAsync(VehicleImage vehicleImage);
    Task SaveChangesAsync();
}