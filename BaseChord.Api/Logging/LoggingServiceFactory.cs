using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BaseChord.Api.Logging
{
    public static class LoggingServiceFactory
    {
        public static void AddCustomizedLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
                {
                    config
                        .Enrich.FromLogContext()
                        .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Information))
                        .MinimumLevel.Override("CorrelationId", LogEventLevel.Error)
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "chorconnect.log"), rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                });
        }
    }
}