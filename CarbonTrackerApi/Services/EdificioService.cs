using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Services;

public class EdificioService(
    IFatorEmissaoService fatorEmissaoService,
    IEdificioRepository edificioRepository,
    IMedicaoEnergiaRepository medicaoEnergiaRepository,
    IMetaCarbonoRepository metaCarbonoRepository)
    : IEdificioService
{
    public async Task<PaginatedOutput<ConsumoDiarioOutput>> ObterConsumoDiario(int edificioId, DateTime dataInicio,
        DateTime dataFim, PaginationInput paginationInput)
    {
        if (dataInicio.Kind == DateTimeKind.Unspecified)
            dataInicio = DateTime.SpecifyKind(dataInicio, DateTimeKind.Utc);
        if (dataFim.Kind == DateTimeKind.Unspecified)
            dataFim = DateTime.SpecifyKind(dataFim, DateTimeKind.Utc);

        dataFim = dataFim.Date.AddDays(1).AddTicks(-1);

        if (dataInicio > dataFim)
            throw new ArgumentException("A data de início não pode ser posterior à data de fim.");
        if (paginationInput.PageNumber < 1 || paginationInput.PageSize < 1)
            throw new ArgumentException("O número da página (PageNumber) e o tamanho da página (PageSize) devem ser maiores que zero.");

        var edificio = await edificioRepository.GetByIdAsync(edificioId);
        if (edificio == null)
            throw new InvalidOperationException($"Edifício com ID {edificioId} não encontrado.");

        var medicoesDoEdificio = new List<MedicaoEnergia>();
        if (edificio.Medidores != null)
        {
            foreach (var medidor in edificio.Medidores)
            {
                if (medidor.Medicoes != null)
                    medicoesDoEdificio.AddRange(medidor.Medicoes.Where(m => m.Timestamp >= dataInicio && m.Timestamp <= dataFim));
            }
        }

        var agrupadoPorData = medicoesDoEdificio
            .GroupBy(m => m.Timestamp.Date)
            .OrderBy(g => g.Key)
            .ToList();

        var totalCount = agrupadoPorData.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / paginationInput.PageSize);

        var paginatedGroupedData = agrupadoPorData
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Take(paginationInput.PageSize);

        var resultados = new List<ConsumoDiarioOutput>();
        foreach (var grupoDia in paginatedGroupedData)
        {
            decimal consumoTotalDia = 0;
            decimal emissaoTotalDia = 0;

            foreach (var medicao in grupoDia)
            {
                consumoTotalDia += medicao.ConsumoValor;

                var tipoMedidorFonte = medicao.MedidorEnergia?.TipoMedidor ?? "Eletricidade";

                var fatorEmissao = await fatorEmissaoService.GetFatorEmissaoByTipoMedidorAndDateAsync(
                    tipoMedidorFonte,
                    medicao.Timestamp.Date,
                    edificio.Cidade
                );

                if (fatorEmissao != null)
                    emissaoTotalDia += medicao.ConsumoValor * fatorEmissao.ValorEmissao;
                else
                    Console.WriteLine($"Aviso: Fator de emissão não encontrado para Tipo: {tipoMedidorFonte}, " +
                                      $"Data: {medicao.Timestamp.Date}, Região: {edificio.Cidade ?? "N/A"}");
            }

            resultados.Add(new ConsumoDiarioOutput(grupoDia.Key, consumoTotalDia, emissaoTotalDia));
        }

        return new PaginatedOutput<ConsumoDiarioOutput>(
            resultados,
            totalCount,
            paginationInput.PageNumber,
            paginationInput.PageSize,
            totalPages
        );
    }

    public async Task<PaginatedOutput<AlertaConsumoOutput>> ObterAlertasConsumo(int edificioId, PaginationInput paginationInput)
    {
        var todosAlertas = new List<AlertaConsumoOutput>();

        var edificio = await edificioRepository.GetByIdAsync(edificioId);
        if (edificio == null)
            throw new InvalidOperationException($"Edifício com ID {edificioId} não encontrado.");

        var dataFim = DateTimeOffset.UtcNow;
        var dataInicio = dataFim.AddDays(-7);

        var medicoesUltimos7Dias = new List<MedicaoEnergia>();
        if (edificio.Medidores != null)
        {
            foreach (var medidor in edificio.Medidores)
            {
                var medicoes = await medicaoEnergiaRepository
                    .GetMedicoesByMedidorAndPeriodAsync(medidor.Id, dataInicio, dataFim);
                medicoesUltimos7Dias.AddRange(medicoes);
            }
        }

        var consumoPorDia = medicoesUltimos7Dias
            .GroupBy(m => m.Timestamp.Date)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.ConsumoValor));

        if (consumoPorDia.Count != 0)
        {
            var diasParaMedia = consumoPorDia
                .Where(kvp => kvp.Key < DateTime.UtcNow.Date)
                .Select(kvp => kvp.Value)
                .ToList();

            if (diasParaMedia.Count == 0)
                diasParaMedia = consumoPorDia.Values.ToList();

            var mediaConsumoSemanal = diasParaMedia.Average();
            var hoje = DateTime.UtcNow.Date;

            if (consumoPorDia.TryGetValue(hoje, out var consumoHoje))
            {
                const decimal thresholdPercentual = 1.3m;
                const decimal limiteAbsoluto = 500m;

                if (consumoHoje > mediaConsumoSemanal * thresholdPercentual && consumoHoje > limiteAbsoluto)
                {
                    todosAlertas.Add(new AlertaConsumoOutput
                    (
                        1,
                        "Pico de Consumo Inesperado",
                        consumoHoje,
                        Math.Max(mediaConsumoSemanal * thresholdPercentual, limiteAbsoluto),
                        hoje,
                        $"Consumo ({consumoHoje:N2} kWh) hoje excedeu em {(thresholdPercentual - 1) * 100:N0}% " +
                                   $"a média semanal ({mediaConsumoSemanal:N2} kWh) e o limite de {limiteAbsoluto:N2} kWh " +
                                   $"para o edifício {edificio.Nome}.",
                        edificioId
                    ));
                }
            }
        }

        var totalCount = todosAlertas.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / paginationInput.PageSize);

        var paginatedAlertas = todosAlertas
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Take(paginationInput.PageSize)
            .ToList();

        return new PaginatedOutput<AlertaConsumoOutput>(
            paginatedAlertas,
            totalCount,
            paginationInput.PageNumber,
            paginationInput.PageSize,
            totalPages
        );
    }

    public async Task<MetaCarbono?> AtualizarMetaCarbono(int edificioId, MetaCarbonoInput metaInput)
    {
        var edificio = await edificioRepository.GetByIdAsync(edificioId);
        if (edificio == null)
            return null;

        var metasExistentes = await metaCarbonoRepository.GetAllAsync();
        var meta = metasExistentes.FirstOrDefault(m => m.EdificioId == edificioId && m.AnoMeta == metaInput.AnoMeta);

        if (meta == null)
        {
            meta = new MetaCarbono
            {
                EdificioId = edificioId,
                AnoMeta = metaInput.AnoMeta,
                ReducaoPercentual = metaInput.ReducaoPercentual,
                AnoBase = metaInput.AnoBase,
                DataCriacao = DateTime.UtcNow
            };
            await metaCarbonoRepository.AddAsync(meta);
        }
        else
        {
            meta.ReducaoPercentual = metaInput.ReducaoPercentual;
            meta.AnoBase = metaInput.AnoBase;
            metaCarbonoRepository.Update(meta);
        }

        await edificioRepository.SaveChangesAsync();

        return meta;
    }

    public async Task<EdificioOutput?> GetEdificioById(int id)
    {
        var edificio = await edificioRepository.GetByIdAsync(id);
        return edificio == null
            ? null
            : new EdificioOutput(edificio.Id, edificio.Nome, edificio.Cidade, edificio.Endereco, edificio.TipoEdificio,
                edificio.Latitude, edificio.Longitude);
    }

    public async Task<PaginatedOutput<EdificioOutput>> GetAllEdificios(PaginationInput paginationInput)
    {
        var totalCount = await edificioRepository.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / paginationInput.PageSize);

        var edificios = await edificioRepository.GetAllAsync();

        var paginatedEdificios = edificios
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Take(paginationInput.PageSize)
            .Select(e => new EdificioOutput(e.Id, e.Nome, e.Cidade, e.Endereco, e.TipoEdificio, e.Latitude, e.Longitude))
            .ToList();

        return new PaginatedOutput<EdificioOutput>(
            paginatedEdificios,
            totalCount,
            paginationInput.PageNumber,
            paginationInput.PageSize,
            totalPages
        );
    }

    public async Task<EdificioOutput> AddEdificio(EdificioInput edificioInput)
    {
        var newEdificio = new Edificio
        {
            Nome = edificioInput.Nome,
            Cidade = edificioInput.Cidade,
            Endereco = edificioInput.Endereco,
            TipoEdificio = edificioInput.TipoEdificio,
            Latitude = edificioInput.Latitude,
            Longitude = edificioInput.Longitude
        };

        await edificioRepository.AddAsync(newEdificio);
        await edificioRepository.SaveChangesAsync();

        return new EdificioOutput(newEdificio.Id, newEdificio.Nome, newEdificio.Cidade, newEdificio.Endereco, newEdificio.TipoEdificio, newEdificio.Latitude, newEdificio.Longitude);
    }

    public async Task<EdificioOutput?> UpdateEdificio(int id, EdificioInput edificioInput)
    {
        var existingEdificio = await edificioRepository.GetByIdAsync(id);
        if (existingEdificio == null)
            return null;

        existingEdificio.Nome = edificioInput.Nome;
        existingEdificio.Cidade = edificioInput.Cidade;
        existingEdificio.Endereco = edificioInput.Endereco;
        existingEdificio.TipoEdificio = edificioInput.TipoEdificio;
        existingEdificio.Latitude = edificioInput.Latitude;
        existingEdificio.Longitude = edificioInput.Longitude;

        edificioRepository.Update(existingEdificio);
        await edificioRepository.SaveChangesAsync();

        return new EdificioOutput(existingEdificio.Id, existingEdificio.Nome, existingEdificio.Cidade, existingEdificio.Endereco, existingEdificio.TipoEdificio, existingEdificio.Latitude, existingEdificio.Longitude);
    }

    public async Task<bool> DeleteEdificio(int id)
    {
        var existingEdificio = await edificioRepository.GetByIdAsync(id);
        if (existingEdificio == null)
            return false;

        edificioRepository.Delete(existingEdificio);
        await edificioRepository.SaveChangesAsync();

        return true;
    }
}