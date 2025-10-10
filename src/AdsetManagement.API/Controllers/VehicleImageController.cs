using Microsoft.AspNetCore.Mvc;
using AdsetManagement.Application.DTOs.Requests;
using AdsetManagement.Application.DTOs.Responses;
using AdsetManagement.Application.Interfaces;

namespace AdsetManagement.API.Controllers;

[ApiController]
[Route("api/vehicle/{vehicleId}/images")]
public class VehicleImageController : ControllerBase
{
    private readonly IVehicleImageService _vehicleImageService;

    public VehicleImageController(IVehicleImageService vehicleImageService)
    {
        _vehicleImageService = vehicleImageService;
    }

    [HttpPost]
    public async Task<ActionResult<ImageUploadResponse>> UploadImages(int vehicleId, [FromForm] ImageUploadRequest request)
    {
        try
        {
            
            if (request?.Images != null)
            {
                foreach (var img in request.Images)
                {
                    Console.WriteLine($"[VehicleImageController] Image: {img.FileName}, Size: {img.Length}, Type: {img.ContentType}");
                }
            }
            
            if (request?.Images == null || request.Images.Count == 0)
            {
                return Ok(new ImageUploadResponse 
                { 
                    VehicleId = vehicleId, 
                    UploadedCount = 0,
                    Messages = new List<string> { "Nenhuma imagem recebida no servidor" }
                });
            }
            
            var response = await _vehicleImageService.UploadImagesAsync(vehicleId, request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ImageResponse>>> GetImages(int vehicleId)
    {
        var images = await _vehicleImageService.GetImagesAsync(vehicleId);
        return Ok(images);
    }

    [HttpDelete("{imageId}")]
    public async Task<ActionResult> DeleteImage(int vehicleId, int imageId)
    {
        var result = await _vehicleImageService.DeleteImageAsync(vehicleId, imageId);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

[ApiController]
[Route("uploads/vehicles/{vehicleId}")]
public class ImageFileController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public ImageFileController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("{fileName}")]
    public IActionResult GetImage(int vehicleId, string fileName)
    {
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var filePath = Path.Combine(webRootPath, "uploads", "vehicles", vehicleId.ToString(), fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var mimeType = fileName.ToLower() switch
        {
            var f when f.EndsWith(".jpg") || f.EndsWith(".jpeg") => "image/jpeg",
            var f when f.EndsWith(".png") => "image/png",
            var f when f.EndsWith(".webp") => "image/webp",
            _ => "application/octet-stream"
        };

        return PhysicalFile(filePath, mimeType);
    }
}