using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class MaintenanceLogRepository : IMaintenanceLogRepository
{
    private readonly AquariumDbContext _context;

    public MaintenanceLogRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceLog?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceLogs.FindAsync(id);
    }

    public async Task<IReadOnlyList<MaintenanceLog>> GetByTankAsync(int tankId, DateTime? from = null, DateTime? to = null, MaintenanceType? type = null, int page = 1, int pageSize = 50)
    {
        var query = _context.MaintenanceLogs.Where(m => m.TankId == tankId);

        if (from.HasValue)
            query = query.Where(m => m.PerformedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(m => m.PerformedAt <= to.Value);
        if (type.HasValue)
            query = query.Where(m => m.MaintenanceType == type.Value);

        return await query
            .OrderByDescending(m => m.PerformedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<MaintenanceLog> AddAsync(MaintenanceLog log)
    {
        _context.MaintenanceLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<int> GetCountByTankAsync(int tankId)
    {
        return await _context.MaintenanceLogs.CountAsync(m => m.TankId == tankId);
    }
}
