using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BaseChord.Api.Middleware.Logging
{
    public static class LoggingServiceFactory
    {
        public static IServiceCollection AddCustomizedLogging(this IServiceCollection sc)
        {
            sc.AddSerilog((service, conf) =>
            {
                conf
                .Enrich.FromLogContext()
                .Enrich.With(new HttpEnricher(service.GetRequiredService<IHttpContextAccessor>(), service.GetRequiredService<ICorrelationContextAccessor>()))
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Information))
                .MinimumLevel.Override("CorrelationId", LogEventLevel.Error)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "chorconnect.log"), rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            });

            return sc;
        }
    }
}