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

    public async Task<AlertConfig?> GetByAlertTypeAsync(string alertType)
    {
        return await _context.AlertConfigs
            .FirstOrDefaultAsync(a => a.AlertType == alertType);
    }

    public async Task<IReadOnlyList<AlertConfig>> GetAllAsync()
    {
        return await _context.AlertConfigs
            .OrderBy(a => a.AlertType)
            .ToListAsync();
    }

    public async Task<AlertConfig?> GetByIdAsync(int id)
    {
        return await _context.AlertConfigs.FindAsync(id);
    }

    public async Task UpdateAsync(AlertConfig config)
    {
        _context.AlertConfigs.Update(config);
        await _context.SaveChangesAsync();
    }
}
