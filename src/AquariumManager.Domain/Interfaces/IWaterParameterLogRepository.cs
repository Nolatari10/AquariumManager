using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IWaterParameterLogRepository
{
    Task<WaterParameterLog?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<WaterParameterLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50);
    Task<WaterParameterLog> AddAsync(WaterParameterLog log);
    Task<int> GetCountByTankAsync(int tankId);
}
