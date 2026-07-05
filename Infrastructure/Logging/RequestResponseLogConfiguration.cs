using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyManagement.Api.Infrastructure.Logging;

public sealed class RequestResponseLogConfiguration : IEntityTypeConfiguration<RequestResponseLog>
{
    public void Configure(EntityTypeBuilder<RequestResponseLog> builder)
    {
        builder.ToTable("RequestResponseLogs");
        builder.HasKey(log => log.Id);

        builder.Property(log => log.TraceId).HasMaxLength(100).IsRequired();
        builder.Property(log => log.Method).HasMaxLength(20).IsRequired();
        builder.Property(log => log.Path).HasMaxLength(500).IsRequired();
        builder.Property(log => log.QueryString).HasMaxLength(1000);
        builder.Property(log => log.IpAddress).HasMaxLength(100);
        builder.Property(log => log.UserAgent).HasMaxLength(500);
    }
}
