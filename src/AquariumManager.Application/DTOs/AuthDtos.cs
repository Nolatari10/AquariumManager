namespace AquariumManager.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, int UserId, string Email, string Role);

public record RegisterOwnerRequest(string Email, string Password);

public record RegisterEmployeeRequest(string Email, string Password);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
