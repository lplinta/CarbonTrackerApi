using System.Net;
using CarbonTrackerApi.Controllers;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarbonTrackerApi.UnitTests.Controllers;

public class EdificioControllerTests
{
    private readonly Mock<IEdificioService> _mockEdificioService;
    private readonly EdificioController _controller;

    public EdificioControllerTests()
    {
        var mockLogger = new Mock<ILogger<EdificioController>>();
        _mockEdificioService = new Mock<IEdificioService>();
        _controller = new EdificioController(_mockEdificioService.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetConsumoDiario_ShouldReturnOk_WhenConsumoExists()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2023, 1, 1);
        var dataFim = new DateTime(2023, 1, 31);
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };

        var consumoDiarioOutput = new PaginatedOutput<ConsumoDiarioOutput>
        (
            [
                new(DateTime.Now.Date, 1, 100.0m),
                new(DateTime.Now.Date.AddDays(-1), 1, 120.0m)
            ],
            2,
            1,
            10,
            1
        );

        _mockEdificioService.Setup(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput))
            .ReturnsAsync(consumoDiarioOutput);

        // Act
        var result = await _controller.GetConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);

        okResult?.Value.Should().BeOfType<PaginatedOutput<ConsumoDiarioOutput>>();
        var returnedValue = okResult?.Value as PaginatedOutput<ConsumoDiarioOutput>;
        returnedValue.Should().BeEquivalentTo(consumoDiarioOutput);

        _mockEdificioService.Verify(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetConsumoDiario_ShouldReturnNotFound_WhenInvalidOperationExceptionIsThrown()
    {
        // Arrange
        var edificioId = 999;
        var dataInicio = new DateTime(2023, 1, 1);
        var dataFim = new DateTime(2023, 1, 31);
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };

        _mockEdificioService.Setup(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput))
            .ThrowsAsync(new InvalidOperationException("Edifício não encontrado."));

        // Act
        var result = await _controller.GetConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        notFoundResult?.Value.Should().BeEquivalentTo(new { message = "Edifício não encontrado." });

        _mockEdificioService.Verify(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetConsumoDiario_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2023, 1, 1);
        var dataFim = new DateTime(2022, 12, 31);
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };

        _mockEdificioService.Setup(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput))
            .ThrowsAsync(new ArgumentException("A data de início não pode ser posterior à data de fim."));

        // Act
        var result = await _controller.GetConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        badRequestResult?.Value.Should().BeEquivalentTo(new { message = "A data de início não pode ser posterior à data de fim." });

        _mockEdificioService.Verify(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetConsumoDiario_ShouldReturnInternalServerError_WhenGenericExceptionIsThrown()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2023, 1, 1);
        var dataFim = new DateTime(2023, 1, 31);
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };

        _mockEdificioService.Setup(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput))
            .ThrowsAsync(new Exception("Erro inesperado."));

        // Act
        var result = await _controller.GetConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        internalServerErrorResult?.Value.Should().BeEquivalentTo(new { message = "Ocorreu um erro interno ao processar sua requisição." });

        _mockEdificioService.Verify(s => s.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetConsumoDiario_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2023, 1, 1);
        var dataFim = new DateTime(2023, 1, 31);
        var paginationInput = new PaginationInput { PageNumber = 0, PageSize = 10 };

        _controller.ModelState.AddModelError("PageNumber", "O PageNumber deve ser maior que zero.");

        // Act
        var result = await _controller.GetConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        badRequestResult?.Value.Should().BeOfType<SerializableError>();

        var serializableError = badRequestResult?.Value as SerializableError;
        serializableError.Should().ContainKey("PageNumber");
        serializableError["PageNumber"].Should().BeEquivalentTo(new[] { "O PageNumber deve ser maior que zero." });

        _mockEdificioService.Verify(s => s.ObterConsumoDiario(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<PaginationInput>()), Times.Never);
    }

    [Fact]
    public async Task GetAlertasConsumo_ValidInput_ReturnsOkWithPaginatedOutput()
    {
        // Arrange
        var edificioId = 1;
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };
        var alertaList = new List<AlertaConsumoOutput>
        {
            new(1, "Alto Consumo", 1500m, 1200m, DateTime.UtcNow, "Consumo excedeu o limite esperado.", 1)
        };
        var paginatedOutput = new PaginatedOutput<AlertaConsumoOutput>(alertaList, 1, 1, 10, 1);

        _mockEdificioService.Setup(s => s.ObterAlertasConsumo(edificioId, paginationInput))
            .ReturnsAsync(paginatedOutput);

        // Act
        var result = await _controller.GetAlertasConsumo(edificioId, paginationInput);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);

        okResult?.Value.Should().BeOfType<PaginatedOutput<AlertaConsumoOutput>>();
        var returnedPaginatedOutput = okResult?.Value as PaginatedOutput<AlertaConsumoOutput>;
        returnedPaginatedOutput.Should().BeEquivalentTo(paginatedOutput);

        _mockEdificioService.Verify(s => s.ObterAlertasConsumo(edificioId, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetAlertasConsumo_ServiceThrowsInvalidOperationException_ReturnsNotFound()
    {
        // Arrange
        var edificioId = 999;
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };
        var errorMessage = "Edifício não encontrado.";
        _mockEdificioService.Setup(s => s.ObterAlertasConsumo(edificioId, paginationInput))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.GetAlertasConsumo(edificioId, paginationInput);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        notFoundResult?.Value.Should().BeEquivalentTo(new { message = errorMessage });

        _mockEdificioService.Verify(s => s.ObterAlertasConsumo(edificioId, paginationInput), Times.Once);
    }

    [Fact]
    public async Task GetAlertasConsumo_ServiceThrowsGenericException_ReturnsInternalServerError()
    {
        // Arrange
        var edificioId = 1;
        var paginationInput = new PaginationInput { PageNumber = 1, PageSize = 10 };
        var errorMessage = "Ocorreu um erro inesperado.";
        _mockEdificioService.Setup(s => s.ObterAlertasConsumo(edificioId, paginationInput))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.GetAlertasConsumo(edificioId, paginationInput);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        internalServerErrorResult?.Value.Should().BeEquivalentTo(new { message = "Ocorreu um erro interno ao processar sua requisição." });

        _mockEdificioService.Verify(s => s.ObterAlertasConsumo(edificioId, paginationInput), Times.Once);
    }

    [Fact]
    public async Task PutMetaCarbono_ValidInput_ReturnsOkWithMetaCarbonoOutput()
    {
        // Arrange
        var edificioId = 1;
        var input = new MetaCarbonoInput(2025, 0.15m, 2024);
        var metaCarbonoModel = new Models.MetaCarbono
        {
            Id = 1,
            EdificioId = edificioId,
            AnoMeta = 2025,
            ReducaoPercentual = 0.15m,
            AnoBase = 2024,
            DataCriacao = DateTime.UtcNow
        };

        var expectedOutput = new MetaCarbonoOutput(metaCarbonoModel.Id, metaCarbonoModel.EdificioId, metaCarbonoModel.AnoMeta, metaCarbonoModel.ReducaoPercentual, metaCarbonoModel.AnoBase, metaCarbonoModel.DataCriacao);

        _mockEdificioService.Setup(s => s.AtualizarMetaCarbono(edificioId, input))
            .ReturnsAsync(metaCarbonoModel);

        // Act
        var result = await _controller.PutMetaCarbono(edificioId, input);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);

        okResult?.Value.Should().BeOfType<MetaCarbonoOutput>();
        var returnedOutput = okResult?.Value as MetaCarbonoOutput;
        returnedOutput.Should().BeEquivalentTo(expectedOutput);

        _mockEdificioService.Verify(s => s.AtualizarMetaCarbono(edificioId, input), Times.Once);
    }

    [Fact]
    public async Task PutMetaCarbono_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var edificioId = 1;
        var input = new MetaCarbonoInput(0, 0, 0);
        _controller.ModelState.AddModelError("AnoMeta", "Ano da meta é inválido.");

        // Act
        var result = await _controller.PutMetaCarbono(edificioId, input);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        badRequestResult?.Value.Should().BeOfType<SerializableError>();
        var serializableError = badRequestResult?.Value as SerializableError;
        serializableError.Should().ContainKey("AnoMeta");
        serializableError["AnoMeta"].Should().BeEquivalentTo(new[] { "Ano da meta é inválido." });

        _mockEdificioService.Verify(s => s.AtualizarMetaCarbono(It.IsAny<int>(), It.IsAny<MetaCarbonoInput>()), Times.Never);
    }

    [Fact]
    public async Task PutMetaCarbono_ServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        var edificioId = 999;
        var input = new MetaCarbonoInput(2025, 0.15m, 2024);
        _mockEdificioService.Setup(s => s.AtualizarMetaCarbono(edificioId, input))
            .ReturnsAsync((Models.MetaCarbono?)null);

        // Act
        var result = await _controller.PutMetaCarbono(edificioId, input);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult?.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        notFoundResult?.Value.Should().Be($"Edifício com ID {edificioId} não encontrado.");

        _mockEdificioService.Verify(s => s.AtualizarMetaCarbono(edificioId, input), Times.Once);
    }


    [Fact]
    public async Task PutMetaCarbono_ServiceThrowsGenericException_ReturnsInternalServerError()
    {
        // Arrange
        var edificioId = 1;
        var input = new MetaCarbonoInput(2025, 0.15m, 2024);
        var errorMessage = "Erro de banco de dados.";
        _mockEdificioService.Setup(s => s.AtualizarMetaCarbono(edificioId, input))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.PutMetaCarbono(edificioId, input);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        internalServerErrorResult?.Value.Should().BeEquivalentTo(new { message = "Ocorreu um erro interno ao processar sua requisição." });

        _mockEdificioService.Verify(s => s.AtualizarMetaCarbono(edificioId, input), Times.Once);
    }
}