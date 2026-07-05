using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Api.Infrastructure.Persistence;

namespace PharmacyManagement.Api.Infrastructure.Logging;

public static class LoggingEndpoints
{
    public static IEndpointRouteBuilder MapLoggingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logs");

        group.MapGet("/requests", async (PharmacyDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var logs = await dbContext.RequestResponseLogs
                .AsNoTracking()
                .OrderByDescending(log => log.RequestedAtUtc)
                .Take(100)
                .ToListAsync(cancellationToken);

            return Results.Ok(logs);
        });

        group.MapGet("/audit", async (PharmacyDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var logs = await dbContext.AuditLogs
                .AsNoTracking()
                .OrderByDescending(log => log.OccurredAtUtc)
                .Take(100)
                .ToListAsync(cancellationToken);

            return Results.Ok(logs);
        });

        return app;
    }
}
