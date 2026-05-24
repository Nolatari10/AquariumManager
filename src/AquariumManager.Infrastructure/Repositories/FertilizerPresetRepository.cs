using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class FertilizerPresetRepository : IFertilizerPresetRepository
{
    private readonly AquariumDbContext _context;

    public FertilizerPresetRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<FertilizerPreset?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.FertilizerPresets
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);
    }

    public async Task<IReadOnlyList<FertilizerPreset>> GetAllAsync(int tenantId, int? ownerUserId = null)
    {
        var query = _context.FertilizerPresets.Where(p => p.IsActive && p.TenantId == tenantId).AsQueryable();

        if (ownerUserId.HasValue)
            query = query.Where(p => p.OwnerUserId == null || p.OwnerUserId == ownerUserId.Value);
        else
            query = query.Where(p => p.OwnerUserId == null);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<FertilizerPreset> AddAsync(FertilizerPreset preset)
    {
        _context.FertilizerPresets.Add(preset);
        await _context.SaveChangesAsync();
        return preset;
    }

    public async Task UpdateAsync(FertilizerPreset preset)
    {
        _context.FertilizerPresets.Update(preset);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var preset = await _context.FertilizerPresets.FindAsync(id);
        if (preset is not null)
        {
            _context.FertilizerPresets.Remove(preset);
            await _context.SaveChangesAsync();
        }
    }
}
