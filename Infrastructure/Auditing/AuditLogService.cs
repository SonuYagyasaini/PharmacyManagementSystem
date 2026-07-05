using System.Text.Json;
using PharmacyManagement.Api.Infrastructure.Persistence;

namespace PharmacyManagement.Api.Infrastructure.Auditing;

public sealed class AuditLogService(PharmacyDbContext dbContext, IHttpContextAccessor? httpContextAccessor = null)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task LogAsync(
        string entityName,
        string entityId,
        string action,
        object? oldValues,
        object? newValues,
        CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor?.HttpContext;

        var auditLog = new AuditLog(
            entityName,
            entityId,
            action,
            oldValues is null ? null : JsonSerializer.Serialize(oldValues, JsonOptions),
            newValues is null ? null : JsonSerializer.Serialize(newValues, JsonOptions),
            httpContext?.Request.Path.Value,
            httpContext?.TraceIdentifier);

        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
