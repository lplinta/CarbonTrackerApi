using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class MedicaoEnergiaInput
{
    [Required(ErrorMessage = "O ID do medidor de energia é obrigatório.")]
    public int MedidorEnergiaId { get; set; }

    [Required(ErrorMessage = "O valor de consumo é obrigatório.")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "O valor de consumo deve ser maior que zero.")]
    public decimal ConsumoValor { get; set; }

    [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
    [StringLength(20, ErrorMessage = "A unidade de medida deve ter no máximo 20 caracteres.")]
    public string UnidadeMedida { get; set; } = string.Empty;

    [Required(ErrorMessage = "O timestamp da medição é obrigatório.")]
    public DateTimeOffset Timestamp { get; set; }

    public MedicaoEnergiaInput() { }

    public MedicaoEnergiaInput(int medidorEnergiaId, decimal consumoValor, string unidadeMedia,
        DateTimeOffset timestamp)
    {
        MedidorEnergiaId = medidorEnergiaId;
        ConsumoValor = consumoValor;
        UnidadeMedida = unidadeMedia;
        Timestamp = timestamp;
    }
}