using LeL.ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeL.ExpenseTracker.Api.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExpenseDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString(ExpenseDbContext.ConnectionStringName), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(3));
        });

        return services;
    }
}
