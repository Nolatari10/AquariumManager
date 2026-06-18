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

    public async Task<Sale?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == id);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync(int tenantId)
    {
        return await _context.Sales
            .Where(s => s.TenantId == tenantId)
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(int tenantId, DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.TenantId == tenantId)
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetPagedAsync(int tenantId, int page, int pageSize)
    {
        return await _context.Sales
            .Where(s => s.TenantId == tenantId)
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(si => si.Species)
            .Include(s => s.Items)
                .ThenInclude(si => si.SpeciesVariant)
            .OrderByDescending(s => s.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(int tenantId)
    {
        return await _context.Sales
            .Where(s => s.TenantId == tenantId)
            .CountAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        _context.Sales.Add(sale);
    }

   
}
