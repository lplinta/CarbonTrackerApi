using System.Text;
using CarbonTrackerApi.BDDTests.Utils;
using CarbonTrackerApi.Data;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class MedicaoEnergiaSteps(CustomWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private HttpResponseMessage? _response;
    private MedicaoEnergiaInput? _input;

    [Given("que o edifício com ID {int} existe no banco de dados")]
    public void GivenQueOEdificioComIdExisteNoBancoDeDados(int edificioId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var medidorExistente = db.Set<Edificio>().Find(edificioId);

        if (medidorExistente == null)
        {
            var novoEdificio = new Edificio
            {
                Id = edificioId,
            };

            db.Set<Edificio>().Add(novoEdificio);
            db.SaveChanges();
        }
    }

    [Given(@"que o medidor de energia com ID {int} existe no banco de dados")]
    public void DadoQueOMedidorDeEnergiaComIdExisteNoBancoDeDados(int medidorId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var medidorExistente = db.Set<MedidorEnergia>().Find(medidorId);

        if (medidorExistente == null)
        {
            var novoMedidor = new MedidorEnergia
            {
                Id = medidorId,
                NumeroSerie = "123456789",
                TipoMedidor = "Eletricidade",
                Localizacao = "Casa",
                EdificioId = 1
            };

            db.Set<MedidorEnergia>().Add(novoMedidor);
            db.SaveChanges();
        }
    }

    [Given(@"que eu tenha os seguintes dados da medição:")]
    public void DadoQueEuTenhaOsSeguintesDadosDaMedicao(Table table)
    {
        var data = table.Rows.ToDictionary(r => r["campo"], r => r["valor"]);

        _input = new MedicaoEnergiaInput
        {
            ConsumoValor = decimal.Parse(data["consumoValor"]),
            UnidadeMedida = data["unidadeMedida"],
            Timestamp = DateTime.Parse(data["timestamp"]),
            MedidorEnergiaId = int.Parse(data["medidorEnergiaId"])
        };
    }

    [When(@"eu enviar a requisição para o endpoint ""(.*)""")]
    public async Task QuandoEuEnviarARequisicaoParaOEndpoint(string endpoint)
    {
        var json = JsonConvert.SerializeObject(_input);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _response = await _client.PostAsync(endpoint, content);
    }

    [Then(@"o status code da resposta deve ser (.*)")]
    public void EntaoOStatusCodeDaRespostaDeveSer(int statusCode)
    {
        _response.Should().NotBeNull();
        ((int)_response!.StatusCode).Should().Be(statusCode);
    }

    [Then(@"o corpo da resposta deve conter o campo ""(.*)"" com valor ""(.*)""")]
    public async Task EntaoOCorpoDaRespostaDeveConterOCampoComValor(string campo, string valorEsperado)
    {
        var body = await _response!.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

        json.Should().ContainKey(campo);
        json[campo].ToString().Should().Be(valorEsperado);
    }
}