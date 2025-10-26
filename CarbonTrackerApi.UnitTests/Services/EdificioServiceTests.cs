using Moq;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;
using CarbonTrackerApi.Services;

namespace CarbonTrackerApi.UnitTests.Services;
public class EdificioServiceTests
{
    private readonly Mock<IFatorEmissaoService> _mockFatorEmissaoService;
    private readonly Mock<IEdificioRepository> _mockEdificioRepository;
    private readonly Mock<IMedicaoEnergiaRepository> _mockMedicaoEnergiaRepository;
    private readonly Mock<IMetaCarbonoRepository> _mockMetaCarbonoRepository;
    private readonly EdificioService _edificioService;

    public EdificioServiceTests()
    {
        _mockFatorEmissaoService = new Mock<IFatorEmissaoService>();
        _mockEdificioRepository = new Mock<IEdificioRepository>();
        _mockMedicaoEnergiaRepository = new Mock<IMedicaoEnergiaRepository>();
        _mockMetaCarbonoRepository = new Mock<IMetaCarbonoRepository>();

        _edificioService = new EdificioService(
            _mockFatorEmissaoService.Object,
            _mockEdificioRepository.Object,
            _mockMedicaoEnergiaRepository.Object,
            _mockMetaCarbonoRepository.Object
        );
    }

