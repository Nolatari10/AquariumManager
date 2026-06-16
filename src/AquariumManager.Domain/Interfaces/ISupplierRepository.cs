using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<Supplier>> GetAllAsync(int tenantId);
    Task<IReadOnlyList<Supplier>> GetAllWithLotsAsync(int tenantId);
    Task AddAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(int id);
}
