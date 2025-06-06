﻿using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BaseChord.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BaseChord.Api.Middleware.ExceptionHandling;

public class ExceptionMiddleware
{
    private record Response
    {
        public required string Message { get; set; }
        
        public IEnumerable<ValidationFailure>? ValidationErrors { get; set; }
    }
    
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

        Response result = new Response {
            Message = exception.Message
        };
        HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected
        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result.ValidationErrors = validationException.Errors;

                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;

                break;
        }

        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsJsonAsync(result);
    }
}