namespace AquariumManager.Domain.Entities;

public class Tenant
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? ContactInfo { get; private set; }

    public ICollection<User> Users { get; private set; } = new List<User>();

    private Tenant() { }

    public Tenant(string name, string? contactInfo = null)
    {
        Name = name;
        ContactInfo = contactInfo;
    }

    public void UpdateInfo(string name, string? contactInfo)
    {
        Name = name;
        ContactInfo = contactInfo;
    }
}
