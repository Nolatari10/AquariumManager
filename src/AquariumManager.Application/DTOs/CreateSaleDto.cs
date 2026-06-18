namespace AquariumManager.Application.DTOs;

public record CreateSaleDto
{
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string SaleType { get; set; } = "Retail";
    public string? OrderNote { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}