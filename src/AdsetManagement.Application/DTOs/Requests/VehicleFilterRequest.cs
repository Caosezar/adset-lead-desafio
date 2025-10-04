namespace AdsetManagement.Application.DTOs.Requests;

public class VehicleFilterRequest
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