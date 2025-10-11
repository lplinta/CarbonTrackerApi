using System.Net;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarbonTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EdificioController(IEdificioService edificioService, ILogger<EdificioController> logger)
    : ControllerBase
{
    [HttpGet("{id:int}/consumo-diario")]
    [ProducesResponseType(typeof(PaginatedOutput<ConsumoDiarioOutput>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetConsumoDiario(
        [FromRoute] int id,
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] PaginationInput paginationInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var consumoDiario = await edificioService.ObterConsumoDiario(id, dataInicio, dataFim, paginationInput);
            return Ok(consumoDiario);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Edifício não encontrado ou erro de operação ao obter consumo diário para ID {EdificioId}.", id);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao obter consumo diário para ID {EdificioId}.", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao obter consumo diário para edifício ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet("{id:int}/alertas-consumo")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaginatedOutput<AlertaConsumoOutput>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAlertasConsumo([FromRoute] int id, [FromQuery] PaginationInput paginationInput)
    {
        try
        {
            var alertas = await edificioService.ObterAlertasConsumo(id, paginationInput);
            return Ok(alertas);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Edifício não encontrado ou erro de operação ao obter alertas para ID {EdificioId}.", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao obter alertas de consumo para edifício ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpPut("{id:int}/metas-carbono")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MetaCarbonoOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> PutMetaCarbono([FromRoute] int id, [FromBody] MetaCarbonoInput metaInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var metaAtualizada = await edificioService.AtualizarMetaCarbono(id, metaInput);

            if (metaAtualizada == null)
            {
                logger.LogWarning("Não foi possível atualizar a meta para o edifício ID {EdificioId}.", id);
                return NotFound($"Edifício com ID {id} não encontrado.");
            }

            var metaCarbonoOutput = new MetaCarbonoOutput
            (
                metaAtualizada.Id,
                metaAtualizada.EdificioId,
                metaAtualizada.AnoMeta,
                metaAtualizada.ReducaoPercentual,
                metaAtualizada.AnoBase,
                metaAtualizada.DataCriacao
            );

            return Ok(metaCarbonoOutput);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao atualizar meta de carbono para edifício ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(EdificioOutput), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddEdificio([FromBody] EdificioInput edificioInput)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var newEdificio = await edificioService.AddEdificio(edificioInput);
            return StatusCode((int)HttpStatusCode.Created, newEdificio);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao adicionar edifício.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao adicionar edifício.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EdificioOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetEdificioById([FromRoute] int id)
    {
        try
        {
            var edificio = await edificioService.GetEdificioById(id);
            if (edificio == null)
            {
                logger.LogInformation("Edifício com ID {EdificioId} não encontrado.", id);
                return NotFound($"Edifício com ID {id} não encontrado.");
            }
            return Ok(edificio);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar edifício por ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedOutput<EdificioOutput>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllEdificios([FromQuery] PaginationInput paginationInput)
    {
        try
        {
            var edificios = await edificioService.GetAllEdificios(paginationInput);
            return Ok(edificios);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar todos os edifícios.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EdificioOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateEdificio([FromRoute] int id, [FromBody] EdificioInput edificioInput)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedEdificio = await edificioService.UpdateEdificio(id, edificioInput);
            if (updatedEdificio == null)
            {
                logger.LogInformation("Tentativa de atualizar edifício com ID {EdificioId} não encontrado.", id);
                return NotFound($"Edifício com ID {id} não encontrado.");
            }
            return Ok(updatedEdificio);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao atualizar edifício com ID {EdificioId}.", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao atualizar edifício com ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteEdificio([FromRoute] int id)
    {
        try
        {
            var deleted = await edificioService.DeleteEdificio(id);
            if (!deleted)
            {
                logger.LogInformation("Tentativa de deletar edifício com ID {EdificioId} não encontrado.", id);
                return NotFound($"Edifício com ID {id} não encontrado.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao deletar edifício com ID {EdificioId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }
}