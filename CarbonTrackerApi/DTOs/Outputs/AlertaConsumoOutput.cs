namespace CarbonTrackerApi.DTOs.Outputs;

public record AlertaConsumoOutput
(
    int Id,
    string TipoAlerta,
    decimal ValorRegistrado,
    decimal LimiteEsperado,
    DateTime DataOcorrencia,
    string Mensagem,
    int EdificioId
);