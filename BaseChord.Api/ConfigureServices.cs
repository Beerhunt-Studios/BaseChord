using BaseChord.Api.Configurations;
using BaseChord.Api.Middleware.ExceptionHandling;
using BaseChord.Api.Middleware.Logging;
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

namespace BaseChord.Api;

public static class ConfigureServices
{
    public static void ConfigureBaseApp(this IApplicationBuilder app)
    {
        app.UseCorrelationId();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwaggerDocumentation();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Execute Migrations
        var dbContext = app.ApplicationServices.GetRequiredService<DbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultCorrelationId();
        services.AddHttpContextAccessor();
        services.AddMvc();
        services.AddOptions();
        services.AddHttpClient(string.Empty).AddCorrelationIdForwarding();

        services.AddSwagger();
        services.AddCustomizedLogging();

        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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
