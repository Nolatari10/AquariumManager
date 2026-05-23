using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IAlertConfigRepository
{
    Task<AlertConfig?> GetByAlertTypeAsync(string alertType);
    Task<IReadOnlyList<AlertConfig>> GetAllAsync();
    Task<AlertConfig?> GetByIdAsync(int id);
    Task UpdateAsync(AlertConfig config);
}
