namespace AdsetManagement.API.DTOs;

public class CreateVehicleDTO
{
    public List<string?> Imagens { get; set; } = new();
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int? Km { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public OtherOptionsDTO? OtherOptions { get; set; }
}

public class UpdateVehicleDTO
{
    public List<string?> Imagens { get; set; } = new();
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int? Km { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public OtherOptionsDTO? OtherOptions { get; set; }
}

public class VehicleResponseDTO
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
    public OtherOptionsDTO? OtherOptions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class VehicleFilterDTO
{
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? Ano { get; set; }
    public string? Cor { get; set; }
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public int? KmMax { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OtherOptionsDTO
{
    public bool? ArCondicionado { get; set; }
    public bool? Alarme { get; set; }
    public bool? Airbag { get; set; }
    public bool? ABS { get; set; }
}