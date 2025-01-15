using System.ComponentModel.DataAnnotations;

namespace Transactions_Api.Application.DTOs;

public class TransacaoUpdateDTO
{
    [Required] [MaxLength(64)] public string Txid { get; set; }

    // Dados do Pagador
    [Required] [MaxLength(100)] public string PagadorNome { get; set; }

    [Required] [MaxLength(11)] public string PagadorCpf { get; set; }

    [Required] [MaxLength(8)] public string PagadorBanco { get; set; }

    [Required] [MaxLength(6)] public string PagadorAgencia { get; set; }

    [Required] [MaxLength(10)] public string PagadorConta { get; set; }

    // Dados do Recebedor
    [Required] [MaxLength(100)] public string RecebedorNome { get; set; }

    [Required] [MaxLength(11)] public string RecebedorCpf { get; set; }

    [Required] [MaxLength(8)] public string RecebedorBanco { get; set; }

    [Required] [MaxLength(6)] public string RecebedorAgencia { get; set; }

    [Required] [MaxLength(10)] public string RecebedorConta { get; set; }
}