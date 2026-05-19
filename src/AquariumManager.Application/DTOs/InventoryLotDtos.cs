namespace AquariumManager.Application.DTOs;

public record CreateInventoryLotDto
{
    public int SpeciesVariantId { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int DeadOnArrival { get; set; }
    public decimal UnitCost { get; set; }
    public int? SupplierId { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}

public record InventoryLotDto
{
    public int Id { get; set; }
    public int SpeciesVariantId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string SpeciesCommonName { get; set; } = string.Empty;
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int DeadOnArrival { get; set; }
    public int TotalMortality { get; set; }
    public int CurrentStock { get; set; }
    public decimal UnitCost { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}

public record RegisterMortalityDto
{
    public int InventoryLotId { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public string? Cause { get; set; }
    public string? Notes { get; set; }
}
