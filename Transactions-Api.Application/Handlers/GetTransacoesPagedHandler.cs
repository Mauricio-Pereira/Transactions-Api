using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetTransacoesPagedHandler : IRequestHandler<GetTransacoesPagedQuery, (IEnumerable<TransacaoResourceDTO> Items, bool NotFound)>
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ICachingService _cachingService;

        public GetTransacoesPagedHandler(
            ITransacaoService transacaoService,
            ICachingService cachingService)
        {
            _transacaoService = transacaoService;
            _cachingService = cachingService;
        }

        public async Task<(IEnumerable<TransacaoResourceDTO> Items, bool NotFound)> Handle(
            GetTransacoesPagedQuery request,
            CancellationToken cancellationToken)
        {
            // Verifica parâmetros (poderia jogar exceção ou tratar de outra forma)
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                // Indicar ao controller que é invalid
                return (Enumerable.Empty<TransacaoResourceDTO>(), notFound: true);
            }

            // 1) Cache
            var key = $"transacoes-paged-{request.Page}-{request.PageSize}";
            var transacoesCache = await _cachingService.GetAsync(key);
            if (!string.IsNullOrEmpty(transacoesCache))
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<TransacaoResourceDTO>>(transacoesCache);
                return (items, false);
            }

            // 2) Buscar
            var transacoes = await _transacaoService.GetAllPaginatedAsync(request.Page, request.PageSize);
            if (transacoes == null || !transacoes.Any())
            {
                return (Enumerable.Empty<TransacaoResourceDTO>(), notFound: true);
            }

            var transacoesResource = transacoes
                .Select(t => new TransacaoResourceDTO { Transacao = t })
                .ToList();

            // 3) Salva no cache
            await _cachingService.SetAsync(key, 
                JsonConvert.SerializeObject(transacoesResource));

            return (transacoesResource, false);
        }
    }