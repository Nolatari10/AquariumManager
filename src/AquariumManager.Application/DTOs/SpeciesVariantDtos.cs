namespace AquariumManager.Application.DTOs;

public record CreateSpeciesVariantDto
{
    public string VariantName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

public record UpdateSpeciesVariantDto
{
    public string VariantName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

public record SpeciesVariantDto
{
    public int Id { get; set; }
    public int SpeciesId { get; set; }
    public string SpeciesCommonName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public int InventoryLotCount { get; set; }
}
