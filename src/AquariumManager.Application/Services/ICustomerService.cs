using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<CustomerDto>> GetAllAsync();
    Task<IReadOnlyList<CustomerDto>> GetByTypeAsync(string type);
    Task<OperationResult> UpdateAsync(int id, UpdateCustomerDto dto);
    Task<OperationResult> DeleteAsync(int id);
}
