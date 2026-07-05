using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyManagement.Api.Infrastructure.Auditing;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(auditLog => auditLog.EntityId).HasMaxLength(100).IsRequired();
        builder.Property(auditLog => auditLog.Action).HasMaxLength(50).IsRequired();
        builder.Property(auditLog => auditLog.RequestPath).HasMaxLength(500);
        builder.Property(auditLog => auditLog.TraceId).HasMaxLength(100);
    }
}
