using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterOwnerAsync(RegisterOwnerRequest request);
    Task<LoginResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request);
    Task<OperationResult> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync();
    Task<OperationResult> DeleteUserAsync(int userId, int currentUserId);
}
