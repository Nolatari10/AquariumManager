using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<Sale>> GetAllAsync(int tenantId);
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(int tenantId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Sale>> GetPagedAsync(int tenantId, int page, int pageSize);
    Task<int> GetCountAsync(int tenantId);
    Task AddAsync(Sale sale);
}
