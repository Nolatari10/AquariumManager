namespace AquariumManager.Application.DTOs;

public record CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = "Retail";
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}

public record UpdateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = "Retail";
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}

public record CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = "Retail";
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
