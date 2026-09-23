using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Infrastructure.Persistence;

namespace POS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PosDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PosDbConnection"),
            providerOptions => providerOptions.EnableRetryOnFailure()));
        return services;
    }

}

