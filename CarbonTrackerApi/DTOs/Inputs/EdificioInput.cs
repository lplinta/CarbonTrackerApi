using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class EdificioInput
{
    [Required]
    [MaxLength(255)]
    public string Nome { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Endereco { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string TipoEdificio { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Latitude { get; set; }
    [MaxLength(50)]
    public string? Longitude { get; set; }

    public EdificioInput() { }

    public EdificioInput(string nome, string cidade, string endereco,
        string tipoEdificio, string? latitude, string? longitude)
    {
        Nome = nome;
        Cidade = cidade;
        Endereco = endereco;
        TipoEdificio = tipoEdificio;
        Latitude = latitude;
        Longitude = longitude;
    }
}