using System.Reflection;
using BaseChord.Application.Database;
using BaseChord.Infrastructure.Database;
using BaseChord.IntegrationTests.Seeder;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Microsoft.Extensions.DependencyInjection;

namespace BaseChord.IntegrationTests;

public static class ConfigureService
{
    public static void AddInfrastructureTestServices<TDbContext>(this IServiceCollection services, params Assembly[] assemblies)
        where TDbContext : ThreadSafeDbContext
    {
        services.AddDbContext<TDbContext>(options =>
        {
            options
                .UseInMemoryDatabase("InMemoryDbForTesting")
                .ConfigureWarnings(x => 
                    x.Ignore(InMemoryEventId.TransactionIgnoredWarning)
                );
        });
        services.AddScoped<ThreadSafeDbContext>(x => x.GetRequiredService<TDbContext>());
        
        services.AddSingleton<DbTransactionLock>();
        services.AddScoped<IDbTransactionFactory, DbTransactionFactory>();
        
        services.AddMassTransitTestHarness(options => options.AddConsumers(assemblies));
        
        services.AddSeeders(assemblies);
    }

    private static void AddSeeders(this IServiceCollection services, params Assembly[] assemblies)
    {
        var seederTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetInterfaces().Any(inf => inf == typeof(ISeeder)));

        foreach (var seederType in seederTypes)
        {
            services.AddScoped(typeof(ISeeder), seederType);
        }

        services.AddScoped<SeedManager>();
    }
}