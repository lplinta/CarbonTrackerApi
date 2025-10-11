using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Services;

public class MedidorEnergiaService(IRepository<MedidorEnergia> medidorEnergiaRepository) : IMedidorEnergiaService
{
    public async Task<MedidorEnergiaOutput?> GetMedidorById(int id)
    {
        var medidor = await medidorEnergiaRepository.GetByIdAsync(id);
        if (medidor == null)
            return null;

        return new MedidorEnergiaOutput(
            medidor.Id,
            medidor.NumeroSerie,
            medidor.TipoMedidor,
            medidor.EdificioId,
            medidor.Localizacao
        );
    }

    public async Task<PaginatedOutput<MedidorEnergiaOutput>> GetAllMedidores(PaginationInput paginationInput)
    {
        var totalCount = await medidorEnergiaRepository.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / paginationInput.PageSize);

        var medidores = await medidorEnergiaRepository.GetAllAsync();

        var paginatedMedidores = medidores
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Take(paginationInput.PageSize)
            .Select(medidor => new MedidorEnergiaOutput(
                medidor.Id,
                medidor.NumeroSerie,
                medidor.TipoMedidor,
                medidor.EdificioId,
                medidor.Localizacao
            ))
            .ToList();

        return new PaginatedOutput<MedidorEnergiaOutput>(
            paginatedMedidores,
            totalCount,
            paginationInput.PageNumber,
            paginationInput.PageSize,
            totalPages
        );
    }

    public async Task<MedidorEnergiaOutput> AddMedidor(MedidorEnergiaInput medidorInput)
    {
        var newMedidor = new MedidorEnergia
        {
            NumeroSerie = medidorInput.NumeroSerie,
            TipoMedidor = medidorInput.TipoMedidor,
            EdificioId = medidorInput.EdificioId,
            Localizacao = medidorInput.Localizacao ?? string.Empty
        };

        await medidorEnergiaRepository.AddAsync(newMedidor);
        await medidorEnergiaRepository.SaveChangesAsync();

        return new MedidorEnergiaOutput(
            newMedidor.Id,
            newMedidor.NumeroSerie,
            newMedidor.TipoMedidor,
            newMedidor.EdificioId,
            newMedidor.Localizacao
        );
    }

    public async Task<MedidorEnergiaOutput?> UpdateMedidor(int id, MedidorEnergiaInput medidorInput)
    {
        var existingMedidor = await medidorEnergiaRepository.GetByIdAsync(id);
        if (existingMedidor == null)
            return null;

        existingMedidor.NumeroSerie = medidorInput.NumeroSerie;
        existingMedidor.TipoMedidor = medidorInput.TipoMedidor;
        existingMedidor.EdificioId = medidorInput.EdificioId;
        existingMedidor.Localizacao = medidorInput.Localizacao ?? string.Empty;

        medidorEnergiaRepository.Update(existingMedidor);
        await medidorEnergiaRepository.SaveChangesAsync();

        return new MedidorEnergiaOutput(
            existingMedidor.Id,
            existingMedidor.NumeroSerie,
            existingMedidor.TipoMedidor,
            existingMedidor.EdificioId,
            existingMedidor.Localizacao
        );
    }

    public async Task<bool> DeleteMedidor(int id)
    {
        var existingMedidor = await medidorEnergiaRepository.GetByIdAsync(id);
        if (existingMedidor == null)
            return false;

        medidorEnergiaRepository.Delete(existingMedidor);
        await medidorEnergiaRepository.SaveChangesAsync();

        return true;
    }
}