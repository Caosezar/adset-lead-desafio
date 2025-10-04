namespace AdsetManagement.Application.DTOs.Responses;

public class ImageUploadResponse
{
    public int VehicleId { get; set; }
    public List<string> UploadedImages { get; set; } = new();
    public int TotalImages { get; set; }
    public string Message { get; set; } = "Imagens enviadas com sucesso";
}

public class ImageListResponse
{
    public int VehicleId { get; set; }
    public List<string> Images { get; set; } = new();
    public int TotalImages { get; set; }
}