namespace PharmacyManagement.Api.Modules.Medicines;

public sealed record CreateMedicineRequest(
    string FullName,
    string? Notes,
    DateOnly ExpiryDate,
    int Quantity,
    decimal Price,
    string Brand);

public sealed record UpdateMedicineRequest(
    string FullName,
    string? Notes,
    DateOnly ExpiryDate,
    int Quantity,
    decimal Price,
    string Brand);

public sealed record MedicineResponse(
    Guid Id,
    string FullName,
    string Notes,
    DateOnly ExpiryDate,
    int Quantity,
    decimal Price,
    string Brand,
    bool IsExpiringWithin30Days,
    bool IsLowStock,
    string HighlightColor,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
