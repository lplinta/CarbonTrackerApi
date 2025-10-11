using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;

namespace CarbonTrackerApi.Interfaces.Services;

public interface IMedidorEnergiaService
{
    Task<MedidorEnergiaOutput?> GetMedidorById(int id);
    Task<PaginatedOutput<MedidorEnergiaOutput>> GetAllMedidores(PaginationInput paginationInput);
    Task<MedidorEnergiaOutput> AddMedidor(MedidorEnergiaInput medidorInput);
    Task<MedidorEnergiaOutput?> UpdateMedidor(int id, MedidorEnergiaInput medidorInput);
    Task<bool> DeleteMedidor(int id);
}