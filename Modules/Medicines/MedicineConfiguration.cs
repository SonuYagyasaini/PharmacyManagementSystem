using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyManagement.Api.Modules.Medicines;

public sealed class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.ToTable("Medicines");
        builder.HasKey(medicine => medicine.Id);

        builder.Property(medicine => medicine.FullName).HasMaxLength(200).IsRequired();
        builder.Property(medicine => medicine.Notes).HasMaxLength(1000);
        builder.Property(medicine => medicine.Brand).HasMaxLength(150).IsRequired();
        builder.Property(medicine => medicine.Price).HasPrecision(18, 2);

        builder.Ignore(medicine => medicine.IsExpiringWithin30Days);
        builder.Ignore(medicine => medicine.IsLowStock);
        builder.Ignore(medicine => medicine.HighlightColor);

        builder.HasData(
            new
            {
                Id = Guid.Parse("2b7cd386-89dd-44b9-ac6e-e921a1ccb81d"),
                FullName = "Paracetamol 500mg Tablet",
                Notes = "Use after food. Keep away from children.",
                ExpiryDate = new DateOnly(2026, 7, 25),
                Quantity = 50,
                Price = 20.50m,
                Brand = "ABC Pharma",
                CreatedAtUtc = new DateTime(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = (DateTime?)null,
                IsDeleted = false,
                DeletedAtUtc = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("fa245e88-3244-426f-a0fa-b4c5bd6cd59f"),
                FullName = "Cough Syrup 100ml",
                Notes = "Shake well before use.",
                ExpiryDate = new DateOnly(2027, 2, 10),
                Quantity = 7,
                Price = 85.00m,
                Brand = "HealthCare",
                CreatedAtUtc = new DateTime(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = (DateTime?)null,
                IsDeleted = false,
                DeletedAtUtc = (DateTime?)null
            });
    }
}
