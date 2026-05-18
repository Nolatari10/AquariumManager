using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id);
    Task<IReadOnlyList<Sale>> GetAllAsync();
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Sale>> GetPagedAsync(int page, int pageSize);
    Task<int> GetCountAsync();
    Task AddAsync(Sale sale);
}
