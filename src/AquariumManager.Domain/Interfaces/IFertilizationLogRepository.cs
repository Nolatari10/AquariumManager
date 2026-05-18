using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IFertilizationLogRepository
{
    Task<FertilizationLog?> GetByIdAsync(int id);
    Task<IReadOnlyList<FertilizationLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, FertilizerType? type = null, int page = 1, int pageSize = 50);
    Task<FertilizationLog> AddAsync(FertilizationLog log);
    Task<int> GetCountByTankAsync(int tankId);
}
