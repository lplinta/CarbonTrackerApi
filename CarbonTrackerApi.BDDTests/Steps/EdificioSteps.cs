using System.Net.Http.Json;
using CarbonTrackerApi.BDDTests.Utils;
using CarbonTrackerApi.DTOs.Inputs;
using FluentAssertions;
using Reqnroll;
using System.Text.Json;
using CarbonTrackerApi.Data;
using CarbonTrackerApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class EdificioSteps(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
{
    private readonly HttpClient _client = factory.CreateClient();
    private EdificioInput _edificioInput = null!;
    private EdificioInput _updatedInput = null!;

    [Given(@"que eu possuo os seguintes dados do edifício:")]
    public void GivenQueEuPossuoOsSeguintesDadosDoEdificio(Table table)
    {
        var dict = table.Rows.ToDictionary(r => r["campo"], r => r["valor"]);
        _edificioInput = new EdificioInput
        {
            Nome = dict["nome"],
            Cidade = dict["cidade"],
            Endereco = dict["endereco"],
            TipoEdificio = dict["tipoEdificio"],
        };
    }

    [Given(@"que eu possuo os seguintes novos dados do edifício:")]
    public void GivenQueEuPossuoOsSeguintesNovosDadosDoEdificio(Table table)
    {
        var dict = table.Rows.ToDictionary(r => r["campo"], r => r["valor"]);
        _updatedInput = new EdificioInput
        {
            Nome = dict["nome"],
            Cidade = dict["cidade"],
            Endereco = dict["endereco"],
            TipoEdificio = dict["tipoEdificio"],
        };
    }

    [When(@"eu enviar uma requisição POST para ""(.*)""")]
    public async Task WhenEuEnviarUmaRequisicaoPostPara(string endpoint)
    {
        var response = await _client.PostAsJsonAsync(endpoint, _edificioInput);
        scenarioContext.Set(response, "Response");
    }

    [When(@"eu enviar uma requisição GET para ""(.*)""")]
    public async Task WhenEuEnviarUmaRequisicaoGetPara(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        scenarioContext.Set(response, "Response");
    }

    [When(@"eu enviar uma requisição PUT para ""(.*)""")]
    public async Task WhenEuEnviarUmaRequisicaoPutPara(string endpoint)
    {
        var response = await _client.PutAsJsonAsync(endpoint, _updatedInput);
        scenarioContext.Set(response, "Response");
    }

    [Given(@"que existe um edifício cadastrado com ID (.*)")]
    public void GivenQueExisteUmEdificioCadastradoComId(int id)
    {
        try
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var medidorExistente = db.Set<Edificio>().Find(id);

            if (medidorExistente == null)
            {
                var novoEdificio = new Edificio
                {
                    Id = id,
                };

                db.Set<Edificio>().Add(novoEdificio);
                db.SaveChanges();
            }
        }
        catch (Exception)
        {
            //ignored
        }
    }

    [Given(@"que não existe um edifício com ID (.*)")]
    public void GivenQueNaoExisteUmEdificioComId(int id)
    {
    }

    [Then(@"o corpo deve conter o campo ""(.*)"" com valor ""(.*)""")]
    public async Task ThenOCorpoDeveConterOCampoComValor(string campo, string valor)
    {
        var response = scenarioContext.Get<HttpResponseMessage>("Response");
        response.Should().NotBeNull();
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty(campo).ToString().Should().Be(valor);
    }

    [Then("o corpo deve conter o campo {string} com valor {int}")]
    public async Task ThenOCorpoDeveConterOCampoComValor(string campo, int id)
    {
        var response = scenarioContext.Get<HttpResponseMessage>("Response");
        response.Should().NotBeNull();
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty(campo).ToString().Should().Be(id.ToString());
    }

    [Then(@"o corpo deve conter a mensagem ""(.*)""")]
    public async Task ThenOCorpoDeveConterAMensagem(string mensagem)
    {
        var response = scenarioContext.Get<HttpResponseMessage>("Response");
        response.Should().NotBeNull();
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(mensagem);
    }
}
