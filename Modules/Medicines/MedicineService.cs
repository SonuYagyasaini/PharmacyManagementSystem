using PharmacyManagement.Api.Infrastructure.Exceptions;
using PharmacyManagement.Api.Infrastructure.Auditing;

namespace PharmacyManagement.Api.Modules.Medicines;

public sealed class MedicineService(
    IMedicineRepository repository,
    AuditLogService auditLogService,
    ILogger<MedicineService> logger)
{
    public async Task<IReadOnlyList<MedicineResponse>> GetMedicinesAsync(
        string? search,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching medicines. Search: {Search}, SortBy: {SortBy}, SortDirection: {SortDirection}", search, sortBy, sortDirection);

        var medicines = await repository.GetAllAsync(search, cancellationToken: cancellationToken);

        var normalizedSortBy = (sortBy ?? "createdAtUtc").Trim().ToLowerInvariant();
        var normalizedDirection = (sortDirection ?? "desc").Trim().ToLowerInvariant();

        var sortedMedicines = normalizedSortBy switch
        {
            "fullname" or "name" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.FullName)
                : medicines.OrderByDescending(m => m.FullName),
            "brand" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.Brand)
                : medicines.OrderByDescending(m => m.Brand),
            "quantity" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.Quantity)
                : medicines.OrderByDescending(m => m.Quantity),
            "price" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.Price)
                : medicines.OrderByDescending(m => m.Price),
            "expirydate" or "expiry" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.ExpiryDate)
                : medicines.OrderByDescending(m => m.ExpiryDate),
            "status" or "stockage" => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.ExpiryDate).ThenBy(m => m.Quantity)
                : medicines.OrderByDescending(m => m.ExpiryDate).ThenByDescending(m => m.Quantity),
            _ => normalizedDirection == "asc"
                ? medicines.OrderBy(m => m.CreatedAtUtc)
                : medicines.OrderByDescending(m => m.CreatedAtUtc)
        };

        return sortedMedicines.Select(ToResponse).ToList();
    }

    public async Task<MedicineResponse> GetMedicineAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching medicine {MedicineId}", id);

        var medicine = await repository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Medicine was not found.");

        return ToResponse(medicine);
    }

    public async Task<MedicineResponse> AddMedicineAsync(
        CreateMedicineRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var medicine = new Medicine(
            request.FullName.Trim(),
            request.Notes?.Trim() ?? string.Empty,
            request.ExpiryDate,
            request.Quantity,
            request.Price,
            request.Brand.Trim());

        await repository.AddAsync(medicine, cancellationToken);
        await auditLogService.LogAsync(
            nameof(Medicine),
            medicine.Id.ToString(),
            AuditAction.Created,
            null,
            ToAuditSnapshot(medicine),
            cancellationToken);

        logger.LogInformation("Medicine {MedicineId} created", medicine.Id);

        return ToResponse(medicine);
    }

    public async Task<MedicineResponse> UpdateMedicineAsync(
        Guid id,
        UpdateMedicineRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var medicine = await repository.GetByIdAsync(id, trackChanges: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Medicine was not found.");

        var oldValues = ToAuditSnapshot(medicine);

        medicine.Update(
            request.FullName.Trim(),
            request.Notes?.Trim() ?? string.Empty,
            request.ExpiryDate,
            request.Quantity,
            request.Price,
            request.Brand.Trim());

        await repository.SaveChangesAsync(cancellationToken);
        await auditLogService.LogAsync(
            nameof(Medicine),
            medicine.Id.ToString(),
            AuditAction.Updated,
            oldValues,
            ToAuditSnapshot(medicine),
            cancellationToken);

        logger.LogInformation("Medicine {MedicineId} updated", medicine.Id);

        return ToResponse(medicine);
    }

    public async Task DeleteMedicineAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var medicine = await repository.GetByIdAsync(id, trackChanges: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Medicine was not found.");

        var oldValues = ToAuditSnapshot(medicine);
        medicine.Delete();

        await repository.SaveChangesAsync(cancellationToken);
        await auditLogService.LogAsync(
            nameof(Medicine),
            medicine.Id.ToString(),
            AuditAction.Deleted,
            oldValues,
            ToAuditSnapshot(medicine),
            cancellationToken);

        logger.LogInformation("Medicine {MedicineId} soft deleted", medicine.Id);
    }

    private static void Validate(CreateMedicineRequest request)
        => ValidateMedicineValues(request.FullName, request.Brand, request.Quantity, request.Price);

    private static void Validate(UpdateMedicineRequest request)
        => ValidateMedicineValues(request.FullName, request.Brand, request.Quantity, request.Price);

    private static void ValidateMedicineValues(string fullName, string brand, int quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BadRequestException("Medicine full name is required.");
        }

        if (string.IsNullOrWhiteSpace(brand))
        {
            throw new BadRequestException("Medicine brand is required.");
        }

        if (quantity < 0)
        {
            throw new BadRequestException("Quantity cannot be negative.");
        }

        if (price <= 0)
        {
            throw new BadRequestException("Price must be greater than zero.");
        }
    }

    private static MedicineResponse ToResponse(Medicine medicine)
        => new MedicineResponse
        {
            Id = medicine.Id,
            FullName = medicine.FullName,
            Notes = medicine.Notes,
            ExpiryDate = medicine.ExpiryDate,
            Quantity = medicine.Quantity,
            Price = medicine.Price,
            Brand = medicine.Brand,
            IsExpiringWithin30Days = medicine.IsExpiringWithin30Days,
            IsLowStock = medicine.IsLowStock,
            HighlightColor = medicine.HighlightColor,
            CreatedAtUtc = medicine.CreatedAtUtc,
            UpdatedAtUtc = medicine.UpdatedAtUtc
        };

    private static object ToAuditSnapshot(Medicine medicine)
        => new
        {
            medicine.Id,
            medicine.FullName,
            medicine.Notes,
            medicine.ExpiryDate,
            medicine.Quantity,
            medicine.Price,
            medicine.Brand,
            medicine.CreatedAtUtc,
            medicine.UpdatedAtUtc,
            medicine.IsDeleted,
            medicine.DeletedAtUtc
        };
}
