using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(int id);
    Task<Tenant> AddAsync(Tenant tenant);
}
