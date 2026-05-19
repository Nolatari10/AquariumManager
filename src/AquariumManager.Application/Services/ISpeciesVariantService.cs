using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ISpeciesVariantService
{
    Task<IReadOnlyList<SpeciesVariantDto>> GetBySpeciesIdAsync(int speciesId);
    Task<SpeciesVariantDto?> GetByIdAsync(int id);
    Task<OperationResult<SpeciesVariantDto>> CreateAsync(int speciesId, CreateSpeciesVariantDto dto);
    Task<OperationResult> UpdateAsync(int speciesId, int variantId, UpdateSpeciesVariantDto dto);
    Task<OperationResult> DeleteAsync(int speciesId, int variantId);
}
