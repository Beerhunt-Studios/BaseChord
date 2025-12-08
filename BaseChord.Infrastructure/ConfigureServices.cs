using BaseChord.Application.Database;
using BaseChord.Infrastructure.Database;
using EFCoreSecondLevelCacheInterceptor;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace BaseChord.Infrastructure;

/// <summary>
/// Provides extension methods for configuring and adding infrastructure services
/// including database connectivity, event bus integration, and caching services.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// This includes database infrastructure, database-related services,
    /// event bus integration, and caching mechanisms.
    /// </summary>
    /// <typeparam name="TDbContext">
    /// The type of the database context, which must inherit from ThreadSafeDbContext.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The application's configuration data, typically used
    /// to retrieve connection strings or other settings.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies containing message consumers used by the event bus.
    /// </param>
    /// <returns>
    /// The modified <see cref="IServiceCollection"/> with the added services.
    /// </returns>
    public static IServiceCollection AddInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        where TDbContext : ThreadSafeDbContext
    {
        services.AddDatabaseInfrastructureServices<TDbContext>(configuration);
        services.AddDatabaseServices();
        services.AddEventbusInfrastructureServices<TDbContext>(configuration, assemblies);

        return services;
    }

    /// <summary>
    /// Configures and adds database infrastructure services to the dependency injection container.
    /// This includes setting up the database context, enabling EF Core second-level caching,
    /// and configuring database-related options such as query splitting behavior.
    /// </summary>
    /// <typeparam name="TDbContext">
    /// The type of the database context, which must inherit from ThreadSafeDbContext.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The application's configuration data, typically used to retrieve the database connection string,
    /// caching configurations, and other related settings.
    /// </param>
    /// <returns>
    /// The modified <see cref="IServiceCollection"/> with the added database infrastructure services.
    /// </returns>
    public static IServiceCollection AddDatabaseInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext : ThreadSafeDbContext
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
        services.AddScoped<ThreadSafeDbContext>(x => x.GetRequiredService<TDbContext>());

        return services;
    }

    /// <summary>
    /// Adds database-related services to the dependency injection container.
    /// This includes services for database transaction management and locking.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The modified <see cref="IServiceCollection"/> with the added database services.
    /// </returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddSingleton<DbTransactionLock>();
        services.AddScoped<IDbTransactionFactory, DbTransactionFactory>();

        return services;
    }

    /// <summary>
    /// Configures and adds event bus infrastructure services to the dependency injection container.
    /// This includes setting up MassTransit with RabbitMQ, configuring an Entity Framework outbox,
    /// adding message consumers, and defining retry and redelivery policies.
    /// </summary>
    /// <typeparam name="TDbContext">
    /// The type of the database context, which must inherit from <see cref="DbContext"/>.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the event bus infrastructure services will be added.
    /// </param>
    /// <param name="configuration">
    /// The application's configuration, typically used for retrieving settings such as the Eventbus host.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies containing message consumers to register with MassTransit.
    /// </param>
    /// <returns>
    /// The modified <see cref="IServiceCollection"/> with the added event bus infrastructure services.
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if a required configuration setting, such as the Eventbus host, is not specified.
    /// </exception>
    public static IServiceCollection AddEventbusInfrastructureServices<TDbContext>(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        where TDbContext : DbContext
    {
        services.AddMassTransit(options =>
        {
            options.AddConsumers(assemblies);
            options.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                cfg.UseMessageRetry(r => r.Immediate(5));
                cfg.UseEntityFrameworkOutbox<TDbContext>(context);
            });
            
            options.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            options.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["Eventbus:Host"] ?? throw new NullReferenceException("The host has not been specififed for the Eventbus"), x =>
                {
                    x.Username(configuration["Eventbus:Username"] ?? throw new NullReferenceException("The username has not been specififed for the Eventbus"));
                    x.Password(configuration["Eventbus:Password"] ?? throw new NullReferenceException("The password has not been specififed for the Eventbus"));
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
