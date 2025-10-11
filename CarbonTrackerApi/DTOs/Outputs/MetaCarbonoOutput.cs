namespace CarbonTrackerApi.DTOs.Outputs;

public record MetaCarbonoOutput
(
    int Id,
    int EdificioId,
    int AnoMeta,
    decimal ReducaoPercentual,
    int AnoBase,
    DateTime DataCriacao
);