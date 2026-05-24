using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class FertilizerPresetService : IFertilizerPresetService
{
    private readonly IFertilizerPresetRepository _presetRepo;
    private readonly ICurrentUserService _currentUser;

    public FertilizerPresetService(
        IFertilizerPresetRepository presetRepo,
        ICurrentUserService currentUser)
    {
        _presetRepo = presetRepo;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<FertilizerPresetDto>> GetAllAsync(int? ownerUserId = null)
    {
        var presets = await _presetRepo.GetAllAsync(_currentUser.TenantId, ownerUserId);
        return presets.Select(MapToDto).ToList();
    }

    public async Task<FertilizerPresetDto> CreateAsync(int? ownerUserId, CreateFertilizerPresetDto dto)
    {
        var preset = new FertilizerPreset(
            dto.Name, dto.FertilizerType, dto.DefaultDoseAmount, dto.DefaultDoseUnit,
            ownerUserId, dto.NitratePerDose, dto.PhosphatePerDose, dto.PotassiumPerDose,
            dto.IronPerDose, dto.Notes);

        preset.TenantId = _currentUser.TenantId;

        await _presetRepo.AddAsync(preset);
        return MapToDto(preset);
    }

    public async Task UpdateAsync(int id, CreateFertilizerPresetDto dto)
    {
        var preset = await _presetRepo.GetByIdAsync(_currentUser.TenantId, id)
            ?? throw new InvalidOperationException("Preset not found.");

        var updated = new FertilizerPreset(
            dto.Name, dto.FertilizerType, dto.DefaultDoseAmount, dto.DefaultDoseUnit,
            preset.OwnerUserId, dto.NitratePerDose, dto.PhosphatePerDose, dto.PotassiumPerDose,
            dto.IronPerDose, dto.Notes);

        updated.TenantId = _currentUser.TenantId;

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
