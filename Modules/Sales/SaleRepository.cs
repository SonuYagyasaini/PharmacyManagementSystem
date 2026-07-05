using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Api.Infrastructure.Persistence;

namespace PharmacyManagement.Api.Modules.Sales;

public interface ISaleRepository
{
    Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
}

public sealed class SaleRepository(PharmacyDbContext dbContext) : ISaleRepository
{
    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Sales
            .AsNoTracking()
            .OrderByDescending(sale => sale.SoldAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await dbContext.Sales.AddAsync(sale, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
