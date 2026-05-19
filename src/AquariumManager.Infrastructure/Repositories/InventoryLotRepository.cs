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

    public async Task<InventoryLot?> GetByIdAsync(int id)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IReadOnlyList<InventoryLot>> GetBySpeciesIdAsync(int speciesId)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.SpeciesVariant.SpeciesId == speciesId)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetBySpeciesVariantIdAsync(int speciesVariantId)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.SpeciesVariantId == speciesVariantId)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesVariantIdAsync(int speciesVariantId)
    {
        var lots = await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .Where(l => l.SpeciesVariantId == speciesVariantId)
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

    public async Task UpdateAsync(InventoryLot lot)
    {
        _context.InventoryLots.Update(lot);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetAllAsync()
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .OrderByDescending(l => l.ArrivalDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryLot>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.InventoryLots
            .Include(l => l.SpeciesVariant)
                .ThenInclude(v => v.Species)
            .Include(l => l.Supplier)
            .Include(l => l.MortalityRecords)
            .OrderByDescending(l => l.ArrivalDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.InventoryLots.CountAsync();
    }
}
