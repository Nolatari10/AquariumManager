using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUser;

    public CustomerService(ICustomerRepository customerRepository, ICurrentUserService currentUser)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Customer name is required.");

        if (!Enum.TryParse<CustomerType>(dto.CustomerType, true, out var customerType))
            throw new ArgumentException($"Invalid CustomerType: {dto.CustomerType}");

        var customer = new Customer
        {
            Name = dto.Name.Trim(),
            CustomerType = customerType,
            ContactName = dto.ContactName?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim(),
            Notes = dto.Notes?.Trim(),
            IsActive = dto.IsActive
        };
        customer.TenantId = _currentUser.TenantId;

        await _customerRepository.AddAsync(customer);
        return MapToDto(customer);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(_currentUser.TenantId, id);
        return customer is null ? null : MapToDto(customer);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync()
    {
        var list = await _customerRepository.GetAllAsync(_currentUser.TenantId);
        return list.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<CustomerDto>> GetByTypeAsync(string type)
    {
        if (!Enum.TryParse<CustomerType>(type, true, out var customerType))
            throw new ArgumentException($"Invalid CustomerType: {type}");

        var list = await _customerRepository.GetByTypeAsync(_currentUser.TenantId, customerType);
        return list.Select(MapToDto).ToList();
    }

    public async Task<OperationResult> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(_currentUser.TenantId, id);
        if (customer is null)
            return OperationResult.Fail("Customer not found.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return OperationResult.Fail("Customer name is required.");

        if (!Enum.TryParse<CustomerType>(dto.CustomerType, true, out var customerType))
            return OperationResult.Fail($"Invalid CustomerType: {dto.CustomerType}");

        customer.Name = dto.Name.Trim();
        customer.CustomerType = customerType;
        customer.ContactName = dto.ContactName?.Trim();
        customer.Phone = dto.Phone?.Trim();
        customer.Email = dto.Email?.Trim();
        customer.Notes = dto.Notes?.Trim();
        customer.IsActive = dto.IsActive;

        await _customerRepository.UpdateAsync(customer);
        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(_currentUser.TenantId, id);
        if (customer is null)
            return OperationResult.Fail("Customer not found.");

        await _customerRepository.DeleteAsync(id);
        return OperationResult.Ok();
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        CustomerType = c.CustomerType.ToString(),
        ContactName = c.ContactName,
        Phone = c.Phone,
        Email = c.Email,
        Notes = c.Notes,
        IsActive = c.IsActive
    };
}
