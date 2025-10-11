using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Services;

public interface IFatorEmissaoService
{
    Task<FatorEmissao?> GetFatorEmissaoByTipoMedidorAndDateAsync(string tipoMedidor, DateTime data, string? regiao = null);

    Task<FatorEmissaoOutput?> GetFatorEmissaoById(int id);
    Task<PaginatedOutput<FatorEmissaoOutput>> GetAllFatoresEmissao(PaginationInput paginationInput);
    Task<FatorEmissaoOutput> AddFatorEmissao(FatorEmissaoInput fatorEmissaoInput);
    Task<FatorEmissaoOutput?> UpdateFatorEmissao(int id, FatorEmissaoInput fatorEmissaoInput);
    Task<bool> DeleteFatorEmissao(int id);
}