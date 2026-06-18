using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AquariumDbContext _context;

    public CustomerRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(int tenantId)
    {
        return await _context.Customers
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Customer>> GetByTypeAsync(int tenantId, CustomerType type)
    {
        return await _context.Customers
            .Where(c => c.TenantId == tenantId && c.CustomerType == type)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
