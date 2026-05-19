using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AquariumDbContext _context;

    public SaleRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .OrderByDescending(s => s.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Sales.CountAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        _context.Sales.Add(sale);
    }

  
}
