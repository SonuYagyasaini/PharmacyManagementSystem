namespace PharmacyManagement.Api.Modules.Sales;

public sealed record CreateSaleRequest
{
    public Guid MedicineId { get; init; }
    public int Quantity { get; init; }
}

public sealed record SaleResponse
{
    public Guid Id { get; init; }
    public Guid MedicineId { get; init; }
    public string MedicineName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime SoldAtUtc { get; init; }
}
