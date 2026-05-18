using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class TargetParameterRangeRepository : ITargetParameterRangeRepository
{
    private readonly AquariumDbContext _context;

    public TargetParameterRangeRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TargetParameterRange>> GetByTankAsync(int tankId)
    {
        return await _context.TargetParameterRanges
            .Where(r => r.TankId == tankId)
            .ToListAsync();
    }

    public async Task UpsertAsync(int tankId, List<(ParameterName Name, decimal MinValue, decimal MaxValue, string Unit)> ranges)
    {
        var existing = await _context.TargetParameterRanges
            .Where(r => r.TankId == tankId)
            .ToListAsync();

        _context.TargetParameterRanges.RemoveRange(existing);

        foreach (var range in ranges)
        {
            _context.TargetParameterRanges.Add(new TargetParameterRange(
                tankId, range.Name, range.MinValue, range.MaxValue, range.Unit));
        }

        await _context.SaveChangesAsync();
    }
}
