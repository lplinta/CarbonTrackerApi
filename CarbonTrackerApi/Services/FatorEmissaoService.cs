using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Services;

public class FatorEmissaoService(IFatorEmissaoRepository fatorEmissaoRepository) : IFatorEmissaoService
{
    public async Task<FatorEmissao?> GetFatorEmissaoByTipoMedidorAndDateAsync(string tipoMedidor, DateTime data, string? regiao = null)
    {
        var dataUtc = DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);

        var fatores = (await fatorEmissaoRepository.GetAllAsync(
            f => f.TipoMedidor == tipoMedidor &&
                 f.Regiao == regiao &&
                 f.DataInicio <= dataUtc &&
                 (f.DataFim == null || f.DataFim >= dataUtc)
        )).ToList();

        if (fatores.Count != 0)
            return fatores.OrderByDescending(f => f.DataInicio).FirstOrDefault();

        fatores = (await fatorEmissaoRepository.GetAllAsync(
            f => f.TipoMedidor == tipoMedidor &&
                 f.Regiao == null &&
                 f.DataInicio <= dataUtc &&
                 (f.DataFim == null || f.DataFim >= dataUtc)
        )).ToList();

        return fatores.OrderByDescending(f => f.DataInicio).FirstOrDefault();
    }

    public async Task<FatorEmissaoOutput?> GetFatorEmissaoById(int id)
    {
        var fatorEmissao = await fatorEmissaoRepository.GetByIdAsync(id);
        if (fatorEmissao == null)
            return null;

        return new FatorEmissaoOutput(
            fatorEmissao.Id,
            fatorEmissao.TipoMedidor,
            fatorEmissao.UnidadeEmissao,
            fatorEmissao.ValorEmissao,
            fatorEmissao.Regiao,
            fatorEmissao.DataInicio,
            fatorEmissao.DataFim
        );
    }

    public async Task<PaginatedOutput<FatorEmissaoOutput>> GetAllFatoresEmissao(PaginationInput paginationInput)
    {
        var totalCount = await fatorEmissaoRepository.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / paginationInput.PageSize);

        var fatoresEmissao = await fatorEmissaoRepository.GetAllAsync();

        var paginatedFatores = fatoresEmissao
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Take(paginationInput.PageSize)
            .Select(f => new FatorEmissaoOutput(
                f.Id,
                f.TipoMedidor,
                f.UnidadeEmissao,
                f.ValorEmissao,
                f.Regiao,
                f.DataInicio,
                f.DataFim
            ))
            .ToList();

        return new PaginatedOutput<FatorEmissaoOutput>(
            paginatedFatores,
            totalCount,
            paginationInput.PageNumber,
            paginationInput.PageSize,
            totalPages
        );
    }

    public async Task<FatorEmissaoOutput> AddFatorEmissao(FatorEmissaoInput fatorEmissaoInput)
    {
        if (fatorEmissaoInput.DataFim.HasValue && fatorEmissaoInput.DataFim.Value < fatorEmissaoInput.DataInicio)
        {
            throw new ArgumentException("A DataFim não pode ser anterior à DataInicio.");
        }

        var newFatorEmissao = new FatorEmissao
        {
            TipoMedidor = fatorEmissaoInput.TipoMedidor,
            UnidadeEmissao = fatorEmissaoInput.UnidadeEmissao,
            ValorEmissao = fatorEmissaoInput.ValorEmissao,
            Regiao = fatorEmissaoInput.Regiao,
            DataInicio = DateTime.SpecifyKind(fatorEmissaoInput.DataInicio, DateTimeKind.Utc),
            DataFim = fatorEmissaoInput.DataFim.HasValue ? DateTime.SpecifyKind(fatorEmissaoInput.DataFim.Value, DateTimeKind.Utc) : null
        };

        await fatorEmissaoRepository.AddAsync(newFatorEmissao);
        await fatorEmissaoRepository.SaveChangesAsync();

        return new FatorEmissaoOutput(
            newFatorEmissao.Id,
            newFatorEmissao.TipoMedidor,
            newFatorEmissao.UnidadeEmissao,
            newFatorEmissao.ValorEmissao,
            newFatorEmissao.Regiao,
            newFatorEmissao.DataInicio,
            newFatorEmissao.DataFim
        );
    }

    public async Task<FatorEmissaoOutput?> UpdateFatorEmissao(int id, FatorEmissaoInput fatorEmissaoInput)
    {
        var existingFatorEmissao = await fatorEmissaoRepository.GetByIdAsync(id);
        if (existingFatorEmissao == null)
        {
            return null;
        }

        if (fatorEmissaoInput.DataFim.HasValue && fatorEmissaoInput.DataFim.Value < fatorEmissaoInput.DataInicio)
        {
            throw new ArgumentException("A DataFim não pode ser anterior à DataInicio.");
        }

        existingFatorEmissao.TipoMedidor = fatorEmissaoInput.TipoMedidor;
        existingFatorEmissao.UnidadeEmissao = fatorEmissaoInput.UnidadeEmissao;
        existingFatorEmissao.ValorEmissao = fatorEmissaoInput.ValorEmissao;
        existingFatorEmissao.Regiao = fatorEmissaoInput.Regiao;
        existingFatorEmissao.DataInicio = DateTime.SpecifyKind(fatorEmissaoInput.DataInicio, DateTimeKind.Utc);
        existingFatorEmissao.DataFim = fatorEmissaoInput.DataFim.HasValue ? DateTime.SpecifyKind(fatorEmissaoInput.DataFim.Value, DateTimeKind.Utc) : null;

        fatorEmissaoRepository.Update(existingFatorEmissao);
        await fatorEmissaoRepository.SaveChangesAsync();

        return new FatorEmissaoOutput(
            existingFatorEmissao.Id,
            existingFatorEmissao.TipoMedidor,
            existingFatorEmissao.UnidadeEmissao,
            existingFatorEmissao.ValorEmissao,
            existingFatorEmissao.Regiao,
            existingFatorEmissao.DataInicio,
            existingFatorEmissao.DataFim
        );
    }

    public async Task<bool> DeleteFatorEmissao(int id)
    {
        var existingFatorEmissao = await fatorEmissaoRepository.GetByIdAsync(id);
        if (existingFatorEmissao == null)
            return false;

        fatorEmissaoRepository.Delete(existingFatorEmissao);
        await fatorEmissaoRepository.SaveChangesAsync();

        return true;
    }
}