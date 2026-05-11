using System;
using System.Net;
using System.Text.Json;

namespace EcomApi.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? HttpStatusCode.NotFound
            : HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { statusCode = (int)statusCode, message = ex.Message };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
