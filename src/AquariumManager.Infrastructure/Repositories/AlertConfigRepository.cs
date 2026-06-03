using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class AlertConfigRepository : IAlertConfigRepository
{
    private readonly AquariumDbContext _context;

    public AlertConfigRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<AlertConfig?> GetByAlertTypeAsync(int tenantId, string alertType)
    {
        return await _context.AlertConfigs
            .Where(a => a.TenantId == tenantId)
            .FirstOrDefaultAsync(a => a.AlertType == alertType);
    }

    public async Task<IReadOnlyList<AlertConfig>> GetAllAsync(int tenantId)
    {
        return await _context.AlertConfigs
            .Where(a => a.TenantId == tenantId)
            .OrderBy(a => a.AlertType)
            .ToListAsync();
    }

    public async Task<AlertConfig?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.AlertConfigs
            .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.Id == id);
    }

    public async Task UpdateAsync(AlertConfig config)
    {
        _context.AlertConfigs.Update(config);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(AlertConfig config)
    {
        _context.AlertConfigs.Add(config);
        await _context.SaveChangesAsync();
    }
}
