namespace PharmacyManagement.Api.Modules.Sales;

public sealed record CreateSaleRequest(Guid MedicineId, int Quantity);

public sealed record SaleResponse(
    Guid Id,
    Guid MedicineId,
    string MedicineName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    DateTime SoldAtUtc);
