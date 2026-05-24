using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class TankPhotoRepository : ITankPhotoRepository
{
    private readonly AquariumDbContext _context;

    public TankPhotoRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<TankPhoto?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.TankPhotos
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);
    }

    public async Task<IReadOnlyList<TankPhoto>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20)
    {
        var query = _context.TankPhotos.Where(p => p.TankId == tankId);

        if (from.HasValue)
            query = query.Where(p => p.TakenAt >= from.Value);
        if (to.HasValue)
            query = query.Where(p => p.TakenAt <= to.Value);

        return await query
            .OrderByDescending(p => p.TakenAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<TankPhoto> AddAsync(TankPhoto photo)
    {
        _context.TankPhotos.Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task DeleteAsync(int id)
    {
        var photo = await _context.TankPhotos.FindAsync(id);
        if (photo is not null)
        {
            _context.TankPhotos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }
}
