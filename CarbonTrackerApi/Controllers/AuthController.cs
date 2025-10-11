using System.Net;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarbonTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginOutput), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginInput loginInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var loginResponse = await authService.Authenticate(loginInput);

            if (loginResponse != null) return Ok(loginResponse);

            logger.LogWarning("Credenciais inválidas para o usuário {Username}", loginInput.Username);
            return Unauthorized("Credenciais inválidas.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro interno ao realizar o login do usuário {Username}", loginInput.Username);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro interno ao processar sua requisição.");
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterOutput), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterInput registerInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var newUser = await authService.Register(registerInput);

            if (newUser == null)
            {
                logger.LogWarning("Falha no cadastro: Usuário '{Username}' já existe.", registerInput.Username);
                return Conflict("Nome de usuário já existe.");
            }

            var userOutput = new RegisterOutput(newUser.Id, newUser.Username, newUser.Email, newUser.Role);

            return StatusCode((int)HttpStatusCode.Created, userOutput);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro interno ao realizar o cadastro do usuário {Username}", registerInput.Username);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Ocorreu um erro interno ao processar sua requisição.");
        }
    }
}