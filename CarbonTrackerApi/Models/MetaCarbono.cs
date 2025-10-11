using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.Models;

public class MetaCarbono
{
    public int Id { get; set; }
    public int EdificioId { get; set; }
    [Required]
    public int AnoMeta { get; set; }
    [Required]
    public decimal ReducaoPercentual { get; set; }
    [Required]
    public int AnoBase { get; set; }
    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Edificio? Edificio { get; set; }
}