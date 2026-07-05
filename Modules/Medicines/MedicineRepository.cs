using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Api.Infrastructure.Persistence;

namespace PharmacyManagement.Api.Modules.Medicines;

public interface IMedicineRepository
{
    Task<IReadOnlyList<Medicine>> GetAllAsync(string? search, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<Medicine?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task AddAsync(Medicine medicine, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed class MedicineRepository(PharmacyDbContext dbContext) : IMedicineRepository
{
    public async Task<IReadOnlyList<Medicine>> GetAllAsync(
        string? search,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var query = trackChanges ? dbContext.Medicines : dbContext.Medicines.AsNoTracking();
        query = query.Where(medicine => !medicine.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(medicine => medicine.FullName.Contains(search));
        }

        return await query.OrderBy(medicine => medicine.FullName).ToListAsync(cancellationToken);
    }

    public Task<Medicine?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        var query = trackChanges ? dbContext.Medicines : dbContext.Medicines.AsNoTracking();
        return query.FirstOrDefaultAsync(medicine => medicine.Id == id && !medicine.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Medicine medicine, CancellationToken cancellationToken = default)
    {
        await dbContext.Medicines.AddAsync(medicine, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
