using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AquariumDbContext _context;

    public SupplierRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.Suppliers
            .Include(s => s.InventoryLots)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == id);
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(int tenantId)
    {
        return await _context.Suppliers
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Supplier>> GetAllWithLotsAsync(int tenantId)
    {
        return await _context.Suppliers
            .Where(s => s.TenantId == tenantId)
            .Include(s => s.InventoryLots)
                .ThenInclude(l => l.MortalityRecords)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier != null)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }
    }
}
