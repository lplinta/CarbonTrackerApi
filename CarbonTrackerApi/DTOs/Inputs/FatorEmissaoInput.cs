using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class FatorEmissaoInput
{
    [Required]
    [MaxLength(50)]
    public string TipoMedidor { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string UnidadeEmissao { get; set; } = string.Empty;
    [Required]
    [Range(0.0, double.MaxValue, ErrorMessage = "O valor de emissão deve ser maior ou igual a zero.")]
    public decimal ValorEmissao { get; set; }
    [MaxLength(100)]
    public string? Regiao { get; set; }
    [Required]
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public FatorEmissaoInput() { }

    public FatorEmissaoInput(string tipoMedidor, string unidadeEmissao, decimal valorEmissao,
        string? regiao, DateTime dataInicio, DateTime? dataFim)
    {
        TipoMedidor = tipoMedidor;
        UnidadeEmissao = unidadeEmissao;
        ValorEmissao = valorEmissao;
        Regiao = regiao;
        DataInicio = dataInicio;
        DataFim = dataFim;
    }
}