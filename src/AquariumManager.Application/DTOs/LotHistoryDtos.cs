namespace AquariumManager.Application.DTOs;

public record LotHistoryDto
{
    public int LotId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string? SupplierName { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int DeadOnArrival { get; set; }
    public int CurrentStock { get; set; }
    public decimal UnitCost { get; set; }
    public string? BatchNumber { get; set; }
    public List<LotHistoryEventDto> Events { get; set; } = new();
}

public record LotHistoryEventDto
{
    public DateTime Date { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Cause { get; set; }
    public string? Notes { get; set; }
    public int? RelatedRecordId { get; set; }
}
