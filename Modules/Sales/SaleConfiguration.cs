using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyManagement.Api.Modules.Sales;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(sale => sale.Id);

        builder.Property(sale => sale.MedicineName).HasMaxLength(200).IsRequired();
        builder.Property(sale => sale.UnitPrice).HasPrecision(18, 2);
        builder.Property(sale => sale.TotalAmount).HasPrecision(18, 2);

        builder.HasOne(sale => sale.Medicine)
            .WithMany()
            .HasForeignKey(sale => sale.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
