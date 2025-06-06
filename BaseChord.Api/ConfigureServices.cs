﻿using BaseChord.Api.Configurations;
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

namespace BaseChord.Api;

public static class ConfigureServices
{
    public static void ConfigureBaseApp(this IApplicationBuilder app)
    {
        app.UseCorrelationId();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<LoggingEnricherMiddleware>();
        app.UseSwaggerDocumentation();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        using var scope = app.ApplicationServices.CreateScope();

        // Execute Migrations
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
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

        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
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
