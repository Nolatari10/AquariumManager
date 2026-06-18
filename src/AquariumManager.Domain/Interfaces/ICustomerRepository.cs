using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<Customer>> GetAllAsync(int tenantId);
    Task<IReadOnlyList<Customer>> GetByTypeAsync(int tenantId, CustomerType type);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
