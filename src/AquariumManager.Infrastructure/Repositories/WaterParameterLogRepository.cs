using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class WaterParameterLogRepository : IWaterParameterLogRepository
{
    private readonly AquariumDbContext _context;

    public WaterParameterLogRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<WaterParameterLog?> GetByIdAsync(int id)
    {
        return await _context.WaterParameterLogs.FindAsync(id);
    }

    public async Task<IReadOnlyList<WaterParameterLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50)
    {
        var query = _context.WaterParameterLogs.Where(w => w.TankId == tankId);

        if (from.HasValue)
            query = query.Where(w => w.MeasuredAt >= from.Value);
        if (to.HasValue)
            query = query.Where(w => w.MeasuredAt <= to.Value);

        return await query
            .OrderByDescending(w => w.MeasuredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<WaterParameterLog> AddAsync(WaterParameterLog log)
    {
        _context.WaterParameterLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<int> GetCountByTankAsync(int tankId)
    {
        return await _context.WaterParameterLogs.CountAsync(w => w.TankId == tankId);
    }
}
