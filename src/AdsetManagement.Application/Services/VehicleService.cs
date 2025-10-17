using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Application.Interfaces;
using AdsetManagement.Application.Mappings;
using AdsetManagement.Domain.Entities;
using AdsetManagement.Domain.Interfaces;
using AdsetManagement.Domain.ValueObjects;

namespace AdsetManagement.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleImageRepository _vehicleImageRepository;
    private readonly IFileService _fileService;

    public VehicleService(
        IVehicleRepository vehicleRepository, 
        IVehicleImageRepository vehicleImageRepository,
        IFileService fileService)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleImageRepository = vehicleImageRepository;
        _fileService = fileService;
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request)
    {
        var vehicle = VehicleMapper.Map(request);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);
        return VehicleMapper.Map(createdVehicle);
    }

    public async Task<VehicleResponse> CreateVehicleWithImagesAsync(CreateVehicleWithImagesRequest request)
    {
        
        var otherOptions = new OtherOptionsRequest
        {
            ArCondicionado = request.ArCondicionado,
            Alarme = request.Alarme,
            Airbag = request.Airbag,
            ABS = request.ABS
        };

        
        var createRequest = new CreateVehicleRequest
        {
            Marca = request.Marca,
            Modelo = request.Modelo,
            Ano = request.Ano,
            Placa = request.Placa,
            Km = request.Km,
            Cor = request.Cor,
            Preco = request.Preco,
            OtherOptions = otherOptions,
            PacoteICarros = request.PacoteICarros,
            PacoteWebMotors = request.PacoteWebMotors,
            Imagens = new List<string?>()
        };

        var vehicle = VehicleMapper.Map(createRequest);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);

        
        if (request.Images != null && request.Images.Count > 0)
        {
            foreach (var image in request.Images)
            {
                if (image.Length > 0)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var imageUrl = await _fileService.SaveFileAsync(image.FileName, stream, createdVehicle.Id);
                        var vehicleImage = new VehicleImage
                        {
                            VehicleId = createdVehicle.Id,
                            ImageUrl = imageUrl
                        };
                        await _vehicleImageRepository.AddImageAsync(vehicleImage);
                    }
                }
            }
        }

        
        var vehicleWithImages = await GetVehicleByIdAsync(createdVehicle.Id);
        return vehicleWithImages!;
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

        
        foreach (var vehicle in pagedVehicles)
        {
            await SyncVehicleImagesAsync(vehicle);
        }

        return VehicleMapper.Map(pagedVehicles, totalItems, filter.Page, filter.PageSize);
    }

    public async Task<VehicleResponse?> UpdateVehicleAsync(int id, UpdateVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;

        VehicleMapper.Map(vehicle, request);
        var updatedVehicle = await _vehicleRepository.UpdateAsync(vehicle);
        
        if (updatedVehicle != null)
        {
            await SyncVehicleImagesAsync(updatedVehicle);
            return VehicleMapper.Map(updatedVehicle);
        }
        
        return null;
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
        
        if (updatedVehicle != null)
        {
            await SyncVehicleImagesAsync(updatedVehicle);
            return VehicleMapper.Map(updatedVehicle);
        }
        
        return null;
    }
}