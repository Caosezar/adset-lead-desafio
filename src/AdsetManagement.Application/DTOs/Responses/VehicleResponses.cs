namespace AdsetManagement.Application.DTOs.Responses;

public class VehicleResponse
{
    public int Id { get; set; }
    public List<string?> Imagens { get; set; } = new();
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int? Km { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public OtherOptionsResponse? OtherOptions { get; set; }
    public string? PacoteICarros { get; set; }
    public string? PacoteWebMotors { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreateUserId { get; set; }
    public int? UpdateUserId { get; set; }
}

public class VehicleListResponse
{
    public List<VehicleResponse> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class OtherOptionsResponse
{
    public bool? ArCondicionado { get; set; }
    public bool? Alarme { get; set; }
    public bool? Airbag { get; set; }
    public bool? ABS { get; set; }
}