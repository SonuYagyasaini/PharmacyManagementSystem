namespace PharmacyManagement.Api.Infrastructure.Auditing;

public sealed class AuditLog
{
    public Guid Id { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? RequestPath { get; private set; }
    public string? TraceId { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        string entityName,
        string entityId,
        string action,
        string? oldValues,
        string? newValues,
        string? requestPath,
        string? traceId)
    {
        Id = Guid.NewGuid();
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        OldValues = oldValues;
        NewValues = newValues;
        RequestPath = requestPath;
        TraceId = traceId;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
