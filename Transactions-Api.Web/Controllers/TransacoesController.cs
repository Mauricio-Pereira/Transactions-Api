using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Shared.Exceptions;
using Transactions_Api.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Transactions_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransacoesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITransacaoService _transacao;


    public TransacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Method to get all transactions from the database (Not recommended for large databases)
    /// </summary>
    /// <returns>List of transactions</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<IEnumerable<TransacaoResourceDTO>>> GetTransacoes()
    {
        var result = await _mediator.Send(new GetAllTransacoesQuery());
            
        // Se estiver vazio, retornamos 404
        if (!result.Any())
        {
            return NotFound("Nenhuma transação encontrada");
        }

        // Adiciona os links
        var final = result.Select(r => 
        {
            r.AddLinks(Url);
            return r;
        });

        return Ok(final);
    }

    /// <summary>
    /// Get transactions paginated
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Page with transactions</returns>
    [HttpGet]
    [Route("paged")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult> GetTransacoesPaged(int page = 1, int pageSize = 10)
    {
        var (items, notFound) = await _mediator.Send(
            new GetTransacoesPagedQuery(page, pageSize));

        // Se a query sinalizar notFound ou retornar lista vazia, decida como tratar
        if (notFound || !items.Any())
        {
            return NotFound("Nenhuma transação encontrada");
        }

        var finalItems = items.Select(r =>
        {
            r.AddLinks(Url);
            return r;
        });

        var response = new
        {
            Page = page,
            PageSize = pageSize,
            Items = finalItems
        };

        return Ok(response);
    }


    /// <summary>
    /// Method to get a transaction by its Txid
    /// </summary>
    /// <param name="txid">Transaction ID</param>
    /// <returns>Returns the transaction with the given Txid</returns>
    [HttpGet]
    [Route("{txid}", Name = "GetTransacaoByTxid")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<TransacaoResourceDTO>> GetTransacaoByTxid(string txid)
    {
        var resource = await _mediator.Send(new GetTransacaoByTxidQuery(txid));
        if (resource == null)
        {
            return NotFound("Transação não encontrada");
        }

        // Adiciona links
        resource.AddLinks(Url);
        return Ok(resource);
    }

    /// <summary>
    /// Method to create a new transaction
    /// </summary>
    /// <param name="transacaoCreateRequestDto">Object with the transaction data</param>
    /// <remarks>
    /// Example of request:
    /// 
    /// POST api/transacoes
    /// ```json
    ///     {  
    ///         "valor": 100.00  
    ///     }
    ///  ```
    /// </remarks>>
    /// <returns>Object with the created transaction data</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<IActionResult> AddTransacao([FromBody] TransacaoCreateRequestDTO dto)
    {
        if (dto == null)
        {
            throw new BadRequestException("Transação inválida");
        }
        // Monta o comando e envia pro Mediator
        var command = new CreateTransactionCommand
        {
            Valor = dto.Valor
        };

        var transacaoResponse = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetTransacaoByTxid), new { txid = transacaoResponse.Txid }, transacaoResponse);
    }


    /// <summary>
    /// Method to update a transaction including payer and receiver data, by Txid
    /// </summary>
    /// <param name="txid">Transaction ID</param>
    /// <param name="transacaoDto">Object with the updated transaction data</param>
    /// <remarks>
    /// Example of request:
    ///     PUT api/transacoes/{txid}
    /// 
    ///     {
    ///         "txid": {txid},  
    ///         "pagadorNome": "João da Silva",  
    ///         "pagadorCpf": "39053344705",  
    ///         "pagadorBanco": "001",  
    ///         "pagadorAgencia": "1234",  
    ///         "pagadorConta": "1234567",  
    ///         "recebedorNome": "Maria Oliveira",  
    ///         "recebedorCpf": "84983149022",  
    ///         "recebedorBanco": "237",  
    ///         "recebedorAgencia": "5678",  
    ///         "recebedorConta": "7654321"  
    ///     }
    /// </remarks>
    /// <returns>Object with the updated transaction data</returns>
    [HttpPut("{txid}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> UpdateTransacao(string txid, [FromBody] TransacaoUpdateDTO dto)
    {
        var command = new UpdateTransactionCommand
        {
            Txid = txid,
            UpdateDto = dto
        };
        var resource = await _mediator.Send(command);
        // Gera o link para a transação atualizada
        resource.AddLinks(Url);

        return Ok(resource);
    }


    /// <summary>
    /// Method to delete a transaction by its Txid
    /// </summary>
    /// <param name="txid">Transaction ID</param>
    /// <returns>Object with the deleted transaction data</returns>
    [HttpDelete("{txid}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<IActionResult> DeleteTransacao(string txid)
    {
        var command = new DeleteTransactionCommand { Txid = txid };
        var transacaoDeleted = await _mediator.Send(command);
        return Ok(transacaoDeleted);
    }
}