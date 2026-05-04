public record CreateSaleItemDto
{
    public int SpeciesId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}