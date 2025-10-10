using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;

namespace AdsetManagement.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request);
    Task<VehicleResponse> CreateVehicleWithImagesAsync(CreateVehicleWithImagesRequest request);
    Task<VehicleResponse?> GetVehicleByIdAsync(int id);
    Task<VehicleListResponse> GetVehiclesAsync(VehicleFilterRequest filter);
    Task<VehicleResponse?> UpdateVehicleAsync(int id, UpdateVehicleRequest request);
    Task<bool> DeleteVehicleAsync(int id);
    Task<VehicleResponse?> UpdateVehiclePacotesAsync(int id, UpdatePacotesRequest request);
}