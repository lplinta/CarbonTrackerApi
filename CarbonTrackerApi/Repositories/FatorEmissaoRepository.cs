using CarbonTrackerApi.Data;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbonTrackerApi.Repositories;

public class FatorEmissaoRepository(ApplicationDbContext context)
    : Repository<FatorEmissao>(context), IFatorEmissaoRepository
{
    public async Task<FatorEmissao?> GetFatorEmissaoByTipoAndDateAsync(string tipoMedidor, DateTime date, string? region = null)
    {
        var query = DbSet.Where(f =>
            f.TipoMedidor == tipoMedidor && f.DataInicio <= date && (f.DataFim == null || f.DataFim >= date));

        query = !string.IsNullOrEmpty(region)
            ? query.Where(f => f.Regiao == region)
            : query.Where(f => f.Regiao == null || f.Regiao == "");

        return await query
            .OrderByDescending(f => f.DataInicio)
            .FirstOrDefaultAsync();
    }
}