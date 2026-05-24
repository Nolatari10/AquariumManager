namespace AquariumManager.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    public int TenantId { get; set; }
    public Tenant Tenant { get; private set; } = null!;

    private User() { }

    public User(string email, string passwordHash, string role, int tenantId)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        TenantId = tenantId;
    }

    public void SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}
