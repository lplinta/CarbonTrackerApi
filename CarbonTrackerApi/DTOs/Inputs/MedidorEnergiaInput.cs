using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class MedidorEnergiaInput
{
    [Required]
    [MaxLength(100)]
    public string NumeroSerie { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string TipoMedidor { get; set; } = string.Empty;
    [Required]
    public int EdificioId { get; set; }
    [MaxLength(255)]
    public string? Localizacao { get; set; }

    public MedidorEnergiaInput() { }

    public MedidorEnergiaInput(string numeroSerie, string tipoMedidor, int edificioId,
        string? localizacao)
    {
        NumeroSerie = numeroSerie;
        TipoMedidor = tipoMedidor;
        EdificioId = edificioId;
        Localizacao = localizacao;
    }
}