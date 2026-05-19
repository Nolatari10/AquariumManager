namespace AquariumManager.Application.DTOs;

public record CreateSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public record UpdateSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public record SpeciesDto
{
    public int Id { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;
    public decimal? MinPH { get; set; }
    public decimal? MaxPH { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public string? CompatibilityNotes { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string ImageUrl { get;  set; } = string.Empty;
}

public record BulkImportSpeciesDto
{
    public List<CreateSpeciesDto> Species { get; set; } = new();
}

public record BulkImportResultDto
{
    public int TotalProcessed { get; set; }
    public int Created { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}

public record BulkDeleteSpeciesDto
{
    public List<int> Ids { get; set; } = new();
}

public record BulkDeleteResultDto
{
    public int Requested { get; set; }
    public int Deleted { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
