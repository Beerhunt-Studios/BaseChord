using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace BaseChord.Api.Middleware.Logging;

internal class LoggingEnricherMiddleware
{
    private readonly ICorrelationContextAccessor _correlationContextAccessor;
    // https://stackoverflow.com/a/38935583/856692
    private readonly RequestDelegate _next;

    public LoggingEnricherMiddleware(RequestDelegate next, ICorrelationContextAccessor correlationContextAccessor)
    {
        _next = next;
        _correlationContextAccessor = correlationContextAccessor;
    }


    public async Task Invoke(HttpContext context)
    {
        HttpRequest? request = context.Request;
        var loggingContext = ContextInfo(_correlationContextAccessor);
        foreach (var prop in loggingContext)
        {
            LogContext.PushProperty(prop.Key, prop.Value);
        }

        await _next(context);
    }

    public static IEnumerable<KeyValuePair<string, string?>> ContextInfo(ICorrelationContextAccessor correlationContextAccessor) =>
        new Dictionary<string, string?>
        {
                {
                    "CorrelationId", correlationContextAccessor?.CorrelationContext?.CorrelationId
                }
        };
}
