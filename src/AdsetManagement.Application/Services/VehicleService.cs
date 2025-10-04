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
    private readonly IVehicleImageRepository _vehicleImageRepository;
    private readonly IVehicleImageService _vehicleImageService;

    public VehicleService(IVehicleRepository vehicleRepository, IVehicleImageRepository vehicleImageRepository, IVehicleImageService vehicleImageService)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleImageRepository = vehicleImageRepository;
        _vehicleImageService = vehicleImageService;
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request)
    {
        var vehicle = VehicleMapper.Map(request);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);
        return VehicleMapper.Map(createdVehicle);
    }

    public async Task<VehicleResponse> CreateVehicleWithImagesAsync(CreateVehicleWithImagesRequest request)
    {
        // Primeiro, criar o veículo sem imagens
        var createRequest = new CreateVehicleRequest
        {
            Marca = request.Marca,
            Modelo = request.Modelo,
            Ano = request.Ano,
            Placa = request.Placa,
            Km = request.Km,
            Cor = request.Cor,
            Preco = request.Preco,
            OtherOptions = request.OtherOptions,
            PacoteICarros = request.PacoteICarros,
            PacoteWebMotors = request.PacoteWebMotors,
            Imagens = new List<string?>() // Inicialmente vazio
        };

        var vehicle = VehicleMapper.Map(createRequest);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);

        // Se há imagens para fazer upload
        if (request.Images != null && request.Images.Any())
        {
            var imageUploadRequest = new ImageUploadRequest
            {
                Images = request.Images
            };

            // Usar o VehicleImageService para fazer upload das imagens
            await _vehicleImageService.UploadImagesAsync(createdVehicle.Id, imageUploadRequest);
            
            // Buscar o veículo atualizado com as imagens sincronizadas
            var updatedVehicle = await _vehicleRepository.GetByIdAsync(createdVehicle.Id);
            return VehicleMapper.Map(updatedVehicle!);
        }

        return VehicleMapper.Map(createdVehicle);
    }

    public async Task<VehicleResponse?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;
        
        await SyncVehicleImagesAsync(vehicle);
        
        return VehicleMapper.Map(vehicle);
    }

    private async Task SyncVehicleImagesAsync(Vehicle vehicle)
    {
        var vehicleImages = await _vehicleImageRepository.GetImagesByVehicleIdAsync(vehicle.Id);
        vehicle.Imagens = vehicleImages.Select(img => img.ImageUrl).ToList<string?>();
        
        await _vehicleRepository.UpdateAsync(vehicle);
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