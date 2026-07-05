using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagement.Api.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, safeMessage) = exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DbUpdateException => (StatusCodes.Status409Conflict, "Database operation failed."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        logger.LogError(
            exception,
            "Request failed with status {StatusCode}. TraceId: {TraceId}",
            statusCode,
            context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ApiErrorResponse(
            statusCode,
            environment.IsDevelopment() ? exception.Message : safeMessage,
            Activity.Current?.Id ?? context.TraceIdentifier);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
