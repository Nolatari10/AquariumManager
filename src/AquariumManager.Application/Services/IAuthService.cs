using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterOwnerAsync(RegisterOwnerRequest request);
    Task<LoginResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, int tenantId);
    Task<OperationResult> ChangePasswordAsync(int userId, int tenantId, ChangePasswordRequest request);
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync(int tenantId);
    Task<OperationResult> DeleteUserAsync(int userId, int currentUserId, int tenantId);
}
