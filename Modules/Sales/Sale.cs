using PharmacyManagement.Api.Modules.Medicines;

namespace PharmacyManagement.Api.Modules.Sales;

public sealed class Sale
{
    public Guid Id { get; private set; }
    public Guid MedicineId { get; private set; }
    public string MedicineName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime SoldAtUtc { get; private set; }
    public Medicine? Medicine { get; private set; }

    private Sale()
    {
    }

    public Sale(Medicine medicine, int quantity)
    {
        Id = Guid.NewGuid();
        MedicineId = medicine.Id;
        MedicineName = medicine.FullName;
        Quantity = quantity;
        UnitPrice = medicine.Price;
        TotalAmount = decimal.Round(quantity * medicine.Price, 2);
        SoldAtUtc = DateTime.UtcNow;
    }
}
