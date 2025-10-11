using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Repositories;

public interface IEdificioRepository : IRepository<Edificio>
{
    Task<Edificio?> GetEdificioWithMedicoesAsync(int edificioId, DateTimeOffset startDate, DateTimeOffset endDate);
    Task<List<MetaCarbono>> GetMetasCarbonoByEdificioIdAsync(int edificioId);
}
