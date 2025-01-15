namespace Transactions_Api.Infrastructure.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=transactionsapi;User=root;Password=C@verinha.p4;",
            new MySqlServerVersion(new Version(8, 0, 25))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
