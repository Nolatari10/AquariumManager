using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterOwnerAsync(RegisterOwnerRequest request);
    Task<LoginResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request);
}
