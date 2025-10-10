using AdsetManagement.Domain.ValueObjects;

namespace AdsetManagement.Domain.Entities;

public class Vehicle : BaseEntity
{
    public List<string?> Imagens { get; set; } = new();
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int? Km { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public OtherOptions? 
    OtherOptions { get; set; }
    public string? PacoteICarros { get; set; }
    public string? PacoteWebMotors { get; set; }
    
    public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
}