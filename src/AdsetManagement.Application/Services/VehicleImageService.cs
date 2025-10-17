using AdsetManagement.Application.Interfaces;
using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Domain.Entities;
using AdsetManagement.Domain.Interfaces;

namespace AdsetManagement.Application.Services;

public class VehicleImageService : IVehicleImageService
{
    private readonly IVehicleImageRepository _vehicleImageRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFileService _fileService;
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int MaxImagesPerVehicle = 15;

    public VehicleImageService(IVehicleImageRepository vehicleImageRepository, IVehicleRepository vehicleRepository, IFileService fileService)
    {
        _vehicleImageRepository = vehicleImageRepository;
        _vehicleRepository = vehicleRepository;
        _fileService = fileService;
    }

    public async Task<ImageUploadResponse> UploadImagesAsync(int vehicleId, ImageUploadRequest request)
    {
        var vehicleExists = await _vehicleImageRepository.VehicleExistsAsync(vehicleId);
        if (!vehicleExists)
            throw new ArgumentException("Veículo não encontrado");

        var existingCount = await _vehicleImageRepository.GetImageCountAsync(vehicleId);
        var response = new ImageUploadResponse { VehicleId = vehicleId };
        var uploadedCount = 0;

        if (request?.Images == null || request.Images.Count == 0)
        {
            response.Messages.Add("Nenhuma imagem foi enviada");
            return response;
        }

        foreach (var image in request.Images)
        {
            if (existingCount + uploadedCount >= MaxImagesPerVehicle)
            {
                response.Messages.Add($"Máximo de {MaxImagesPerVehicle} imagens atingido");
                break;
            }

            if (image.Length > MaxFileSize)
            {
                response.Messages.Add($"{image.FileName}: Arquivo muito grande (máx 5MB)");
                continue;
            }

            if (!_allowedMimeTypes.Contains(image.ContentType?.ToLowerInvariant()))
            {
                response.Messages.Add($"{image.FileName}: Tipo não permitido (jpg, png, webp)");
                continue;
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileName = $"img_{timestamp}_{uniqueId}_{vehicleId}{Path.GetExtension(image.FileName)}";
            
            var imageUrl = await _fileService.SaveFileAsync(fileName, image.OpenReadStream(), vehicleId);

            var vehicleImage = new VehicleImage
            {
                VehicleId = vehicleId,
                FileName = image.FileName,
                ContentType = image.ContentType ?? "application/octet-stream",
                ImageUrl = imageUrl,
                UploadDate = DateTime.UtcNow,
                Order = existingCount + uploadedCount
            };

            await _vehicleImageRepository.AddImageAsync(vehicleImage);
            uploadedCount++;
        }

        await _vehicleImageRepository.SaveChangesAsync();
        await SyncVehicleImagesAsync(vehicleId);
        response.UploadedCount = uploadedCount;
        if (uploadedCount > 0)
            response.Messages.Add($"{uploadedCount} Adicionado(s) com sucesso");

        return response;
    }

    private async Task SyncVehicleImagesAsync(int vehicleId)
    {
        //TODO ordenar as imagens
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle != null)
        {
            var vehicleImages = await _vehicleImageRepository.GetImagesByVehicleIdAsync(vehicleId);
            vehicle.Imagens = vehicleImages
    .OrderBy(img => img.Order)
    .Select(img => img.ImageUrl)
    .ToList<string?>();

            await _vehicleRepository.UpdateAsync(vehicle);
        }
    }

    public async Task<List<ImageResponse>> GetImagesAsync(int vehicleId)
    {
        var images = await _vehicleImageRepository.GetImagesByVehicleIdAsync(vehicleId);

        return images.Select(i => new ImageResponse
        {
            Id = i.Id,
            FileName = i.FileName,
            ContentType = i.ContentType,
            ImageUrl = i.ImageUrl,
            UploadDate = i.UploadDate
        }).ToList();
    }

    public async Task<bool> DeleteImageAsync(int vehicleId, int imageId)
    {
        var image = await _vehicleImageRepository.GetImageByIdAsync(vehicleId, imageId);
        if (image == null)
            return false;

        var fileDeleted = await _fileService.DeleteFileAsync(image.ImageUrl);
        await _vehicleImageRepository.DeleteImageAsync(image);
        await _vehicleImageRepository.SaveChangesAsync();

        await SyncVehicleImagesAsync(vehicleId);

        return true;
    }
}