namespace AquariumManager.Application.DTOs;

// Stock Report
public record StockReportDto
{
    public List<StockReportItemDto> Items { get; set; } = new();
    public int TotalSpecies { get; set; }
    public int TotalStock { get; set; }
}

public record StockReportItemDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public decimal TotalCostValue { get; set; }
    public List<LotBreakdownDto> Lots { get; set; } = new();
}

public record LotBreakdownDto
{
    public int LotId { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentStock { get; set; }
    public string? SupplierName { get; set; }
    public decimal UnitCost { get; set; }
}

// Mortality Report
public record MortalityReportDto
{
    public List<MortalitySummaryDto> Summaries { get; set; } = new();
    public int TotalDeaths { get; set; }
    public int TotalSold { get; set; }
    public int TotalOtherCauses { get; set; }
}

public record MortalitySummaryDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public int TotalDeaths { get; set; }
    public int Sold { get; set; }
    public int OtherCauses { get; set; }
    public List<MortalityRecordDto> Records { get; set; } = new();
}

public record MortalityRecordDto
{
    public int RecordId { get; set; }
    public int LotId { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public string? Cause { get; set; }
    public string? Notes { get; set; }
}

// Sales Report
public record SalesReportDto
{
    public List<SalesSummaryDto> Sales { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public int TotalItemsSold { get; set; }
    public List<TopSpeciesDto> TopSpecies { get; set; } = new();
}

public record SalesSummaryDto
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string SaleType { get; set; } = "Retail";
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public record TopSpeciesDto
{
    public string CommonName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

// Inventory Valuation
public record InventoryValuationDto
{
    public decimal TotalCostValue { get; set; }
    public int TotalUnitsInStock { get; set; }
    public int TotalLots { get; set; }
    public decimal AverageUnitCost { get; set; }
    public List<ValuationByCategoryDto> ByCategory { get; set; } = new();
}

public record ValuationByCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int UnitsInStock { get; set; }
    public decimal TotalCostValue { get; set; }
    public decimal AverageUnitCost { get; set; }
}

// Supplier Performance Report
public record SupplierPerformanceReportDto
{
    public List<SupplierPerformanceDto> Suppliers { get; set; } = new();
    public decimal TotalCostLost { get; set; }
    public decimal AverageMortalityRate { get; set; }
}

public record SupplierPerformanceDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int TotalLotsReceived { get; set; }
    public int TotalDOA { get; set; }
    public int NonSoldMortality { get; set; }
    public decimal CostLostToMortality { get; set; }
    public decimal MortalityRatePercent { get; set; }
    public int Rank { get; set; }
}

// Inventory Turnover / Aging Report
public record InventoryTurnoverReportDto
{
    public List<InventoryTurnoverDto> Lots { get; set; } = new();
    public int FreshLots { get; set; }
    public int AgingLots { get; set; }
    public int OldLots { get; set; }
    public decimal AverageDaysInStock { get; set; }
}

public record InventoryTurnoverDto
{
    public int LotId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int DaysInStock { get; set; }
    public int CurrentStock { get; set; }
    public int InitialQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public string AgingStatus { get; set; } = string.Empty;
    public decimal CostAtRisk { get; set; }
}

// Profitability Report
public record ProfitabilityReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMarginPercent { get; set; }
    public List<ProfitabilityBySpeciesDto> BySpecies { get; set; } = new();
}

public record ProfitabilityBySpeciesDto
{
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit { get; set; }
    public decimal MarginPercent { get; set; }
}
