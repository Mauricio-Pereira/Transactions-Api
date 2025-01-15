using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetTransacaoByTxidHandler 
        : IRequestHandler<GetTransacaoByTxidQuery, TransacaoResourceDTO>
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ICachingService _cachingService;

        public GetTransacaoByTxidHandler(
            ITransacaoService transacaoService,
            ICachingService cachingService)
        {
            _transacaoService = transacaoService;
            _cachingService = cachingService;
        }

        public async Task<TransacaoResourceDTO> Handle(
            GetTransacaoByTxidQuery request, 
            CancellationToken cancellationToken)
        {
            // 1) Try cache
            var transacaoCache = await _cachingService.GetAsync(request.Txid);
            if (!string.IsNullOrEmpty(transacaoCache))
            {
                // Tenta desserializar como TransacaoResourceDTO
                var resourceCache = JsonConvert.DeserializeObject<TransacaoResourceDTO>(transacaoCache);

                if (resourceCache?.Transacao == null)
                {
                    // Então tente como TransacaoResponseDTO
                    var fallback = JsonConvert.DeserializeObject<TransacaoResponseDTO>(transacaoCache);
                    if (fallback != null)
                    {
                        resourceCache = new TransacaoResourceDTO { Transacao = fallback };
                    }
                }

                return resourceCache;
            }

            var transacao = await _transacaoService.GetByTxidAsync(request.Txid);
            if (transacao == null)
            {
                // Retorna null; o controller vai tratar como NotFound
                return null;
            }

            var resource = new TransacaoResourceDTO { Transacao = transacao };

            await _cachingService.SetAsync(request.Txid, 
                JsonConvert.SerializeObject(resource));

            return resource;
        }
    }