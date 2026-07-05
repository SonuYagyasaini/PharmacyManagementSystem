using PharmacyManagement.Api.Infrastructure.Exceptions;
using PharmacyManagement.Api.Infrastructure.Auditing;
using PharmacyManagement.Api.Modules.Medicines;

namespace PharmacyManagement.Api.Modules.Sales;

public sealed class SaleService(
    ISaleRepository saleRepository,
    IMedicineRepository medicineRepository,
    AuditLogService auditLogService,
    ILogger<SaleService> logger)
{
    public async Task<IReadOnlyList<SaleResponse>> GetSalesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching sales");

        var sales = await saleRepository.GetAllAsync(cancellationToken);
        return sales.Select(ToResponse).ToList();
    }

    public async Task<SaleResponse> CreateSaleAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new BadRequestException("Sale quantity must be greater than zero.");
        }

        var medicine = await medicineRepository.GetByIdAsync(request.MedicineId, trackChanges: true, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Medicine was not found.");

        var oldMedicineValues = new
        {
            medicine.Id,
            medicine.FullName,
            medicine.Quantity
        };

        try
        {
            medicine.ReduceStock(request.Quantity);
        }
        catch (InvalidOperationException exception)
        {
            throw new BadRequestException(exception.Message);
        }

        var sale = new Sale(medicine, request.Quantity);
        await saleRepository.AddAsync(sale, cancellationToken);
        await auditLogService.LogAsync(
            nameof(Sale),
            sale.Id.ToString(),
            AuditAction.Created,
            null,
            ToAuditSnapshot(sale),
            cancellationToken);
        await auditLogService.LogAsync(
            nameof(Medicine),
            medicine.Id.ToString(),
            AuditAction.Updated,
            oldMedicineValues,
            new
            {
                medicine.Id,
                medicine.FullName,
                medicine.Quantity
            },
            cancellationToken);

        logger.LogInformation(
            "Sale {SaleId} created for medicine {MedicineId}",
            sale.Id,
            medicine.Id);

        return ToResponse(sale);
    }

    private static SaleResponse ToResponse(Sale sale)
        => new SaleResponse
        {
            Id = sale.Id,
            MedicineId = sale.MedicineId,
            MedicineName = sale.MedicineName,
            Quantity = sale.Quantity,
            UnitPrice = sale.UnitPrice,
            TotalAmount = sale.TotalAmount,
            SoldAtUtc = sale.SoldAtUtc
        };

    private static object ToAuditSnapshot(Sale sale)
        => new
        {
            sale.Id,
            sale.MedicineId,
            sale.MedicineName,
            sale.Quantity,
            sale.UnitPrice,
            sale.TotalAmount,
            sale.SoldAtUtc
        };
}
