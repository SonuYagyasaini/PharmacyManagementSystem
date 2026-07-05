using System.Diagnostics;
using System.Text;
using PharmacyManagement.Api.Infrastructure.Persistence;

namespace PharmacyManagement.Api.Infrastructure.Logging;

public static class RequestLoggingExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("RequestLogging");

            var stopwatch = Stopwatch.StartNew();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            var originalResponseBody = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            logger.LogInformation(
                "HTTP {Method} {Path} started. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();

                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;

                logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    context.TraceIdentifier);

                await SaveRequestResponseLogAsync(context, requestBody, responseText, stopwatch.ElapsedMilliseconds);
            }
        });
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0)
        {
            return null;
        }

        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return Truncate(body);
    }

    private static async Task SaveRequestResponseLogAsync(
        HttpContext context,
        string? requestBody,
        string? responseBody,
        long elapsedMilliseconds)
    {
        try
        {
            var dbContext = context.RequestServices.GetRequiredService<PharmacyDbContext>();
            var log = new RequestResponseLog(
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.Value,
                requestBody,
                context.Response.StatusCode,
                Truncate(responseBody),
                elapsedMilliseconds,
                context.Connection.RemoteIpAddress?.ToString(),
                context.Request.Headers.UserAgent.ToString());

            await dbContext.RequestResponseLogs.AddAsync(log);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("RequestLogging");

            logger.LogError(exception, "Failed to save request/response log");
        }
    }

    private static string? Truncate(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Length <= 4000
                ? value
                : value[..4000];
}
