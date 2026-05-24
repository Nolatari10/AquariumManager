using AquariumManager.Domain.Entities;

public class SaleItem
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int SpeciesId { get; set; }
    public Species Species { get; set; } = null!;

    public int? SpeciesVariantId { get; set; }
    public SpeciesVariant? SpeciesVariant { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}