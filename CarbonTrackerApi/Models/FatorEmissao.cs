using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.Models;

public class FatorEmissao
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string TipoMedidor { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string UnidadeEmissao { get; set; } = string.Empty;
    [Required]
    public decimal ValorEmissao { get; set; }
    [MaxLength(100)]
    public string? Regiao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}