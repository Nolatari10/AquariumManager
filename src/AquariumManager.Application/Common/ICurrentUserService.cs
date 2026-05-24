namespace AquariumManager.Application.Common;

public interface ICurrentUserService
{
    int UserId { get; }
    int TenantId { get; }
    string Role { get; }
}
