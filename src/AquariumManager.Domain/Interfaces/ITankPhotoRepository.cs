using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ITankPhotoRepository
{
    Task<TankPhoto?> GetByIdAsync(int id);
    Task<IReadOnlyList<TankPhoto>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20);
    Task<TankPhoto> AddAsync(TankPhoto photo);
    Task DeleteAsync(int id);
}
