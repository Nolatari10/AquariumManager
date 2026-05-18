using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class TankRepository : ITankRepository
{
    private readonly AquariumDbContext _context;

    public TankRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Tank?> GetByIdAsync(int id)
    {
        return await _context.Tanks
            .Include(t => t.OwnerUser)
            .Include(t => t.TargetParameterRanges)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IReadOnlyList<Tank>> GetByOwnerAsync(int ownerUserId)
    {
        return await _context.Tanks
            .Include(t => t.OwnerUser)
            .Where(t => t.OwnerUserId == ownerUserId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Tank>> GetAllAsync(int? ownerUserId = null, TankType? tankType = null, bool? isActive = null)
    {
        var query = _context.Tanks.Include(t => t.OwnerUser).AsQueryable();

        if (ownerUserId.HasValue)
            query = query.Where(t => t.OwnerUserId == ownerUserId.Value);
        if (tankType.HasValue)
            query = query.Where(t => t.TankType == tankType.Value);
        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<Tank> AddAsync(Tank tank)
    {
        _context.Tanks.Add(tank);
        await _context.SaveChangesAsync();
        return tank;
    }

    public async Task UpdateAsync(Tank tank)
    {
        _context.Tanks.Update(tank);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tank = await _context.Tanks.FindAsync(id);
        if (tank is not null)
        {
            tank.Deactivate();
            await _context.SaveChangesAsync();
        }
    }
}
