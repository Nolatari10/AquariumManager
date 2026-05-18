using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISpeciesRepository
{
    Task<Species?> GetByIdAsync(int id);
    Task<IReadOnlyList<Species>> GetAllAsync();
    Task<IReadOnlyList<Species>> GetPagedAsync(int page, int pageSize);
    Task<int> GetCountAsync();
    Task<Species> AddAsync(Species species);
    Task UpdateAsync(Species species);
    Task DeleteAsync(int id);
    void Track(Species species);
    Task DeleteRangeAsync(IEnumerable<int> ids);
}
