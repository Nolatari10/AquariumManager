namespace AquariumManager.Application.DTOs;

public record BulkInventoryLotCreateItemDto
{
    public int SpeciesVariantId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int DeadOnArrival { get; set; }
    public decimal UnitCost { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}

public record BulkInventoryLotCreateRequestDto
{
    public List<BulkInventoryLotCreateItemDto> Items { get; set; } = new();
}

public record BulkInventoryLotCreateResponseDto
{
    public List<int> CreatedLotIds { get; set; } = new();
    public int TotalCreated { get; set; }
    public int TotalQuantity { get; set; }
}
