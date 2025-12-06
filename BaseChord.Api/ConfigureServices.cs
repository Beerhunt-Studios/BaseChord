using BaseChord.Api.Configurations;
using BaseChord.Api.Middleware.ExceptionHandling;
using BaseChord.Api.Logging;
using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using BaseChord.Api.Converter;
using BaseChord.Api.Middleware.Logging;
using BaseChord.Application.Converter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace BaseChord.Api;

/// <summary>
/// Provides extension methods for configuring services and application middleware in an ASP.NET Core application.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Configures the base settings and integrations for the web host during application startup.
    /// </summary>
    /// <param name="builder">The web host builder used to configure the application host.</param>
    /// <param name="configuration">The configuration instance providing necessary settings for the web host.</param>
    public static void ConfigureBaseBuilder(this WebHostBuilder builder, IConfiguration configuration)
    {
        builder
            .UseSentry(o =>
            {
                o.Dsn = configuration["Sentry:Dsn"];
                o.ProfilesSampleRate = configuration.GetValue<double>("Sentry:SampleRate");
                o.TracesSampleRate = configuration.GetValue<double>("Sentry:SampleRate");
                o.Debug = configuration.GetValue<bool>("Sentry:Debug");
            });
    }
    
    
    /// <summary>
    /// Configures the middleware pipeline for the application, including exception handling, logging,
    /// Swagger documentation, routing, authentication, and authorization. Also ensures pending
    /// database migrations are applied at runtime.
    /// </summary>
    /// <param name="app">The application builder used to specify the middleware pipeline.</param>
    public static void ConfigureBaseApp(this IApplicationBuilder app)
    {
        app.UseCorrelationId();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<LoggingEnricherMiddleware>();
        app.UseSwaggerDocumentation();
        app.UseRouting();
        app.UseSentryTracing();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        using var scope = app.ApplicationServices.CreateScope();

        // Execute Migrations
        var dbContext = scope.ServiceProvider.GetRequiredService<ThreadSafeDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }

    /// <summary>
    /// Registers API-related services and configurations into the provided service collection, including
    /// authentication, health checks, controllers with custom JSON converters, Swagger documentation, and correlation ID middleware.
    /// </summary>
    /// <param name="services">The service collection to which API services and configurations will be added.</param>
    /// <param name="configuration">The configuration instance providing necessary settings for API configuration.</param>
    /// <returns>The updated service collection with API services configured.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultCorrelationId();
        services.AddHttpContextAccessor();
        services.AddMvc();
        services.AddOptions();
        services.AddHttpClient(string.Empty).AddCorrelationIdForwarding();

        services.AddSwagger();

        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new OptionalJsonConverterFactory());
            });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{configuration["Auth0:Domain"]}/";
                options.Audience = configuration["Auth0:Audience"];
                options.MapInboundClaims = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth0:Key"]!))
                };
            });

        return services;
    }
}
