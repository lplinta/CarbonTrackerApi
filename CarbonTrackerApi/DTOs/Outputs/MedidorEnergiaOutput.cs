namespace CarbonTrackerApi.DTOs.Outputs;

public record MedidorEnergiaOutput
(
    int Id,
    string NumeroSerie,
    string TipoMedidor,
    int EdificioId,
    string? Localizacao
);