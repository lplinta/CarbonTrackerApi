using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Services;

public interface IEdificioService
{
    Task<PaginatedOutput<ConsumoDiarioOutput>> ObterConsumoDiario(int edificioId, DateTime dataInicio,
        DateTime dataFim, PaginationInput paginationInput);
    Task<PaginatedOutput<AlertaConsumoOutput>> ObterAlertasConsumo(int edificioId, PaginationInput paginationInput);
    Task<MetaCarbono?> AtualizarMetaCarbono(int edificioId, MetaCarbonoInput metaInput);

    Task<EdificioOutput?> GetEdificioById(int id);
    Task<PaginatedOutput<EdificioOutput>> GetAllEdificios(PaginationInput paginationInput);
    Task<EdificioOutput> AddEdificio(EdificioInput edificioInput);
    Task<EdificioOutput?> UpdateEdificio(int id, EdificioInput edificioInput);
    Task<bool> DeleteEdificio(int id);
}
