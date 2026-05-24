using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IFertilizerPresetRepository
{
    Task<FertilizerPreset?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<FertilizerPreset>> GetAllAsync(int tenantId, int? ownerUserId = null);
    Task<FertilizerPreset> AddAsync(FertilizerPreset preset);
    Task UpdateAsync(FertilizerPreset preset);
    Task DeleteAsync(int id);
}
