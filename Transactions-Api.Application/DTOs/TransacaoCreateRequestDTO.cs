using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transactions_Api.Application.DTOs;

public class TransacaoCreateRequestDTO
{
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

}