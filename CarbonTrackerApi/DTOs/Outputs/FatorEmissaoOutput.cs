namespace CarbonTrackerApi.DTOs.Outputs;

public record FatorEmissaoOutput
(
    int Id,
    string TipoMedidor,
    string UnidadeEmissao,
    decimal ValorEmissao,
    string? Regiao,
    DateTime DataInicio,
    DateTime? DataFim
);