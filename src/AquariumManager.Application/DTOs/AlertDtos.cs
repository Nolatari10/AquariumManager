namespace AquariumManager.Application.DTOs;

public record AlertConfigDto
{
    public int Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public decimal ThresholdValue { get; set; }
    public bool IsEnabled { get; set; }
}

public record UpdateAlertConfigDto
{
    public decimal ThresholdValue { get; set; }
    public bool IsEnabled { get; set; }
}

public record HighMortalityAlertDto
{
    public int LotId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string? VariantName { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int CurrentStock { get; set; }
    public int InitialQuantity { get; set; }
    public int DeadOnArrival { get; set; }
    public int NonSoldMortality { get; set; }
    public decimal MortalityRatePercent { get; set; }
    public decimal UnitCost { get; set; }
    public decimal CostLost { get; set; }
    public int DaysSinceArrival { get; set; }
    public bool ThresholdExceeded { get; set; }
}
