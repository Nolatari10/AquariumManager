namespace AquariumManager.Application.DTOs;

public record BiologicalStockDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public int CurrentBiologicalStock { get; set; }
}
