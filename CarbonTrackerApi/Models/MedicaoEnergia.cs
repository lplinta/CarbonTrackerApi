using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.Models;

public class MedicaoEnergia
{
    public int Id { get; set; }
    [Required]
    public decimal ConsumoValor { get; set; }
    [Required]
    [MaxLength(20)]
    public string UnidadeMedida { get; set; } = string.Empty;
    [Required]
    public DateTimeOffset Timestamp { get; set; }

    public int MedidorEnergiaId { get; set; }
    public MedidorEnergia? MedidorEnergia { get; set; }
}