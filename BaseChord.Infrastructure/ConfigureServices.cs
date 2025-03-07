using BaseChord.Application.Database;
using BaseChord.Infrastructure.Database;
using EFCoreSecondLevelCacheInterceptor;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaseChord.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : BaseDbContext
    {
        services.AddDatabaseInfrastructureServices<TDbContext>(configuration);
        services.AddDatabaseServices();
        services.AddEventbusInfrastructureServices<TDbContext>(configuration);

        return services;
    }

    public static IServiceCollection AddDatabaseInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : BaseDbContext
    {
        services.AddEFSecondLevelCache(
                options => options.UseMemoryCacheProvider().ConfigureLogging(true).UseCacheKeyPrefix("EF_")
            );

        services.AddDbContext<TDbContext>(options =>
        {
            options
            .UseSqlServer(configuration["Database:ConnectionString"], opt => opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        services.AddScoped<DbContext>(x => x.GetRequiredService<TDbContext>());

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddScoped<IDbTransactionFactory, DbTransactionFactory>();

        return services;
    }

    public static IServiceCollection AddEventbusInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : BaseDbContext
    {
        services.AddMassTransit(options =>
        {
            options.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UseMySql();
                o.UseBusOutbox();
            });

            options.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["Eventbus:Host"] ?? throw new NullReferenceException("The host has not been specififed for the Eventbus"), x =>
                {
                    x.Username(configuration["Eventbus:Username"] ?? throw new NullReferenceException("The username has not been specififed for the Eventbus"));
                    x.Password(configuration["Eventbus:Password"] ?? throw new NullReferenceException("The password has not been specififed for the Eventbus"));
                });
            });

            options.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());
            options.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                cfg.UseMessageRetry(r => r.Immediate(5));
                cfg.UseEntityFrameworkOutbox<TDbContext>(context);
            });
        });

        return services;
    }
}
