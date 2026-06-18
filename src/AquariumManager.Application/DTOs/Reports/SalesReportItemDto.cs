public record SalesReportItemDto
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string SaleType { get; set; } = "Retail";
    public int TotalItems { get; set; }
    public decimal TotalAmount { get; set; }
    
}