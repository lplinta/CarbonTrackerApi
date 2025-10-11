using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Services;

public class MedicaoEnergiaService(
    IMedicaoEnergiaRepository medicaoEnergiaRepository,
    IRepository<MedidorEnergia> medidorEnergiaRepository)
    : IMedicaoEnergiaService
{
    public async Task<MedicaoEnergia?> AdicionarMedicao(MedicaoEnergiaInput medicaoInput)
    {
        var medidor = await medidorEnergiaRepository.GetByIdAsync(medicaoInput.MedidorEnergiaId);
        if (medidor == null)
            throw new InvalidOperationException($"Medidor de energia com ID {medicaoInput.MedidorEnergiaId} não encontrado.");

        var medicao = new MedicaoEnergia
        {
            MedidorEnergiaId = medicaoInput.MedidorEnergiaId,
            ConsumoValor = medicaoInput.ConsumoValor,
            UnidadeMedida = medicaoInput.UnidadeMedida,
            Timestamp = medicaoInput.Timestamp
        };

        await medicaoEnergiaRepository.AddAsync(medicao);
        await medicaoEnergiaRepository.SaveChangesAsync();

        return medicao;
    }
}