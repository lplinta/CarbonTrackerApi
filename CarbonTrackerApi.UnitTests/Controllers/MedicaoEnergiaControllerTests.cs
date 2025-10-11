using System.Net;
using CarbonTrackerApi.Controllers;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.UnitTests.Controllers;

public class MedicaoEnergiaControllerTests
{
    private readonly Mock<IMedicaoEnergiaService> _mockMedicaoEnergiaService;
    private readonly MedicaoEnergiaController _controller;

    public MedicaoEnergiaControllerTests()
    {
        var mockLogger = new Mock<ILogger<MedicaoEnergiaController>>();
        _mockMedicaoEnergiaService = new Mock<IMedicaoEnergiaService>();
        _controller = new MedicaoEnergiaController(_mockMedicaoEnergiaService.Object, mockLogger.Object);
    }

    [Fact]
    public async Task PostMedicaoEnergia_ValidInput_ReturnsCreatedWithMedicaoEnergiaOutput()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        var medicaoEnergia = new MedicaoEnergia
        {
            Id = 1,
            ConsumoValor = 100.5m,
            UnidadeMedida = "kWh",
            Timestamp = DateTime.UtcNow,
            MedidorEnergiaId = 1
        };
        var expectedOutput = new MedicaoEnergiaOutput(
            medicaoEnergia.Id,
            medicaoEnergia.ConsumoValor,
            medicaoEnergia.UnidadeMedida,
            medicaoEnergia.Timestamp,
            medicaoEnergia.MedidorEnergiaId
        );

        _mockMedicaoEnergiaService.Setup(s => s.AdicionarMedicao(medicaoInput))
            .ReturnsAsync(medicaoEnergia);

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var createdResult = result as ObjectResult;
        createdResult?.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult?.Value.Should().BeOfType<MedicaoEnergiaOutput>();
        var returnedOutput = createdResult?.Value as MedicaoEnergiaOutput;
        returnedOutput.Should().BeEquivalentTo(expectedOutput);

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(medicaoInput), Times.Once);
    }

    [Fact]
    public async Task PostMedicaoEnergia_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(0, 0, "", DateTime.MinValue);
        _controller.ModelState.AddModelError("ConsumoValor", "O valor de consumo deve ser maior que zero.");

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        badRequestResult?.Value.Should().BeOfType<SerializableError>();

        var serializableError = badRequestResult?.Value as SerializableError;
        serializableError.Should().ContainKey("ConsumoValor");
        serializableError["ConsumoValor"].Should().BeEquivalentTo(new[] { "O valor de consumo deve ser maior que zero." });

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(It.IsAny<MedicaoEnergiaInput>()), Times.Never);
    }

    [Fact]
    public async Task PostMedicaoEnergia_ServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(999, 100.5m, "kWh", DateTime.UtcNow);
        _mockMedicaoEnergiaService.Setup(s => s.AdicionarMedicao(medicaoInput))
            .ReturnsAsync((MedicaoEnergia?)null);

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        notFoundResult?.Value.Should().BeNull();

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(medicaoInput), Times.Once);
    }

    [Fact]
    public async Task PostMedicaoEnergia_ServiceThrowsInvalidOperationException_ReturnsNotFound()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(999, 100.5m, "kWh", DateTime.UtcNow);
        var errorMessage = "Medidor de energia não encontrado.";
        _mockMedicaoEnergiaService.Setup(s => s.AdicionarMedicao(medicaoInput))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        notFoundResult?.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(medicaoInput), Times.Once);
    }

    [Fact]
    public async Task PostMedicaoEnergia_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(1, 100.5m, "unidade_invalida", DateTime.UtcNow);
        var errorMessage = "Unidade de medida inválida.";
        _mockMedicaoEnergiaService.Setup(s => s.AdicionarMedicao(medicaoInput))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        badRequestResult?.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(medicaoInput), Times.Once);
    }

    [Fact]
    public async Task PostMedicaoEnergia_ServiceThrowsGenericException_ReturnsInternalServerError()
    {
        // Arrange
        var medicaoInput = new MedicaoEnergiaInput(1, 100.5m, "kWh", DateTime.UtcNow);
        var errorMessage = "Erro inesperado ao salvar medição.";
        _mockMedicaoEnergiaService.Setup(s => s.AdicionarMedicao(medicaoInput))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.PostMedicaoEnergia(medicaoInput);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        internalServerErrorResult?.Value.Should().BeEquivalentTo(new { message = "Ocorreu um erro interno ao adicionar medição." });

        _mockMedicaoEnergiaService.Verify(s => s.AdicionarMedicao(medicaoInput), Times.Once);
    }
}