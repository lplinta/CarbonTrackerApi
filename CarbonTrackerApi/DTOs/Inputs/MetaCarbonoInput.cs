using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class MetaCarbonoInput
{
    [Required(ErrorMessage = "O ano da meta é obrigatório.")]
    [Range(2023, 2100, ErrorMessage = "O ano da meta deve ser um valor razoável.")]
    public int AnoMeta { get; set; }

    [Required(ErrorMessage = "O percentual de redução é obrigatório.")]
    [Range(0.01, 0.99, ErrorMessage = "O percentual de redução deve estar entre 0.01 (1%) e 0.99 (99%).")]
    public decimal ReducaoPercentual { get; set; }

    [Required(ErrorMessage = "O ano base para a meta é obrigatório.")]
    [Range(2023, 2100, ErrorMessage = "O ano base deve ser um valor razoável.")]
    public int AnoBase { get; set; }

    public MetaCarbonoInput() { }

    public MetaCarbonoInput(int anoMeta, decimal reducaoPercentual, int anoBase)
    {
        AnoMeta = anoMeta;
        ReducaoPercentual = reducaoPercentual;
        AnoBase = anoBase;
    }
}