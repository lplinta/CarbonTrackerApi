using CarbonTrackerApi.Data;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbonTrackerApi.Repositories;

public class EdificioRepository(ApplicationDbContext context)
    : Repository<Edificio>(context), IEdificioRepository
{
    public async Task<Edificio?> GetEdificioWithMedicoesAsync(int edificioId, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await DbSet
            .Include(e => e.Medidores)!
            .ThenInclude(m => m.Medicoes!.Where(me => me.Timestamp >= startDate && me.Timestamp <= endDate))
            .SingleOrDefaultAsync(e => e.Id == edificioId);
    }

    public async Task<List<MetaCarbono>> GetMetasCarbonoByEdificioIdAsync(int edificioId)
    {
        return await Context.Set<MetaCarbono>()
            .Where(m => m.EdificioId == edificioId)
            .ToListAsync();
    }
}