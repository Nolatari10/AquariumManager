using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IFertilizerPresetRepository
{
    Task<FertilizerPreset?> GetByIdAsync(int id);
    Task<IReadOnlyList<FertilizerPreset>> GetAllAsync(int? ownerUserId = null);
    Task<FertilizerPreset> AddAsync(FertilizerPreset preset);
    Task UpdateAsync(FertilizerPreset preset);
    Task DeleteAsync(int id);
}
