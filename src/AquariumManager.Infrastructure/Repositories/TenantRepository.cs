using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AquariumDbContext _context;

    public TenantRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(int id)
    {
        return await _context.Tenants.FindAsync(id);
    }

    public async Task<Tenant> AddAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }
}
