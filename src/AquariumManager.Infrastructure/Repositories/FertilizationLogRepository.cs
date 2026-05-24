using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class FertilizationLogRepository : IFertilizationLogRepository
{
    private readonly AquariumDbContext _context;

    public FertilizationLogRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<FertilizationLog?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.FertilizationLogs
            .Include(f => f.FertilizerPreset)
            .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.Id == id);
    }

    public async Task<IReadOnlyList<FertilizationLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, FertilizerType? type = null, int page = 1, int pageSize = 50)
    {
        var query = _context.FertilizationLogs
            .Include(f => f.FertilizerPreset)
            .Where(f => f.TankId == tankId);

        if (from.HasValue)
            query = query.Where(f => f.DosedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(f => f.DosedAt <= to.Value);
        if (type.HasValue)
            query = query.Where(f => f.FertilizerType == type.Value);

        return await query
            .OrderByDescending(f => f.DosedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<FertilizationLog> AddAsync(FertilizationLog log)
    {
        _context.FertilizationLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<int> GetCountByTankAsync(int tankId)
    {
        return await _context.FertilizationLogs.CountAsync(f => f.TankId == tankId);
    }
}
