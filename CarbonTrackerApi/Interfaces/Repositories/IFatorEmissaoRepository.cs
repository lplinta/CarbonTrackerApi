using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Repositories;

public interface IFatorEmissaoRepository : IRepository<FatorEmissao>
{
    Task<FatorEmissao?> GetFatorEmissaoByTipoAndDateAsync(string tipoEnergia, DateTime date, string? region = null);
}
