using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Repositories;

public interface IMedicaoEnergiaRepository : IRepository<MedicaoEnergia>
{
    Task<List<MedicaoEnergia>> GetMedicoesByMedidorAndPeriodAsync(int medidorId, DateTimeOffset startDate,
        DateTimeOffset endDate);
}
