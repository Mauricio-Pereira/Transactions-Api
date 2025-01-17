using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetTransactionsPagedHandler : IRequestHandler<GetTransacoesPagedQuery, (IEnumerable<TransacaoResourceDTO> Items, bool NotFound)>
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ICachingService _cachingService;
        private readonly ILogger<GetTransactionsPagedHandler> _logger;

        public GetTransactionsPagedHandler(
            ITransacaoService transacaoService,
            ICachingService cachingService,
            ILogger<GetTransactionsPagedHandler> logger)
        {
            _transacaoService = transacaoService;
            _cachingService = cachingService;
            _logger = logger;
        }

        public async Task<(IEnumerable<TransacaoResourceDTO> Items, bool NotFound)> Handle(
            GetTransacoesPagedQuery request,
            CancellationToken cancellationToken)
        {
            // Verifica parâmetros (poderia jogar exceção ou tratar de outra forma)
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                _logger.LogWarning("Parâmetros inválidos.");
                // Indicar ao controller que é invalid
                return (Enumerable.Empty<TransacaoResourceDTO>(), notFound: true);
            }

            // 1) Cache
            var key = $"transacoes-paged-{request.Page}-{request.PageSize}";
            var transacoesCache = await _cachingService.GetAsync(key);
            if (!string.IsNullOrEmpty(transacoesCache))
            {
                _logger.LogInformation("Transações recuperadas do cache.");
                var items = JsonConvert.DeserializeObject<IEnumerable<TransacaoResourceDTO>>(transacoesCache);
                return (items, false);
            }

            // 2) Buscar
            var transacoes = await _transacaoService.GetAllPaginatedAsync(request.Page, request.PageSize);
            if (transacoes == null || !transacoes.Any())
            {
                _logger.LogWarning("Nenhuma transação encontrada no banco de dados.");
                return (Enumerable.Empty<TransacaoResourceDTO>(), notFound: true);
            }

            var transacoesResource = transacoes
                .Select(t => new TransacaoResourceDTO { Transacao = t })
                .ToList();

            // 3) Salva no cache
            try
            {
                await _cachingService.SetAsync(key,
                    JsonConvert.SerializeObject(transacoesResource));
                _logger.LogInformation("Transações salvas no cache.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Falha ao salvar transações no cache: {ex.Message}");
            }

            return (transacoesResource, false);
        }
    }