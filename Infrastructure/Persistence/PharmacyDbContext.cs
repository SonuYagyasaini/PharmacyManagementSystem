using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Api.Infrastructure.Auditing;
using PharmacyManagement.Api.Infrastructure.Logging;
using PharmacyManagement.Api.Modules.Medicines;
using PharmacyManagement.Api.Modules.Sales;

namespace PharmacyManagement.Api.Infrastructure.Persistence;

public sealed class PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : DbContext(options)
{
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RequestResponseLog> RequestResponseLogs => Set<RequestResponseLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MedicineConfiguration());
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new RequestResponseLogConfiguration());
    }
}
