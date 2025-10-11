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
public class FatorEmissaoController(IFatorEmissaoService fatorEmissaoService, ILogger<FatorEmissaoController> logger)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(FatorEmissaoOutput), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddFatorEmissao([FromBody] FatorEmissaoInput fatorEmissaoInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var newFatorEmissao = await fatorEmissaoService.AddFatorEmissao(fatorEmissaoInput);
            return StatusCode((int)HttpStatusCode.Created, newFatorEmissao);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao adicionar fator de emissão.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao adicionar fator de emissão.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FatorEmissaoOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetFatorEmissaoById([FromRoute] int id)
    {
        try
        {
            var fatorEmissao = await fatorEmissaoService.GetFatorEmissaoById(id);
            if (fatorEmissao != null) return Ok(fatorEmissao);
            logger.LogInformation("Fator de emissão com ID {FatorEmissaoId} não encontrado.", id);
            return NotFound($"Fator de emissão com ID {id} não encontrado.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar fator de emissão por ID {FatorEmissaoId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedOutput<FatorEmissaoOutput>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllFatoresEmissao([FromQuery] PaginationInput paginationInput)
    {
        try
        {
            var fatoresEmissao = await fatorEmissaoService.GetAllFatoresEmissao(paginationInput);
            return Ok(fatoresEmissao);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao buscar todos os fatores de emissão.");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FatorEmissaoOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateFatorEmissao([FromRoute] int id, [FromBody] FatorEmissaoInput fatorEmissaoInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedFatorEmissao = await fatorEmissaoService.UpdateFatorEmissao(id, fatorEmissaoInput);
            if (updatedFatorEmissao != null) return Ok(updatedFatorEmissao);
            logger.LogInformation("Tentativa de atualizar fator de emissão com ID {FatorEmissaoId} não encontrado.", id);
            return NotFound($"Fator de emissão com ID {id} não encontrado.");
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argumentos inválidos ao atualizar fator de emissão com ID {FatorEmissaoId}.", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao atualizar fator de emissão com ID {FatorEmissaoId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteFatorEmissao([FromRoute] int id)
    {
        try
        {
            var deleted = await fatorEmissaoService.DeleteFatorEmissao(id);
            if (deleted) return NoContent();
            logger.LogInformation("Tentativa de deletar fator de emissão com ID {FatorEmissaoId} não encontrado.", id);
            return NotFound($"Fator de emissão com ID {id} não encontrado.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro interno ao deletar fator de emissão com ID {FatorEmissaoId}.", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Ocorreu um erro interno ao processar sua requisição." });
        }
    }
}