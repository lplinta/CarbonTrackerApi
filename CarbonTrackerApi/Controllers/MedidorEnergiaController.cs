using System.Net;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarbonTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MedidorEnergiaController(
    IMedidorEnergiaService medidorEnergiaService,
    ILogger<MedidorEnergiaController> logger)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(MedidorEnergiaOutput), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddMedidor([FromBody] MedidorEnergiaInput medidorInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var newMedidor = await medidorEnergiaService.AddMedidor(medidorInput);
            return StatusCode((int)HttpStatusCode.Created, newMedidor);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao adicionar medidor.");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Operação inválida ao adicionar medidor.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao adicionar medidor.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MedidorEnergiaOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetMedidorById([FromRoute] int id)
    {
        try
        {
            var medidor = await medidorEnergiaService.GetMedidorById(id);
            if (medidor != null) return Ok(medidor);
            logger.LogInformation("Medidor com ID {MedidorId} não encontrado.", id);
            return NotFound($"Medidor com ID {id} não encontrado.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar medidor por ID {MedidorId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedOutput<MedidorEnergiaOutput>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllMedidores([FromQuery] PaginationInput paginationInput)
    {
        try
        {
            var medidores = await medidorEnergiaService.GetAllMedidores(paginationInput);
            return Ok(medidores);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar todos os medidores.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MedidorEnergiaOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateMedidor([FromRoute] int id, [FromBody] MedidorEnergiaInput medidorInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedMedidor = await medidorEnergiaService.UpdateMedidor(id, medidorInput);
            if (updatedMedidor == null)
            {
                logger.LogInformation("Tentativa de atualizar medidor com ID {MedidorId} não encontrado.", id);
                return NotFound($"Medidor com ID {id} não encontrado.");
            }
            return Ok(updatedMedidor);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao atualizar medidor com ID {MedidorId}.", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Operação inválida ao atualizar medidor com ID {MedidorId}.", id);
            return BadRequest(new { message = "Não foi possível atualizar o medidor. Verifique os dados fornecidos." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao atualizar medidor com ID {MedidorId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteMedidor([FromRoute] int id)
    {
        try
        {
            var deleted = await medidorEnergiaService.DeleteMedidor(id);
            if (!deleted)
            {
                logger.LogInformation("Tentativa de deletar medidor com ID {MedidorId} não encontrado.", id);
                return NotFound($"Medidor com ID {id} não encontrado.");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Não foi possível deletar medidor com ID {MedidorId}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao deletar medidor com ID {MedidorId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }
}