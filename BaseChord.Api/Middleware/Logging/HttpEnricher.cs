using System.Collections.Generic;
using System.Linq;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace BaseChord.Api.Middleware.Logging
{
    public class HttpEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public HttpEnricher(
            IHttpContextAccessor contextAccessor,
            ICorrelationContextAccessor correlationContextAccessor)
        {
            _contextAccessor = contextAccessor;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            HttpRequest? request = _contextAccessor.HttpContext?.Request;
            var context = ContextInfo(_correlationContextAccessor);
            var properties = context.Select(kvp => propertyFactory.CreateProperty(kvp.Key, kvp.Value));
            foreach (LogEventProperty prop in properties)
            {
                logEvent.AddPropertyIfAbsent(prop);
            }
        }

        public static IEnumerable<KeyValuePair<string, string?>> ContextInfo(ICorrelationContextAccessor correlationContextAccessor) =>
            new Dictionary<string, string?>
            {
                {
                    "CorrelationId", correlationContextAccessor?.CorrelationContext?.CorrelationId
                }

            };
    }
}