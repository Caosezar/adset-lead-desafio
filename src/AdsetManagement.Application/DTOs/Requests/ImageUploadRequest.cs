using Microsoft.AspNetCore.Http;

namespace AdsetManagement.Application.DTOs.Requests;

public class ImageUploadRequest
{
    public List<IFormFile> Images { get; set; } = new();
}