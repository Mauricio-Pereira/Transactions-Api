using AutoMapper;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Domain.Models;

namespace Transactions_Api.Application.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Create mapping between TransacaoCreateDTO and Transacao
        CreateMap<TransacaoCreateRequestDTO, Transacao>();

        // Create mapping between Transacao and TransacaoResponseCreateDTO
        CreateMap<Transacao, TransacaoResponseCreateDTO>();

        // Create mapping between TransacaoUpdateDTO and Transacao, ignoring Txid
        CreateMap<TransacaoUpdateDTO, Transacao>()
            .ForMember(dest => dest.Txid, opt => opt.Ignore())
            .ForMember(dest => dest.DataTransacao, opt => opt.Ignore())
            .ForMember(dest => dest.Valor, opt => opt.Ignore());
        
        // Create mapping between Transacao and TransacaoResponseDTO
        CreateMap<Transacao, TransacaoResponseDTO>();
    }
}