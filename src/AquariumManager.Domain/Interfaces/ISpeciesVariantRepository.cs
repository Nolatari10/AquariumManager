using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ISpeciesVariantRepository
{
    Task<SpeciesVariant?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<SpeciesVariant>> GetBySpeciesIdAsync(int tenantId, int speciesId);
    Task<SpeciesVariant> AddAsync(SpeciesVariant variant);
    Task UpdateAsync(SpeciesVariant variant);
    Task DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(int tenantId, int speciesId, string variantName, int? excludeId = null);
    Task<bool> HasInventoryLotsAsync(int tenantId, int variantId);
    Task<bool> HasInventoryLotsForSpeciesAsync(int tenantId, int speciesId);
}
