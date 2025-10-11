using CarbonTrackerApi.Data;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbonTrackerApi.Repositories;

public class MedicaoEnergiaRepository(ApplicationDbContext context)
    : Repository<MedicaoEnergia>(context), IMedicaoEnergiaRepository
{
    public async Task<List<MedicaoEnergia>> GetMedicoesByMedidorAndPeriodAsync(int medidorId,
        DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await DbSet
            .Where(m =>
                m.MedidorEnergiaId == medidorId &&
                m.Timestamp >= startDate &&
                m.Timestamp <= endDate)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }
}
