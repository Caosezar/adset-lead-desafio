using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Application.Interfaces;
using AdsetManagement.Application.Mappings;
using AdsetManagement.Domain.Entities;
using AdsetManagement.Domain.Interfaces;

namespace AdsetManagement.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request)
    {
        var vehicle = VehicleMapper.Map(request);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);
        return VehicleMapper.Map(createdVehicle);
    }

    public async Task<VehicleResponse?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle != null ? VehicleMapper.Map(vehicle) : null;
    }

    public async Task<VehicleListResponse> GetVehiclesAsync(VehicleFilterRequest filter)
    {
        var vehicles = await _vehicleRepository.FindByFiltersAsync(
            filter.Marca, filter.Modelo, filter.Ano, filter.Cor, 
            filter.PrecoMin, filter.PrecoMax, filter.KmMax);
        
        var vehiclesList = vehicles.ToList();
        var totalItems = vehiclesList.Count;
        
        var pagedVehicles = vehiclesList
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return VehicleMapper.Map(pagedVehicles, totalItems, filter.Page, filter.PageSize);
    }

    public async Task<VehicleResponse?> UpdateVehicleAsync(int id, UpdateVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;

        VehicleMapper.Map(vehicle, request);
        var updatedVehicle = await _vehicleRepository.UpdateAsync(vehicle);
        return updatedVehicle != null ? VehicleMapper.Map(updatedVehicle) : null;
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        return await _vehicleRepository.RemoveAsync(id);
    }

    public async Task<VehicleResponse?> UpdateVehiclePacotesAsync(int id, UpdatePacotesRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;

        VehicleMapper.Map(vehicle, request);
        var updatedVehicle = await _vehicleRepository.UpdateAsync(vehicle);
        return updatedVehicle != null ? VehicleMapper.Map(updatedVehicle) : null;
    }
}