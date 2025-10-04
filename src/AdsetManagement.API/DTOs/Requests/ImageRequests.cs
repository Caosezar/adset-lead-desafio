using System.ComponentModel.DataAnnotations;

namespace AdsetManagement.API.DTOs.Requests;

public class ImageUploadRequest
{
    [Required(ErrorMessage = "Pelo menos uma imagem é obrigatória")]
    public List<IFormFile> Images { get; set; } = new();
}

public class ImageRemoveRequest
{
    [Required(ErrorMessage = "Nome da imagem é obrigatório")]
    public string ImageName { get; set; } = string.Empty;
}