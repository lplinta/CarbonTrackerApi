using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Services;

public interface IMedicaoEnergiaService
{
    Task<MedicaoEnergia?> AdicionarMedicao(MedicaoEnergiaInput medicaoInput);
}
