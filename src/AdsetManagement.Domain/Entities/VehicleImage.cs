namespace AdsetManagement.Domain.Entities;

public class VehicleImage
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    
    public Vehicle Vehicle { get; set; } = null!;
}