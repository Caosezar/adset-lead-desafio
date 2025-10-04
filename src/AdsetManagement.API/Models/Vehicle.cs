namespace AdsetManagement.API.Models;

public class Vehicle
{
    public Guid Id { get; set; }
    public List<string?> Imagens { get; set; } = new();
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int? Km { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public OtherOptions? OtherOptions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class OtherOptions
{
    public bool? ArCondicionado { get; set; }
    public bool? Alarme { get; set; }
    public bool? Airbag { get; set; }
    public bool? ABS { get; set; }
}