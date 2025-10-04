using System.ComponentModel.DataAnnotations;

namespace AdsetManagement.Application.DTOs.Requests;

public class UpdatePacotesRequest
{
    [RegularExpression(@"^(Bronze|Diamante|Platinum|Básico)$", ErrorMessage = "Pacote iCarros deve ser: Bronze, Diamante, Platinum ou Básico")]
    public string? PacoteICarros { get; set; }

    [RegularExpression(@"^(Bronze|Diamante|Platinum|Básico)$", ErrorMessage = "Pacote WebMotors deve ser: Bronze, Diamante, Platinum ou Básico")]
    public string? PacoteWebMotors { get; set; }
}