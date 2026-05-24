using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SpeciesVariantRepository : ISpeciesVariantRepository
{
    private readonly AquariumDbContext _context;

    public SpeciesVariantRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<SpeciesVariant?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.SpeciesVariants
            .Include(v => v.Species)
            .Include(v => v.InventoryLots)
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.Id == id);
    }

    public async Task<IReadOnlyList<SpeciesVariant>> GetBySpeciesIdAsync(int tenantId, int speciesId)
    {
        return await _context.SpeciesVariants
            .Include(v => v.Species)
            .Include(v => v.InventoryLots)
            .Where(v => v.TenantId == tenantId && v.SpeciesId == speciesId)
            .OrderBy(v => v.VariantName)
            .ToListAsync();
    }

    public async Task<SpeciesVariant> AddAsync(SpeciesVariant variant)
    {
        _context.SpeciesVariants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task UpdateAsync(SpeciesVariant variant)
    {
        _context.SpeciesVariants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var variant = await _context.SpeciesVariants.FindAsync(id);
        if (variant is not null)
        {
            _context.SpeciesVariants.Remove(variant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByNameAsync(int tenantId, int speciesId, string variantName, int? excludeId = null)
    {
        var query = _context.SpeciesVariants
            .Where(v => v.TenantId == tenantId && v.SpeciesId == speciesId && v.VariantName == variantName);

        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasInventoryLotsAsync(int tenantId, int variantId)
    {
        return await _context.InventoryLots.AnyAsync(l => l.TenantId == tenantId && l.SpeciesVariantId == variantId);
    }

    public async Task<bool> HasInventoryLotsForSpeciesAsync(int tenantId, int speciesId)
    {
        return await _context.InventoryLots.AnyAsync(l => l.TenantId == tenantId && l.SpeciesVariant.SpeciesId == speciesId);
    }
}
