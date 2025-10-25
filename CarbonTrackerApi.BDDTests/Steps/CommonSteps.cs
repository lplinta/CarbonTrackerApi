using FluentAssertions;
using Newtonsoft.Json;
using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class CommonSteps(ScenarioContext scenarioContext)
{
    [Then(@"o status code da resposta deve ser (.*)")]
    public void EntaoOStatusCodeDaRespostaDeveSer(int statusCode)
    {
        var response = scenarioContext.Get<HttpResponseMessage>("Response");
        response.Should().NotBeNull();
        ((int)response!.StatusCode).Should().Be(statusCode);
    }

    [Given(@"que eu esteja autenticado como um usuário válido")]
    public void DadoQueEuEstejaAutenticadoComoUmUsuarioValido()
    {
    }

    [Then(@"o corpo da resposta deve conter o campo ""(.*)"" com valor ""(.*)""")]
    public async Task EntaoOCorpoDaRespostaDeveConterOCampoComValor(string campo, string valorEsperado)
    {
        var response = scenarioContext.Get<HttpResponseMessage>("Response");
        response.Should().NotBeNull();
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
        json.Should().ContainKey(campo);
        json[campo].ToString().Should().Be(valorEsperado);
    }
}