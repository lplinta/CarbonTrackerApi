using System.Net.Http.Json;
using CarbonTrackerApi.BDDTests.Utils;
using CarbonTrackerApi.DTOs.Inputs;
using FluentAssertions;
using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class AuthControllerSteps(CustomWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private HttpResponseMessage? _response;

    private string? _username;
    private string? _password;

    [Given(@"que existe um usuário válido com username ""(.*)"" e senha ""(.*)""")]
    public async Task GivenQueExisteUmUsuarioValido(string username, string senha)
    {
        _username = username;
        _password = senha;

        var registerInput = new RegisterInput
        {
            Username = username,
            Password = senha,
            Email = $"{username}@teste.com",
            Role = "user"
        };

        await _client.PostAsJsonAsync("/auth/register", registerInput);
    }

    [Given(@"que não existe um usuário com username ""(.*)""")]
    public void GivenQueNaoExisteUmUsuario(string username)
    {
        _username = username;
        _password = "senhaInvalida";
    }

    [Given(@"que já existe um usuário com username ""(.*)""")]
    public async Task GivenQueJaExisteUmUsuario(string username)
    {
        _username = username;

        var registerInput = new RegisterInput
        {
            Username = username,
            Password = "senha123",
            Email = $"{username}@teste.com",
            Role = "user"
        };

        await _client.PostAsJsonAsync("/auth/register", registerInput);
    }

    [When(@"eu envio um POST para ""(.*)"" com essas credenciais")]
    public async Task WhenEnvioUmPostComCredenciais(string endpoint)
    {
        var loginInput = new LoginInput
        {
            Username = _username!,
            Password = _password!
        };

        _response = await _client.PostAsJsonAsync(endpoint, loginInput);
    }

    [When(@"eu envio um POST para ""(.*)"" com username ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEnvioUmPostComUsernameESenha(string endpoint, string username, string senha)
    {
        var loginInput = new LoginInput
        {
            Username = username,
            Password = senha
        };

        _response = await _client.PostAsJsonAsync(endpoint, loginInput);
    }

    [When(@"eu envio um POST de registro para ""(.*)"" com username ""(.*)""")]
    public async Task WhenEnvioUmPostDeRegistroComUsername(string endpoint, string username)
    {
        var registerInput = new RegisterInput
        {
            Username = username,
            Password = "senha123",
            Email = $"{username}@teste.com",
            Role = "user"
        };

        _response = await _client.PostAsJsonAsync(endpoint, registerInput);
    }

    [When(@"eu envio um POST para ""(.*)"" com os dados válidos")]
    public async Task WhenEnvioUmPostComDadosValidos(string endpoint)
    {
        var registerInput = new RegisterInput
        {
            Username = _username!,
            Password = "senha123",
            Email = $"{_username}@teste.com",
            Role = "user"
        };

        _response = await _client.PostAsJsonAsync(endpoint, registerInput);
    }

    [Then(@"a resposta deve ter status code (.*)")]
    public void ThenARespostaDeveTerStatusCode(int expectedStatus)
    {
        _response.Should().NotBeNull();
        ((int)_response!.StatusCode).Should().Be(expectedStatus);
    }

    [Then(@"o corpo deve conter um token JWT")]
    public async Task ThenCorpoDeveConterTokenJwt()
    {
        var json = await _response!.Content.ReadAsStringAsync();
        json.Should().Contain("token");
    }

    [Then(@"o corpo deve conter o username ""(.*)""")]
    public async Task ThenCorpoDeveConterUsername(string username)
    {
        var json = await _response!.Content.ReadAsStringAsync();
        json.Should().Contain(username);
    }

    [Then(@"o corpo deve conter a mensagem ""(.*)""")]
    public async Task ThenCorpoDeveConterMensagem(string mensagem)
    {
        var json = await _response!.Content.ReadAsStringAsync();
        json.Should().Contain(mensagem);
    }
}