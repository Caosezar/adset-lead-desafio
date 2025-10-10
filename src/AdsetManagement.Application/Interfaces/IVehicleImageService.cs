using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;

namespace AdsetManagement.Application.Interfaces;

public interface IVehicleImageService
{
    Task<ImageUploadResponse> UploadImagesAsync(int vehicleId, ImageUploadRequest request);
    Task<List<ImageResponse>> GetImagesAsync(int vehicleId);
    Task<bool> DeleteImageAsync(int vehicleId, int imageId);
}