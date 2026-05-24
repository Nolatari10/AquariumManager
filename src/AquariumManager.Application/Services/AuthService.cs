using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AquariumManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, ITenantRepository tenantRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);
        return new LoginResponse(token, user.Id, user.Email, user.Role, user.Tenant?.Name ?? string.Empty);
    }

    public async Task<LoginResponse> RegisterOwnerAsync(RegisterOwnerRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var tenant = new Tenant(request.StoreName);
        await _tenantRepository.AddAsync(tenant);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Email, passwordHash, "Owner", tenant.Id);
        await _userRepository.AddAsync(user);

        var token = GenerateJwtToken(user);
        return new LoginResponse(token, user.Id, user.Email, user.Role, tenant.Name);
    }

    public async Task<LoginResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, int tenantId)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Email, passwordHash, "Employee", tenantId);
        await _userRepository.AddAsync(user);

        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        var token = GenerateJwtToken(user);
        return new LoginResponse(token, user.Id, user.Email, user.Role, tenant?.Name ?? string.Empty);
    }

    public async Task<OperationResult> ChangePasswordAsync(int userId, int tenantId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(tenantId, userId);
        if (user is null)
            return OperationResult.Fail("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return OperationResult.Fail("Current password is incorrect.");

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return OperationResult.Fail("New password must be at least 6 characters.");

        user.SetPassword(BCrypt.Net.BCrypt.HashPassword(request.NewPassword));
        await _userRepository.UpdateAsync(user);

        return OperationResult.Ok();
    }

    public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync(int tenantId)
    {
        var users = await _userRepository.GetByTenantAsync(tenantId);
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role,
            TenantId = u.TenantId
        }).ToList();
    }

    public async Task<OperationResult> DeleteUserAsync(int userId, int currentUserId, int tenantId)
    {
        if (userId == currentUserId)
            return OperationResult.Fail("Cannot delete your own account.");

        var user = await _userRepository.GetByIdAsync(tenantId, userId);
        if (user is null)
            return OperationResult.Fail("User not found.");

        if (user.TenantId != tenantId)
            return OperationResult.Fail("Cannot delete user from another store.");

        if (user.Role == "Owner")
        {
            var allUsers = await _userRepository.GetByTenantAsync(tenantId);
            var ownerCount = allUsers.Count(u => u.Role == "Owner");
            if (ownerCount <= 1)
                return OperationResult.Fail("Cannot delete the last Owner account.");
        }

        await _userRepository.DeleteAsync(userId);
        return OperationResult.Ok();
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("tenantId", user.TenantId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
