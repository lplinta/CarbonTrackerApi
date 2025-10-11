namespace CarbonTrackerApi.DTOs.Outputs;

public record EdificioOutput
(
    int Id,
    string Nome,
    string Cidade,
    string Endereco,
    string TipoEdificio,
    string? Latitude,
    string? Longitude
);