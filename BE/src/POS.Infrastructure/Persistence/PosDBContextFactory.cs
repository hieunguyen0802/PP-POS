using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace POS.Infrastructure.Persistence;

public class PosDBContextFactory : IDesignTimeDbContextFactory<PosDBContext>
{
    public PosDBContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PosDBContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("PosDbConnection"));

        return new PosDBContext(optionsBuilder.Options);
    }
}