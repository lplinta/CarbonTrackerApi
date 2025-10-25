using System.Text;
using CarbonTrackerApi.BDDTests.Utils;
using CarbonTrackerApi.Data;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class EdificioSteps(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
{
    private readonly HttpClient _client = factory.CreateClient();
    private EdificioInput? _input;

    [Given(@"que o edifício com ID (.*) e Nome ""(.*)"" existe no banco de dados")]
    public void DadoQueOEdificioComIdENomeExisteNoBancoDeDados(int edificioId, string nomeEdificio)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var edificioExistente = db.Set<Edificio>().Find(edificioId);

        if (edificioExistente == null)
        {
            var novoEdificio = new Edificio
            {
                Id = edificioId,
                Nome = nomeEdificio,
                Endereco = $"Endereço de Teste para ID {edificioId}"
            };

            db.Set<Edificio>().Add(novoEdificio);
            db.SaveChanges();
        }
    }

    [Given(@"que eu tenha os seguintes dados de edifício:")]
    public void DadoQueEuTenhaOsSeguintesDadosDeEdificio(Table table)
    {
        var data = table.Rows.ToDictionary(r => r["campo"], r => r["valor"]);

        _input = new EdificioInput
        {
            Nome = data["nome"],
            Endereco = data["endereco"],
            Cidade = data["cidade"],
            TipoEdificio = data["tipoEdificio"]
        };
    }

    [When(@"eu enviar a requisição POST para o endpoint ""(.*)""")]
    public async Task QuandoEuEnviarARequisicaoPostParaOEndpoint(string endpoint)
    {
        var json = JsonConvert.SerializeObject(_input);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(endpoint, content);
        scenarioContext.Set(response, "Response");
    }

    [When(@"eu enviar a requisição GET para o endpoint ""(.*)""")]
    public async Task QuandoEuEnviarARequisicaoGetParaOEndpoint(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        scenarioContext.Set(response, "Response");
    }
}