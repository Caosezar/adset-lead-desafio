using Microsoft.AspNetCore.Mvc;
using AdsetManagement.API.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;

namespace AdsetManagement.API.Controllers;

[ApiController]
[Route("api/vehicle/{vehicleId}/images")]
public class VehicleImageController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int MaxImagesPerVehicle = 15;

    public VehicleImageController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost]
    public async Task<ActionResult<ImageUploadResponse>> UploadImages(int vehicleId, [FromForm] ImageUploadRequest request)
    {
        try
        {
            if (vehicleId <= 0)
                return BadRequest("ID do veículo inválido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Images.Count > MaxImagesPerVehicle)
                return BadRequest($"Máximo de {MaxImagesPerVehicle} imagens permitidas");

            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "vehicles", vehicleId.ToString());

            var existingImagesCount = 0;
            if (Directory.Exists(uploadPath))
            {
                existingImagesCount = Directory.GetFiles(uploadPath)
                    .Where(f => _allowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .Count();
            }

            if (existingImagesCount + request.Images.Count > MaxImagesPerVehicle)
                return BadRequest($"O veículo já possui {existingImagesCount} imagem(ns). Máximo permitido: {MaxImagesPerVehicle}");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var uploadedImages = new List<string>();

            foreach (var image in request.Images)
            {
                if (image.Length == 0)
                    return BadRequest($"Arquivo {image.FileName} está vazio");

                if (image.Length > MaxFileSize)
                    return BadRequest($"Arquivo {image.FileName} excede o tamanho máximo de 5MB");

                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return BadRequest($"Tipo de arquivo não permitido: {extension}. Tipos aceitos: {string.Join(", ", _allowedExtensions)}");

                if (!_allowedMimeTypes.Contains(image.ContentType.ToLowerInvariant()))
                    return BadRequest($"Tipo MIME não permitido: {image.ContentType}");

                var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/vehicles/{vehicleId}/{fileName}";
                uploadedImages.Add(imageUrl);
            }

            return Ok(new ImageUploadResponse
            {
                VehicleId = vehicleId,
                UploadedImages = uploadedImages,
                TotalImages = uploadedImages.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpDelete("{imageName}")]
    public ActionResult RemoveImage(int vehicleId, string imageName)
    {
        try
        {
            if (vehicleId <= 0)
                return BadRequest("ID do veículo inválido");

            if (string.IsNullOrWhiteSpace(imageName))
                return BadRequest("Nome da imagem é obrigatório");

            if (imageName.Contains("..") || imageName.Contains("/") || imageName.Contains("\\"))
                return BadRequest("Nome de arquivo inválido");

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "vehicles", vehicleId.ToString(), imageName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Imagem não encontrada");

            System.IO.File.Delete(filePath);
            
            return Ok(new { 
                Message = "Imagem removida com sucesso",
                VehicleId = vehicleId,
                RemovedFile = imageName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<string>> GetImages(int vehicleId)
    {
        try
        {
            if (vehicleId <= 0)
                return BadRequest("ID do veículo inválido");

            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "vehicles", vehicleId.ToString());

            if (!Directory.Exists(uploadPath))
                return Ok(new List<string>());

            var images = Directory.GetFiles(uploadPath)
                .Where(f => _allowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(file => $"{Request.Scheme}://{Request.Host}/uploads/vehicles/{vehicleId}/{Path.GetFileName(file)}")
                .OrderBy(f => f)
                .ToList();

            return Ok(images);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}