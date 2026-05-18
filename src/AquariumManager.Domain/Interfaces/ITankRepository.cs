using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ITankRepository
{
    Task<Tank?> GetByIdAsync(int id);
    Task<IReadOnlyList<Tank>> GetByOwnerAsync(int ownerUserId);
    Task<IReadOnlyList<Tank>> GetAllAsync(int? ownerUserId = null, TankType? tankType = null, bool? isActive = null);
    Task<Tank> AddAsync(Tank tank);
    Task UpdateAsync(Tank tank);
    Task DeleteAsync(int id);
}
