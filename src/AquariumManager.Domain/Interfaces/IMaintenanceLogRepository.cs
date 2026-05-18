using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IMaintenanceLogRepository
{
    Task<MaintenanceLog?> GetByIdAsync(int id);
    Task<IReadOnlyList<MaintenanceLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, MaintenanceType? type = null, int page = 1, int pageSize = 50);
    Task<MaintenanceLog> AddAsync(MaintenanceLog log);
    Task<int> GetCountByTankAsync(int tankId);
}
