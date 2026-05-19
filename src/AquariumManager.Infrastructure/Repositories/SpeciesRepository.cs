using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly AquariumDbContext _context;

    public SpeciesRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Species?> GetByIdAsync(int id)
    {
        return await _context.Species
            .Include(s => s.InventoryItems) // legacy
            .Include(s => s.Variants)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Species>> GetAllAsync()
    {
        return await _context.Species
            .OrderBy(s => s.CommonName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Species>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Species
            .OrderBy(s => s.CommonName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Species.CountAsync();
    }

    public async Task<Species> AddAsync(Species species)
    {
        _context.Species.Add(species);
        await _context.SaveChangesAsync();
        return species;
    }

    public async Task UpdateAsync(Species species)
    {
        _context.Species.Update(species);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var species = await _context.Species.FindAsync(id);
        if (species is null)
        {
            return;
        }

        _context.Species.Remove(species);
        await _context.SaveChangesAsync();
    }

    public void Track(Species species)
    {
        _context.Species.Add(species);
    }

    public async Task DeleteRangeAsync(IEnumerable<int> ids)
    {
        var list = await _context.Species.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (list.Count > 0)
        {
            _context.Species.RemoveRange(list);
            await _context.SaveChangesAsync();
        }
    }
}
