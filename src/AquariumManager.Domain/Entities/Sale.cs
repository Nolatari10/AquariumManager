namespace AquariumManager.Domain.Entities;
public class Sale
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public SaleType SaleType { get; set; } = SaleType.Retail;
    public string? OrderNote { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}

