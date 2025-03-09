using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BaseChord.Api.Middleware.ExceptionHandling
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        // https://stackoverflow.com/a/38935583/856692
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected
            if (exception is ValidationException)
            {
                code = HttpStatusCode.BadRequest;
            }

            context.Response.StatusCode = (int)code;

            var result = "An error has occurred";

            return context.Response.WriteAsync(result);
        }
    }
}