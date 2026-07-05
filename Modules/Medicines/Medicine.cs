namespace PharmacyManagement.Api.Modules.Medicines;

public sealed class Medicine
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateOnly ExpiryDate { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public string Brand { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private Medicine()
    {
    }

    public Medicine(string fullName, string notes, DateOnly expiryDate, int quantity, decimal price, string brand)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Notes = notes;
        ExpiryDate = expiryDate;
        Quantity = quantity;
        Price = decimal.Round(price, 2);
        Brand = brand;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public bool IsExpiringWithin30Days
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return ExpiryDate <= today.AddDays(30);
        }
    }

    public bool IsLowStock => Quantity < 10;

    public string HighlightColor => IsExpiringWithin30Days
        ? "red"
        : IsLowStock
            ? "yellow"
            : "none";

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        if (Quantity < quantity)
        {
            throw new InvalidOperationException("Insufficient stock.");
        }

        Quantity -= quantity;
    }

    public void Update(string fullName, string notes, DateOnly expiryDate, int quantity, decimal price, string brand)
    {
        FullName = fullName;
        Notes = notes;
        ExpiryDate = expiryDate;
        Quantity = quantity;
        Price = decimal.Round(price, 2);
        Brand = brand;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}
