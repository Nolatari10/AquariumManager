using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISpeciesRepository
{
    Task<Species?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<Species>> GetAllAsync(int tenantId);
    Task<IReadOnlyList<Species>> GetPagedAsync(int tenantId, int page, int pageSize);
    Task<int> GetCountAsync(int tenantId);
    Task<Species> AddAsync(Species species);
    Task UpdateAsync(Species species);
    Task DeleteAsync(int id);
    void Track(Species species);
    Task DeleteRangeAsync(IEnumerable<int> ids);
}
