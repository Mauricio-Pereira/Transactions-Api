using Transactions_Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Transactions_Api.Infrastructure.Infrastructure;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; } 

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .HasIndex(t => t.Txid)
            .IsUnique();
    }

        
}