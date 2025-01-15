using System.Text.RegularExpressions;
using AutoMapper;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Exceptions;
using Transactions_Api.Shared.Utils;

namespace Transactions_Api.Application.Services;

public class TransacaoService : ITransacaoService
{
    
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ITxidGenerator _txidGenerator;

    public TransacaoService(IUnitOfWork uow, IMapper mapper, ITxidGenerator txidGenerator)
    {
        _uow = uow;
        _mapper = mapper;
        _txidGenerator = txidGenerator;
    }
    
    
    
    public async Task<IEnumerable<TransacaoResponseDTO>> GetAllAsync()
    {
        var transacoes = await _uow.TransacaoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TransacaoResponseDTO>>(transacoes);
    }
    
    public async Task<IEnumerable<TransacaoResponseDTO>> GetAllPaginatedAsync(int page, int pageSize)
    {
        var transacoes = await _uow.TransacaoRepository.GetAllPaginatedAsync(page, pageSize);
        return _mapper.Map<IEnumerable<TransacaoResponseDTO>>(transacoes);
    }

    public async Task<TransacaoResponseDTO> GetByTxidAsync(string txid)
    {
        var transacao = await _uow.TransacaoRepository.GetByTxidAsync(txid);
        return _mapper.Map<TransacaoResponseDTO>(transacao);
    }


    public async Task<TransacaoResponseCreateDTO> AddAsync(TransacaoCreateRequestDTO transacaoCreateRequestDto)
    {
        // Validate if the DTO is not null
        if (transacaoCreateRequestDto == null)
        {
            throw new BadRequestException("O objeto TransacaoCreateDTO não pode ser nulo.");
        }
        // Validate the Valor field
        if (transacaoCreateRequestDto.Valor <= 0)
        {
            throw new BadRequestException("O valor da transação deve ser maior que zero.");
        }
        
        
        // Validate the Descricao field
        var transacao = _mapper.Map<Transacao>(transacaoCreateRequestDto);

        // Generate the Txid
        transacao.Txid = _txidGenerator.GerarTxid();

        // Define the transaction date
        transacao.DataTransacao = DateTime.Now;

        // Add the transaction to the database
        await _uow.TransacaoRepository.AddAsync(transacao);
        
        // Map the transaction to the response DTO
        var transacaoResponse = _mapper.Map<TransacaoResponseCreateDTO>(transacao);

        return transacaoResponse;
    }



    public async Task<TransacaoResponseDTO> UpdateAsync(string txid, TransacaoUpdateDTO transacaoDto)
    {
        // Validate if the DTO is not null
        if (transacaoDto == null)
        {
            throw new BadRequestException("O objeto TransacaoUpdateDTO não pode ser nulo.");
        }
        // Validate if the transaction ID in the URL matches the transaction ID in the body
        if (txid != transacaoDto.Txid)
        {
            throw new BadRequestException("O Txid fornecido na URL não corresponde ao Txid do corpo da requisição.");
        }
        // Validate the Txid field
        if (!Regex.IsMatch(txid, "^[a-zA-Z0-9]{26,35}$"))
        {
            throw new BadRequestException("O Txid é inválido. Deve conter apenas caracteres alfanuméricos e ter entre 26 e 35 caracteres.");
        }
        // Validate if the transaction exists
        var transacaoExistente = await _uow.TransacaoRepository.GetByTxidAsync(txid);
        if (transacaoExistente == null)
        {
            throw new NotFoundException("Transação não encontrada.");
        }
        // Validate if the transaction has already been processed
        if (transacaoExistente.E2eId != null)
        {
            throw new BadRequestException("Não é possível alterar uma transação que já foi processada.");
        }
        transacaoExistente.E2eId = Helper.GerarEndToEndId(DateTime.UtcNow);

        _mapper.Map(transacaoDto, transacaoExistente);
        await _uow.TransacaoRepository.UpdateAsync(transacaoExistente);

        return _mapper.Map<TransacaoResponseDTO>(transacaoExistente);
    }

    public async Task<TransacaoResponseDTO> DeleteAsync(string txid)
    {
        // Validate if the transaction exists
        var transacaoToDelete = await _uow.TransacaoRepository.GetByTxidAsync(txid);
        if (transacaoToDelete == null)
            return null;
        await _uow.TransacaoRepository.DeleteAsync(txid);
        return _mapper.Map<TransacaoResponseDTO>(transacaoToDelete);
    }
    
    
    
}