using System.Security.Claims;
using AquariumManager.Application.Common;

namespace AquariumManager.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId =>
        int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    public int TenantId =>
        int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue("tenantId") ?? "0");

    public string Role =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
