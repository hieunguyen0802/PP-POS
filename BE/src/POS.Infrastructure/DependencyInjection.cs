using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;
using POS.Infrastructure.Repositories;
using POS.Infrastructure.Security;
using POS.Application.Common.Interfaces;

namespace POS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PosDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PosDbConnection"),
            providerOptions => providerOptions.EnableRetryOnFailure()));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();


        return services;
    }

}

