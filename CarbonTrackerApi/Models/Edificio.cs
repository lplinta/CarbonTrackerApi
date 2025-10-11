using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.Models;

public class Edificio
{
    public int Id { get; set; }
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

    public ICollection<MedidorEnergia>? Medidores { get; set; }
    public ICollection<MetaCarbono>? MetasCarbono { get; set; }
}