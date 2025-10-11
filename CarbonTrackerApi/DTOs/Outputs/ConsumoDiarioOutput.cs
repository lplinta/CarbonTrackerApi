namespace CarbonTrackerApi.DTOs.Outputs;

public record ConsumoDiarioOutput
(
    DateTime Data,
    decimal ConsumoTotalKWh,
    decimal EmissaoCo2Equivalente
);