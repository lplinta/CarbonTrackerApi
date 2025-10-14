using System.Net;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CarbonTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class MedicaoEnergiaController(
    IMedicaoEnergiaService medicaoEnergiaService,
    ILogger<MedicaoEnergiaController> logger)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(MedicaoEnergiaOutput), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> PostMedicaoEnergia([FromBody] MedicaoEnergiaInput medicaoInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var novaMedicao = await medicaoEnergiaService.AdicionarMedicao(medicaoInput);
            if (novaMedicao == null)
                return NotFound(novaMedicao);

            var medicaoEnergiaOutput = new MedicaoEnergiaOutput
            (
                novaMedicao.Id,
                novaMedicao.ConsumoValor,
                novaMedicao.UnidadeMedida,
                novaMedicao.Timestamp,
                novaMedicao.MedidorEnergiaId
            );

            return StatusCode((int)HttpStatusCode.Created, medicaoEnergiaOutput);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Falha ao adicionar medição: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao adicionar medição: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro interno ao adicionar medição de energia.");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new { message = "Ocorreu um erro interno ao adicionar medição." });
        }
    }
}