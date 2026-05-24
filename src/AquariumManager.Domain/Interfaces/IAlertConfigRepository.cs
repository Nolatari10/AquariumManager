using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IAlertConfigRepository
{
    Task<AlertConfig?> GetByAlertTypeAsync(int tenantId, string alertType);
    Task<IReadOnlyList<AlertConfig>> GetAllAsync(int tenantId);
    Task<AlertConfig?> GetByIdAsync(int tenantId, int id);
    Task UpdateAsync(AlertConfig config);
}
