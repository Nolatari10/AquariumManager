namespace AquariumManager.Application.DTOs;

public record CatalogItemDto
{
    public int SpeciesVariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalStock { get; set; }
    public decimal LatestUnitCost { get; set; }
    public decimal? RetailPrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
}
