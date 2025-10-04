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