using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IFertilizerPresetService
{
    Task<IReadOnlyList<FertilizerPresetDto>> GetAllAsync(int? ownerUserId = null);
    Task<FertilizerPresetDto> CreateAsync(int? ownerUserId, CreateFertilizerPresetDto dto);
    Task UpdateAsync(int id, CreateFertilizerPresetDto dto);
    Task DeleteAsync(int id);
}