    [Fact]
    public async Task ObterConsumoDiario_DeveRetornarConsumoCorreto_QuandoDadosValidos()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 01, 02, 23, 59, 59, DateTimeKind.Utc);
        var paginationInput = new PaginationInput(1);

        var medidor1 = new MedidorEnergia { Id = 1, TipoMedidor = "Eletricidade" };
        var medidor2 = new MedidorEnergia { Id = 2, TipoMedidor = "Gás Natural" };

        var edificio = new Edificio
        {
            Id = edificioId,
            Nome = "Edificio Teste",
            Cidade = "São Paulo",
            Medidores = [medidor1, medidor2]
        };

        medidor1.Medicoes =
            [
                new MedicaoEnergia { Id = 1, MedidorEnergia = medidor1, Timestamp = new DateTime(2024, 01, 01, 10, 0, 0, DateTimeKind.Utc), ConsumoValor = 50.0m },
                new MedicaoEnergia { Id = 2, MedidorEnergia = medidor1, Timestamp = new DateTime(2024, 01, 01, 14, 0, 0, DateTimeKind.Utc), ConsumoValor = 30.0m },
                new MedicaoEnergia { Id = 3, MedidorEnergia = medidor1, Timestamp = new DateTime(2024, 01, 02, 09, 0, 0, DateTimeKind.Utc), ConsumoValor = 20.0m }
            ];
        medidor2.Medicoes =
            [
                new MedicaoEnergia { Id = 4, MedidorEnergia = medidor2, Timestamp = new DateTime(2024, 01, 01, 11, 0, 0, DateTimeKind.Utc), ConsumoValor = 10.0m },
                new MedicaoEnergia { Id = 5, MedidorEnergia = medidor2, Timestamp = new DateTime(2024, 01, 02, 16, 0, 0, DateTimeKind.Utc), ConsumoValor = 5.0m }
            ];

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockFatorEmissaoService
            .Setup(s => s.GetFatorEmissaoByTipoMedidorAndDateAsync("Eletricidade", It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new FatorEmissao { ValorEmissao = 0.5m });
        _mockFatorEmissaoService
            .Setup(s => s.GetFatorEmissaoByTipoMedidorAndDateAsync("Gás Natural", It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(new FatorEmissao { ValorEmissao = 0.8m });

        // Act
        var result = await _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);

        var consumoDia1 = result.Items.FirstOrDefault(c => c.Data.Date == new DateTime(2024, 01, 01).Date);
        Assert.NotNull(consumoDia1);
        Assert.Equal(90.0m, consumoDia1.ConsumoTotalKWh);
        Assert.Equal(50 * 0.5m + 30 * 0.5m + 10 * 0.8m, consumoDia1.EmissaoCo2Equivalente);

        var consumoDia2 = result.Items.FirstOrDefault(c => c.Data.Date == new DateTime(2024, 01, 02).Date);
        Assert.NotNull(consumoDia2);
        Assert.Equal(25.0m, consumoDia2.ConsumoTotalKWh);
        Assert.Equal(20 * 0.5m + 5 * 0.8m, consumoDia2.EmissaoCo2Equivalente);
    }

    [Fact]
    public async Task ObterConsumoDiario_DeveLancarExcecao_QuandoEdificioNaoEncontrado()
    {
        // Arrange
        var edificioId = 99;
        var dataInicio = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 01, 01, 23, 59, 59, DateTimeKind.Utc);
        var paginationInput = new PaginationInput(1);

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput));
        Assert.Equal($"Edifício com ID {edificioId} não encontrado.", exception.Message);
    }

    [Fact]
    public async Task ObterConsumoDiario_DeveLancarExcecao_QuandoDataInicioMaiorQueDataFim()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2024, 01, 02);
        var dataFim = new DateTime(2024, 01, 01);
        var paginationInput = new PaginationInput(1);

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(new Edificio()); // Edifício mock

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput));
        Assert.Equal("A data de início não pode ser posterior à data de fim.", exception.Message);
    }

    [Fact]
    public async Task ObterConsumoDiario_DeveRetornarListaVazia_QuandoEdificioNaoTemMedidores()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 01, 02, 23, 59, 59, DateTimeKind.Utc);
        var paginationInput = new PaginationInput(1);

        var edificio = new Edificio
        {
            Id = edificioId,
            Nome = "Edificio Sem Medidores",
            Medidores = [] // Sem medidores
        };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);

        // Act
        var result = await _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task ObterConsumoDiario_DeveRetornarListaVazia_QuandoNaoHaMedicoesNoPeriodo()
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 01, 02, 23, 59, 59, DateTimeKind.Utc);
        var paginationInput = new PaginationInput(1);

        var medidor = new MedidorEnergia { Id = 1, TipoMedidor = "Eletricidade" };
        medidor.Medicoes =
            [
                new MedicaoEnergia { Id = 1, MedidorEnergia = medidor, Timestamp = new DateTime(2023, 12, 31, 10, 0, 0, DateTimeKind.Utc), ConsumoValor = 50.0m }
            ];
        var edificio = new Edificio
        {
            Id = edificioId,
            Nome = "Edificio com Medicoes Fora do Periodo",
            Medidores = [medidor]
        };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);

        // Act
        var result = await _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public async Task ObterConsumoDiario_DeveLancarExcecao_QuandoPaginationInvalida(int pageNumber, int pageSize)
    {
        // Arrange
        var edificioId = 1;
        var dataInicio = new DateTime(2024, 01, 01);
        var dataFim = new DateTime(2024, 01, 02);
        var paginationInput = new PaginationInput(pageNumber, pageSize);

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(new Edificio()); // Edifício mock

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _edificioService.ObterConsumoDiario(edificioId, dataInicio, dataFim, paginationInput));
        Assert.Equal("O número da página (PageNumber) e o tamanho da página (PageSize) devem ser maiores que zero.", exception.Message);
    }

    // --- Testes para ObterAlertasConsumo ---

    [Fact]
    public async Task ObterAlertasConsumo_DeveRetornarAlerta_QuandoConsumoHojeExcedeLimite()
    {
        // Arrange
        var edificioId = 1;
        var paginationInput = new PaginationInput(1);

        // Define o 'hoje' como um ponto fixo para o teste, usando Utc
        var hoje = DateTime.UtcNow.Date; 
        var diaAnterior = hoje.AddDays(-1);
        var doisDiasAtras = hoje.AddDays(-2);

        var medidor = new MedidorEnergia { Id = 1, TipoMedidor = "Eletricidade" };
        var edificio = new Edificio { Id = edificioId, Nome = "Edificio Alerta", Medidores = new List<MedidorEnergia> { medidor } };

        var medicoes = new List<MedicaoEnergia>
        {
            new() { Id = 1, MedidorEnergia = medidor, Timestamp = hoje.AddHours(10), ConsumoValor = 600m }, // Consumo de hoje
            new() { Id = 2, MedidorEnergia = medidor, Timestamp = diaAnterior.AddHours(12), ConsumoValor = 100m }, // Consumo do dia anterior
            new() { Id = 3, MedidorEnergia = medidor, Timestamp = doisDiasAtras.AddHours(14), ConsumoValor = 100m } // Consumo de 2 dias atrás
        };

        // Simula o GetMedicoesByMedidorAndPeriodAsync para retornar as medições mockadas
        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockMedicaoEnergiaRepository
            .Setup(r => r.GetMedicoesByMedidorAndPeriodAsync(
                It.IsAny<int>(), 
                It.Is<DateTimeOffset>(d => d.Date == hoje.AddDays(-7).Date), // Garante que o período de 7 dias é respeitado
                It.Is<DateTimeOffset>(d => d.Date == hoje.Date)))
            .ReturnsAsync(medicoes);

        // Act
        var result = await _edificioService.ObterAlertasConsumo(edificioId, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Pico de Consumo Inesperado", result.Items.First().TipoAlerta);
        Assert.Equal(600m, result.Items.First().ValorRegistrado);
        Assert.Equal(hoje, result.Items.First().DataOcorrencia.Date);
    }

    [Fact]
    public async Task ObterAlertasConsumo_NaoDeveRetornarAlerta_QuandoConsumoHojeNaoExcedeLimite()
    {
        // Arrange
        var edificioId = 1;
        var paginationInput = new PaginationInput(1);
        var hoje = DateTimeOffset.UtcNow.Date;
        var diaAnterior = hoje.AddDays(-1);

        var medidor = new MedidorEnergia { Id = 1, TipoMedidor = "Eletricidade" };
        var edificio = new Edificio { Id = edificioId, Nome = "Edificio Sem Alerta", Medidores = [medidor] };

        var medicoes = new List<MedicaoEnergia>
            {
                new() { Id = 1, MedidorEnergia = medidor, Timestamp = hoje.AddHours(-2), ConsumoValor = 150m }, // Consumo de hoje (abaixo do limite)
                new() { Id = 2, MedidorEnergia = medidor, Timestamp = diaAnterior.AddHours(-2), ConsumoValor = 100m },
                new() { Id = 3, MedidorEnergia = medidor, Timestamp = diaAnterior.AddDays(-1).AddHours(-2), ConsumoValor = 100m }
            };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockMedicaoEnergiaRepository
            .Setup(r => r.GetMedicoesByMedidorAndPeriodAsync(medidor.Id, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(medicoes);

        // Act
        var result = await _edificioService.ObterAlertasConsumo(edificioId, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task ObterAlertasConsumo_DeveLancarExcecao_QuandoEdificioNaoEncontrado()
    {
        // Arrange
        var edificioId = 99;
        var paginationInput = new PaginationInput(1);

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _edificioService.ObterAlertasConsumo(edificioId, paginationInput));
        Assert.Equal($"Edifício com ID {edificioId} não encontrado.", exception.Message);
    }

    [Fact]
    public async Task ObterAlertasConsumo_DeveRetornarVazio_QuandoNaoHaMedicoes()
    {
        // Arrange
        var edificioId = 1;
        var paginationInput = new PaginationInput(1);

        var medidor = new MedidorEnergia { Id = 1, TipoMedidor = "Eletricidade" };
        var edificio = new Edificio { Id = edificioId, Nome = "Edificio Sem Medicoes", Medidores = [medidor] };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockMedicaoEnergiaRepository
            .Setup(r => r.GetMedicoesByMedidorAndPeriodAsync(medidor.Id, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync([]); // Nenhuma medição

        // Act
        var result = await _edificioService.ObterAlertasConsumo(edificioId, paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    // --- Testes para AtualizarMetaCarbono ---

    [Fact]
    public async Task AtualizarMetaCarbono_DeveAdicionarNovaMeta_QuandoNaoExistir()
    {
        // Arrange
        var edificioId = 1;
        var metaInput = new MetaCarbonoInput(2025, 10.0m, 2024);
        var edificio = new Edificio { Id = edificioId };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockMetaCarbonoRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync([]); // Nenhuma meta existente

        // Act
        var result = await _edificioService.AtualizarMetaCarbono(edificioId, metaInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(edificioId, result.EdificioId);
        Assert.Equal(metaInput.AnoMeta, result.AnoMeta);
        Assert.Equal(metaInput.ReducaoPercentual, result.ReducaoPercentual);
        Assert.Equal(metaInput.AnoBase, result.AnoBase);
        _mockMetaCarbonoRepository.Verify(r => r.AddAsync(It.IsAny<MetaCarbono>()), Times.Once);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarMetaCarbono_DeveAtualizarMetaExistente_QuandoExistir()
    {
        // Arrange
        var edificioId = 1;
        var metaInput = new MetaCarbonoInput(2025, 15.0m, 2023); // Novo percentual e ano base
        var edificio = new Edificio { Id = edificioId };
        var metaExistente = new MetaCarbono { Id = 1, EdificioId = edificioId, AnoMeta = 2025, ReducaoPercentual = 10.0m, AnoBase = 2024 };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);
        _mockMetaCarbonoRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync([metaExistente]);

        // Act
        var result = await _edificioService.AtualizarMetaCarbono(edificioId, metaInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(metaInput.ReducaoPercentual, result.ReducaoPercentual);
        Assert.Equal(metaInput.AnoBase, result.AnoBase);
        _mockMetaCarbonoRepository.Verify(r => r.Update(It.IsAny<MetaCarbono>()), Times.Once);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarMetaCarbono_DeveRetornarNull_QuandoEdificioNaoEncontrado()
    {
        // Arrange
        var edificioId = 99;
        var metaInput = new MetaCarbonoInput(2025, 10.0m, 2024);

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act
        var result = await _edificioService.AtualizarMetaCarbono(edificioId, metaInput);

        // Assert
        Assert.Null(result);
        _mockMetaCarbonoRepository.Verify(r => r.AddAsync(It.IsAny<MetaCarbono>()), Times.Never);
        _mockMetaCarbonoRepository.Verify(r => r.Update(It.IsAny<MetaCarbono>()), Times.Never);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // --- Testes para GetEdificioById ---

    [Fact]
    public async Task GetEdificioById_DeveRetornarEdificio_QuandoEncontrado()
    {
        // Arrange
        var edificioId = 1;
        var edificio = new Edificio { Id = edificioId, Nome = "Edificio ABC", Cidade = "Cidade X" };
        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(edificio);

        // Act
        var result = await _edificioService.GetEdificioById(edificioId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(edificio.Id, result.Id);
        Assert.Equal(edificio.Nome, result.Nome);
    }

    [Fact]
    public async Task GetEdificioById_DeveRetornarNull_QuandoNaoEncontrado()
    {
        // Arrange
        var edificioId = 99;
        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act
        var result = await _edificioService.GetEdificioById(edificioId);

        // Assert
        Assert.Null(result);
    }

    // --- Testes para GetAllEdificios ---

    [Fact]
    public async Task GetAllEdificios_DeveRetornarEdificiosPaginados()
    {
        // Arrange
        var paginationInput = new PaginationInput(1, 2);
        var edificios = new List<Edificio>
            {
                new() { Id = 1, Nome = "Edificio 1" },
                new() { Id = 2, Nome = "Edificio 2" },
                new() { Id = 3, Nome = "Edificio 3" }
            };

        _mockEdificioRepository.Setup(r => r.CountAsync(null)).ReturnsAsync(edificios.Count);
        _mockEdificioRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync(edificios);

        // Act
        var result = await _edificioService.GetAllEdificios(paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count); // Deve retornar 2 itens na primeira página
        Assert.Equal("Edificio 1", result.Items[0].Nome);
        Assert.Equal("Edificio 2", result.Items[1].Nome);
    }

    [Fact]
    public async Task GetAllEdificios_DeveRetornarListaVazia_QuandoNenhumEdificioExiste()
    {
        // Arrange
        var paginationInput = new PaginationInput(1);
        _mockEdificioRepository.Setup(r => r.CountAsync(null)).ReturnsAsync(0);
        _mockEdificioRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync([]);

        // Act
        var result = await _edificioService.GetAllEdificios(paginationInput);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    // --- Testes para AddEdificio ---

    [Fact]
    public async Task AddEdificio_DeveAdicionarNovoEdificio()
    {
        // Arrange
        var edificioInput = new EdificioInput("Novo Edificio", "Cidade Teste", "Rua Teste", "Comercial", "15", "25");
        var newEdificioId = 1;

        _mockEdificioRepository.Setup(r => r.AddAsync(It.IsAny<Edificio>()))
            .Callback<Edificio>(e => e.Id = newEdificioId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _edificioService.AddEdificio(edificioInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newEdificioId, result.Id);
        Assert.Equal(edificioInput.Nome, result.Nome);
        _mockEdificioRepository.Verify(r => r.AddAsync(It.IsAny<Edificio>()), Times.Once);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // --- Testes para UpdateEdificio ---

    [Fact]
    public async Task UpdateEdificio_DeveAtualizarEdificioExistente()
    {
        // Arrange
        var edificioId = 1;
        var edificioInput = new EdificioInput("Edificio Atualizado", "Cidade Atualizada", "Rua Atualizada", "Residencial", "15", "25");
        var existingEdificio = new Edificio { Id = edificioId, Nome = "Antigo Nome" };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(existingEdificio);

        // Act
        var result = await _edificioService.UpdateEdificio(edificioId, edificioInput);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(edificioId, result.Id);
        Assert.Equal(edificioInput.Nome, result.Nome);
        Assert.Equal(edificioInput.Cidade, result.Cidade);
        _mockEdificioRepository.Verify(r => r.Update(existingEdificio), Times.Once);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateEdificio_DeveRetornarNull_QuandoEdificioNaoExistente()
    {
        // Arrange
        var edificioId = 99;
        var edificioInput = new EdificioInput("Edificio Atualizado", "Cidade Atualizada", "Rua Atualizada", "Residencial", "15", "25");

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act
        var result = await _edificioService.UpdateEdificio(edificioId, edificioInput);

        // Assert
        Assert.Null(result);
        _mockEdificioRepository.Verify(r => r.Update(It.IsAny<Edificio>()), Times.Never);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // --- Testes para DeleteEdificio ---

    [Fact]
    public async Task DeleteEdificio_DeveRetornarTrue_QuandoEdificioExiste()
    {
        // Arrange
        var edificioId = 1;
        var existingEdificio = new Edificio { Id = edificioId, Nome = "Edificio a Deletar" };

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync(existingEdificio);

        // Act
        var result = await _edificioService.DeleteEdificio(edificioId);

        // Assert
        Assert.True(result);
        _mockEdificioRepository.Verify(r => r.Delete(existingEdificio), Times.Once);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteEdificio_DeveRetornarFalse_QuandoEdificioNaoExiste()
    {
        // Arrange
        var edificioId = 99;

        _mockEdificioRepository.Setup(r => r.GetByIdAsync(edificioId)).ReturnsAsync((Edificio?)null);

        // Act
        var result = await _edificioService.DeleteEdificio(edificioId);

        // Assert
        Assert.False(result);
        _mockEdificioRepository.Verify(r => r.Delete(It.IsAny<Edificio>()), Times.Never);
        _mockEdificioRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
