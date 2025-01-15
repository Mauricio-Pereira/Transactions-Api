using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transactions_Api.Domain.Models;

[Table("transacoes")]
public class Transacao
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(35)]
    [Column("txid")]
    public string Txid { get; set; }

    [MaxLength(64)]
    [Column("e2e_id")]
    public string? E2eId { get; set; }

    // Dados do Pagador
    [MaxLength(100)]
    [Column("pagador_nome")]
    public string? PagadorNome { get; set; }

    [MaxLength(11)]
    [Column("pagador_documento")]
    public string? PagadorCpf { get; set; }

    [MaxLength(8)]
    [Column("pagador_banco")]
    public string? PagadorBanco { get; set; }

    [MaxLength(6)]
    [Column("pagador_agencia")]
    public string? PagadorAgencia { get; set; }

    [MaxLength(10)]
    [Column("pagador_conta")]
    public string? PagadorConta { get; set; }

    // Dados do Recebedor
    [MaxLength(100)]
    [Column("recebedor_nome")]
    public string? RecebedorNome { get; set; }

    [MaxLength(11)]
    [Column("recebedor_documento")]
    public string? RecebedorCpf { get; set; }

    [MaxLength(8)]
    [Column("recebedor_banco")]
    public string? RecebedorBanco { get; set; }

    [MaxLength(6)]
    [Column("recebedor_agencia")]
    public string? RecebedorAgencia { get; set; }

    [MaxLength(10)]
    [Column("recebedor_conta")]
    public string? RecebedorConta { get; set; }

    [Required]
    [Column("valor", TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    [Required]
    [Column("data_transacao", TypeName = "timestamp")]
    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
    
    
    
}