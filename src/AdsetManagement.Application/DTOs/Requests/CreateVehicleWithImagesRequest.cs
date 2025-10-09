using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AdsetManagement.Application.DTOs.Requests;

public class CreateVehicleWithImagesRequest
{
    [Required(ErrorMessage = "Marca é obrigatória")]
    [StringLength(100, ErrorMessage = "Marca deve ter no máximo 100 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "Modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ano é obrigatório")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Ano deve conter 4 dígitos")]
    [Range(1900, 2025, ErrorMessage = "Ano deve estar entre 1900 e 2025")]
    public string Ano { get; set; } = string.Empty;

    [Required(ErrorMessage = "Placa é obrigatória")]
    [RegularExpression(@"^[A-Z]{3}-\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$", ErrorMessage = "Placa deve estar no formato ABC-1234 ou ABC1D23 (Padrão Mercosul)")]
    public string Placa { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser maior ou igual a zero")]
    public int? Km { get; set; }

    [Required(ErrorMessage = "Cor é obrigatória")]
    [StringLength(50, ErrorMessage = "Cor deve ter no máximo 50 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal Preco { get; set; }

    public bool? ArCondicionado { get; set; }
    public bool? Alarme { get; set; }
    public bool? Airbag { get; set; }
    public bool? ABS { get; set; }

    [RegularExpression(@"^(Bronze|Diamante|Platinum|Básico)$", ErrorMessage = "Pacote iCarros deve ser: Bronze, Diamante, Platinum ou Básico")]
    public string? PacoteICarros { get; set; }

    [RegularExpression(@"^(Bronze|Diamante|Platinum|Básico)$", ErrorMessage = "Pacote WebMotors deve ser: Bronze, Diamante, Platinum ou Básico")]
    public string? PacoteWebMotors { get; set; }


    public List<IFormFile>? Images { get; set; } = new();
}