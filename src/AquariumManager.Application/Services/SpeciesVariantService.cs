using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SpeciesVariantService : ISpeciesVariantService
{
    private readonly ISpeciesVariantRepository _variantRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ICurrentUserService _currentUser;

    public SpeciesVariantService(
        ISpeciesVariantRepository variantRepository,
        ISpeciesRepository speciesRepository,
        ICurrentUserService currentUser)
    {
        _variantRepository = variantRepository;
        _speciesRepository = speciesRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SpeciesVariantDto>> GetBySpeciesIdAsync(int speciesId)
    {
        var variants = await _variantRepository.GetBySpeciesIdAsync(_currentUser.TenantId, speciesId);
        return variants.Select(MapToDto).ToList();
    }

    public async Task<SpeciesVariantDto?> GetByIdAsync(int id)
    {
        var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, id);
        return variant is null ? null : MapToDto(variant);
    }

    public async Task<OperationResult<SpeciesVariantDto>> CreateAsync(int speciesId, CreateSpeciesVariantDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.VariantName))
            return OperationResult<SpeciesVariantDto>.Fail("Variant name is required.");

        var species = await _speciesRepository.GetByIdAsync(_currentUser.TenantId, speciesId);
        if (species is null)
            return OperationResult<SpeciesVariantDto>.Fail("Species not found.");

        var exists = await _variantRepository.ExistsByNameAsync(_currentUser.TenantId, speciesId, dto.VariantName);
        if (exists)
            return OperationResult<SpeciesVariantDto>.Fail("A variant with this name already exists for this species.");

        var variant = new SpeciesVariant(speciesId, dto.VariantName, dto.Notes, dto.ImageUrl);
        variant.TenantId = _currentUser.TenantId;
        await _variantRepository.AddAsync(variant);

        return OperationResult<SpeciesVariantDto>.Ok(MapToDto(variant));
    }

    public async Task<OperationResult> UpdateAsync(int speciesId, int variantId, UpdateSpeciesVariantDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.VariantName))
            return OperationResult.Fail("Variant name is required.");

        var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, variantId);
        if (variant is null || variant.SpeciesId != speciesId)
            return OperationResult.Fail("SpeciesVariant not found.");

        var exists = await _variantRepository.ExistsByNameAsync(_currentUser.TenantId, speciesId, dto.VariantName, variantId);
        if (exists)
            return OperationResult.Fail("A variant with this name already exists for this species.");

        variant.UpdateInfo(dto.VariantName, dto.Notes, dto.ImageUrl);
        await _variantRepository.UpdateAsync(variant);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int speciesId, int variantId)
    {
        var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, variantId);
        if (variant is null || variant.SpeciesId != speciesId)
            return OperationResult.Fail("SpeciesVariant not found.");

        var hasLots = await _variantRepository.HasInventoryLotsAsync(_currentUser.TenantId, variantId);
        if (hasLots)
            return OperationResult.Fail("Cannot delete this variant because it has linked inventory lots.");

        var allVariants = await _variantRepository.GetBySpeciesIdAsync(_currentUser.TenantId, speciesId);
        if (allVariants.Count <= 1)
            return OperationResult.Fail("Cannot delete the last variant of a species. Every species must have at least one variant.");

        await _variantRepository.DeleteAsync(variantId);
        return OperationResult.Ok();
    }

    private static SpeciesVariantDto MapToDto(SpeciesVariant v) => new()
    {
        Id = v.Id,
        SpeciesId = v.SpeciesId,
        SpeciesCommonName = v.Species?.CommonName ?? string.Empty,
        VariantName = v.VariantName,
        Notes = v.Notes,
        ImageUrl = v.ImageUrl,
        InventoryLotCount = v.InventoryLots?.Count ?? 0
    };
}
