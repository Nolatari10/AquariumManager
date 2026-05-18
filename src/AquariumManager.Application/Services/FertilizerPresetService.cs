using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class FertilizerPresetService : IFertilizerPresetService
{
    private readonly IFertilizerPresetRepository _presetRepo;

    public FertilizerPresetService(IFertilizerPresetRepository presetRepo)
    {
        _presetRepo = presetRepo;
    }

    public async Task<IReadOnlyList<FertilizerPresetDto>> GetAllAsync(int? ownerUserId = null)
    {
        var presets = await _presetRepo.GetAllAsync(ownerUserId);
        return presets.Select(MapToDto).ToList();
    }

    public async Task<FertilizerPresetDto> CreateAsync(int? ownerUserId, CreateFertilizerPresetDto dto)
    {
        var preset = new FertilizerPreset(
            dto.Name, dto.FertilizerType, dto.DefaultDoseAmount, dto.DefaultDoseUnit,
            ownerUserId, dto.NitratePerDose, dto.PhosphatePerDose, dto.PotassiumPerDose,
            dto.IronPerDose, dto.Notes);

        await _presetRepo.AddAsync(preset);
        return MapToDto(preset);
    }

    public async Task UpdateAsync(int id, CreateFertilizerPresetDto dto)
    {
        var preset = await _presetRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Preset not found.");

        // Update via reflection or manual - presets don't have UpdateInfo so recreate
        var updated = new FertilizerPreset(
            dto.Name, dto.FertilizerType, dto.DefaultDoseAmount, dto.DefaultDoseUnit,
            preset.OwnerUserId, dto.NitratePerDose, dto.PhosphatePerDose, dto.PotassiumPerDose,
            dto.IronPerDose, dto.Notes);

        await _presetRepo.UpdateAsync(updated);
    }

    public async Task DeleteAsync(int id)
    {
        await _presetRepo.DeleteAsync(id);
    }

    private static FertilizerPresetDto MapToDto(FertilizerPreset p) => new()
    {
        Id = p.Id,
        OwnerUserId = p.OwnerUserId,
        Name = p.Name,
        FertilizerType = p.FertilizerType.ToString(),
        DefaultDoseAmount = p.DefaultDoseAmount,
        DefaultDoseUnit = p.DefaultDoseUnit.ToString(),
        NitratePerDose = p.NitratePerDose,
        PhosphatePerDose = p.PhosphatePerDose,
        PotassiumPerDose = p.PotassiumPerDose,
        IronPerDose = p.IronPerDose,
        Notes = p.Notes,
        IsActive = p.IsActive
    };
}
