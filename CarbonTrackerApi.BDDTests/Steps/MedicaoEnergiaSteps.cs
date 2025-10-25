using CarbonTrackerApi.Controllers;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Steps;

[Binding]
public class MedicaoEnergiaSteps
{
    private readonly Mock<IMedicaoEnergiaService> _mockService = new();
    private readonly Mock<ILogger<MedicaoEnergiaController>> _mockLogger = new();
    private MedicaoEnergiaController? _controller;
    private IActionResult? _resultado;
    private MedicaoEnergiaInput? _input;

    [Given("que o serviço de medição está disponível")]
    public void DadoQueOServicoDeMedicaoEstaDisponivel()
    {
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
    }

    [Given("que o input de medição é válido")]
    public void DadoQueOInputDeMedicaoEValido()
    {
        _input = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        var medicao = new MedicaoEnergia
        {
            Id = 1,
            ConsumoValor = 100.5m,
            UnidadeMedida = "kWh",
            Timestamp = _input.Timestamp,
            MedidorEnergiaId = 1
        };

        _mockService.Setup(s => s.AdicionarMedicao(_input)).ReturnsAsync(medicao);
    }

    [Given("que o input de medição é inválido")]
    public void DadoQueOInputDeMedicaoEInvalido()
    {
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
        _controller.ModelState.AddModelError("ConsumoValor", "O valor de consumo deve ser maior que zero.");
        _input = new MedicaoEnergiaInput(0, 0, "", DateTime.MinValue);
    }

    [Given("que o serviço retorna null ao adicionar a medição")]
    public void DadoQueOServicoRetornaNullAoAdicionarAMedicao()
    {
        _input = new MedicaoEnergiaInput(999, 100.5m, "kWh", DateTime.UtcNow);
        _mockService.Setup(s => s.AdicionarMedicao(_input)).ReturnsAsync((MedicaoEnergia?)null);
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
    }

    [Given("que o serviço lança InvalidOperationException")]
    public void DadoQueOServicoLancaInvalidOperationException()
    {
        _input = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        _mockService.Setup(s => s.AdicionarMedicao(_input))
            .ThrowsAsync(new InvalidOperationException("Medidor de energia não encontrado."));
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
    }

    [Given("que o serviço lança ArgumentException")]
    public void DadoQueOServicoLancaArgumentException()
    {
        _input = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        _mockService.Setup(s => s.AdicionarMedicao(_input))
            .ThrowsAsync(new ArgumentException("Unidade de medida inválida."));
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
    }

    [Given("que o serviço lança uma exceção genérica")]
    public void DadoQueOServicoLancaUmaExcecaoGenerica()
    {
        _input = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        _mockService.Setup(s => s.AdicionarMedicao(_input))
            .ThrowsAsync(new Exception("Erro inesperado"));
        _controller = new MedicaoEnergiaController(_mockService.Object, _mockLogger.Object);
    }

    [When("o usuário enviar uma requisição POST para o endpoint de medição")]
    public async Task QuandoOUsuarioEnviarUmaRequisicaoPostParaOEndpointDeMedicao()
    {
        _resultado = await _controller!.PostMedicaoEnergia(_input!);
    }

    [Then("o sistema deve retornar status (.*)")]
    public void EntaoOSistemaDeveRetornarStatus(int statusCode)
    {
        _resultado.Should().BeAssignableTo<ObjectResult>();
        var result = _resultado as ObjectResult;
        result?.StatusCode.Should().Be(statusCode);
    }

    [Then("os dados da medição criada devem ser retornados")]
    public void EntaoOsDadosDaMedicaoCriadaDevemSerRetornados()
    {
        var result = _resultado as ObjectResult;
        result?.Value.Should().BeOfType<MedicaoEnergiaOutput>();
    }
}
