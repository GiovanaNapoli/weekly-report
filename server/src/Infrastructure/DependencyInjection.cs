using Application.Interfaces.Repositories;
using Infrastructure.Connector;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddRepositories();

        services.AddScoped<DbConnector>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Aponta direto para o assembly da Infrastructure — determinístico
        var assembly = typeof(DependencyInjection).Assembly;

        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Repository") && t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var implementationType in repositoryTypes)
        {
            var interfaceType = implementationType.GetInterface($"I{implementationType.Name}");

            if (interfaceType != null)
                services.AddScoped(interfaceType, implementationType);
        }

        return services;
    }
}
