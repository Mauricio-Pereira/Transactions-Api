﻿using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetTransacaoByTxidHandler
    : IRequestHandler<GetTransacaoByTxidQuery, TransacaoResourceDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly ILogger<GetTransacaoByTxidHandler> _logger;

    public GetTransacaoByTxidHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<GetTransacaoByTxidHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task<TransacaoResourceDTO> Handle(
        GetTransacaoByTxidQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processamento para obter transação com Txid: {Txid}", request.Txid);

        // 1) Tenta buscar no cache
        var transacaoCache = await _cachingService.GetAsync(request.Txid);
        if (!string.IsNullOrEmpty(transacaoCache))
        {
            _logger.LogInformation("Transação com Txid {Txid} encontrada no cache", request.Txid);

            try
            {
                // Tenta desserializar como TransacaoResourceDTO
                var resourceCache = JsonConvert.DeserializeObject<TransacaoResourceDTO>(transacaoCache);

                if (resourceCache?.Transacao == null)
                {
                    _logger.LogWarning(
                        "Falha ao desserializar TransacaoResourceDTO para Txid {Txid}. Tentando desserializar como TransacaoResponseDTO",
                        request.Txid);

                    // Então tenta como TransacaoResponseDTO
                    var fallback = JsonConvert.DeserializeObject<TransacaoResponseDTO>(transacaoCache);
                    if (fallback != null)
                    {
                        resourceCache = new TransacaoResourceDTO { Transacao = fallback };
                    }
                }

                return resourceCache;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desserializar cache para Txid {Txid}", request.Txid);
                throw;
            }
        }

        _logger.LogInformation("Transação com Txid {Txid} não encontrada no cache. Buscando no banco de dados",
            request.Txid);

        // 2) Busca no banco de dados
        var transacao = await _transacaoService.GetByTxidAsync(request.Txid);
        if (transacao == null)
        {
            _logger.LogWarning("Transação com Txid {Txid} não encontrada no banco de dados", request.Txid);
            return null; // O controller tratará como NotFound
        }

        _logger.LogInformation("Transação com Txid {Txid} encontrada no banco de dados", request.Txid);

        // 3) Monta o recurso
        var resource = new TransacaoResourceDTO { Transacao = transacao };

        // 4) Salva no cache
        try
        {
            await _cachingService.SetAsync(request.Txid, JsonConvert.SerializeObject(resource));
            _logger.LogInformation("Transação com Txid {Txid} armazenada no cache", request.Txid);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao armazenar transação com Txid {Txid} no cache", request.Txid);
        }

        return resource;
    }
}