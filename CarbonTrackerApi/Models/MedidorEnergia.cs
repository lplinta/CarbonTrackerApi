using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.Models;

public class MedidorEnergia
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string NumeroSerie { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string TipoMedidor { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Localizacao { get; set; } = string.Empty;

    public int EdificioId { get; set; }
    public Edificio? Edificio { get; set; }

    public ICollection<MedicaoEnergia>? Medicoes { get; set; }
}