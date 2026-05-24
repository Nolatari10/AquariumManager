using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AquariumDbContext _context;

    public UserRepository(AquariumDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int tenantId, int id)
    {
        return await _context.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Tenant)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<User>> GetByTenantAsync(int tenantId)
    {
        return await _context.Users
            .Include(u => u.Tenant)
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
