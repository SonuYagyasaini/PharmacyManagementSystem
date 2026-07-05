namespace PharmacyManagement.Api.Modules.Medicines;

public sealed record CreateMedicineRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateOnly ExpiryDate { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public string Brand { get; init; } = string.Empty;
}

public sealed record UpdateMedicineRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateOnly ExpiryDate { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public string Brand { get; init; } = string.Empty;
}

public sealed record MedicineResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateOnly ExpiryDate { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public string Brand { get; init; } = string.Empty;
    public bool IsExpiringWithin30Days { get; init; }
    public bool IsLowStock { get; init; }
    public string HighlightColor { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
