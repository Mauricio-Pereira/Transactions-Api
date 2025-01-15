using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transactions_Api.Domain.Models
{
    [Table("apikeys")]
    public class ApiKey
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        [Column("apikey")]
        public string Key { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [MaxLength(14)]
        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("conta")]
        public string Conta { get; set; }
    }
}