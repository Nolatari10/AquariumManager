namespace AquariumManager.Domain.Entities;

public class Species
{
    public int Id { get; private set; }
    public int TenantId { get; set; }

    public string CommonName { get; private set; } = string.Empty;
    public string ScientificName { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Variety { get; private set; } = string.Empty;
    public decimal? MinPH { get; private set; }
    public decimal? MaxPH { get; private set; }
    public decimal? MinTemperature { get; private set; }
    public decimal? MaxTemperature { get; private set; }
    public string? CompatibilityNotes { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public string ImageUrl { get;  set; } = string.Empty;
    public decimal? RetailPrice { get; set; }
    public decimal? WholesalePrice { get; set; }

    public ICollection<SpeciesVariant> Variants { get; private set; } = new List<SpeciesVariant>();

    private Species() { }

    public Species(
        string commonName,
        string scientificName,
        string type,
        string variety,
        decimal? minPH,
        decimal? maxPH,
        decimal? minTemperature,
        decimal? maxTemperature,
        string? compatibilityNotes,
        string category,
        string? notes = null,
        string imageUrl = "",
        decimal? retailPrice = null,
        decimal? wholesalePrice = null)
    {
        CommonName = commonName;
        ScientificName = scientificName;
        Type = type;
        Variety = variety;
        MinPH = minPH;
        MaxPH = maxPH;
        MinTemperature = minTemperature;
        MaxTemperature = maxTemperature;
        CompatibilityNotes = compatibilityNotes;
        Category = category;
        Notes = notes;
        ImageUrl = imageUrl;
        RetailPrice = retailPrice;
        WholesalePrice = wholesalePrice;
    }

    public void UpdateInfo(
        string commonName,
        string scientificName,
        string type,
        string variety,
        decimal? minPH,
        decimal? maxPH,
        decimal? minTemperature,
        decimal? maxTemperature,
        string? compatibilityNotes,
        string category,
        string? notes,
        string? imageUrl = null,
        decimal? retailPrice = null,
        decimal? wholesalePrice = null)
    {
        CommonName = commonName;
        ScientificName = scientificName;
        Type = type;
        Variety = variety;
        MinPH = minPH;
        MaxPH = maxPH;
        MinTemperature = minTemperature;
        MaxTemperature = maxTemperature;
        CompatibilityNotes = compatibilityNotes;
        Category = category;
        Notes = notes;
        ImageUrl = imageUrl ?? string.Empty;
        RetailPrice = retailPrice;
        WholesalePrice = wholesalePrice;
    }
}
