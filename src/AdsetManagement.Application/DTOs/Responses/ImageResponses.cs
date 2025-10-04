namespace AdsetManagement.Application.DTOs.Responses;

public class ImageResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
}

public class ImageUploadResponse
{
    public int VehicleId { get; set; }
    public int UploadedCount { get; set; }
    public List<string> Messages { get; set; } = new();
}