using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class InventoryLotRepository : IInventoryLotRepository
{
    private readonly AquariumDbContext _context;

    public InventoryLotRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryLot?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.Id == id);
    }

    public async Task<IReadOnlyList<InventoryLot>> GetBySpeciesIdAsync(int tenantId, int speciesId)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.TenantId == tenantId && l.SpeciesVariant.SpeciesId == speciesId)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetBySpeciesVariantIdAsync(int tenantId, int speciesVariantId)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.TenantId == tenantId && l.SpeciesVariantId == speciesVariantId)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesVariantIdAsync(int tenantId, int speciesVariantId)
    {
        var lots = await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.TenantId == tenantId && l.SpeciesVariantId == speciesVariantId)
            .OrderBy(l => l.ArrivalDate)
            .ToListAsync();

        return lots
            .Where(l => l.GetCurrentStock() > 0)
            .ToList();
    }

    public async Task AddAsync(InventoryLot lot)
    {
        _context.InventoryLots.Add(lot);
        await _context.SaveChangesAsync();
    }

    public void AddRange(IEnumerable<InventoryLot> lots)
    {
        _context.InventoryLots.AddRange(lots);
    }

    public async Task UpdateAsync(InventoryLot lot)
    {
        _context.InventoryLots.Update(lot);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetAllAsync(int tenantId)
    {
        return await _context.InventoryLots
            .Where(l => l.TenantId == tenantId)
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetPagedAsync(int tenantId, int page, int pageSize)
    {
        return await _context.InventoryLots
            .Where(l => l.TenantId == tenantId)
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .OrderByDescending(l => l.ArrivalDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(int tenantId)
    {
        return await _context.InventoryLots
            .Where(l => l.TenantId == tenantId)
            .CountAsync();
    }
}
