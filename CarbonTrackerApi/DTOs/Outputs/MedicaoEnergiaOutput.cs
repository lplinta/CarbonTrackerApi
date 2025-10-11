namespace CarbonTrackerApi.DTOs.Outputs;

public record MedicaoEnergiaOutput
(
    int Id,
    decimal ConsumoValor,
    string UnidadeMedida,
    DateTimeOffset Timestamp,
    int MedidorEnergiaId
);