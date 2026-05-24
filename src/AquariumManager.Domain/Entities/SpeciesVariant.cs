namespace AquariumManager.Domain.Entities;

public class SpeciesVariant
{
    public int Id { get; private set; }
    public int TenantId { get; set; }
    public int SpeciesId { get; private set; }
    public Species Species { get; private set; } = null!;
    public string VariantName { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string? ImageUrl { get; private set; }

    public ICollection<InventoryLot> InventoryLots { get; private set; } = new List<InventoryLot>();

    private SpeciesVariant() { }

    public SpeciesVariant(int speciesId, string variantName, string? notes = null, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name is required.", nameof(variantName));

        SpeciesId = speciesId;
        VariantName = variantName;
        Notes = notes;
        ImageUrl = imageUrl;
    }

    public void UpdateInfo(string variantName, string? notes, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name is required.", nameof(variantName));

        VariantName = variantName;
        Notes = notes;
        if (imageUrl != null) ImageUrl = imageUrl;
    }
}
