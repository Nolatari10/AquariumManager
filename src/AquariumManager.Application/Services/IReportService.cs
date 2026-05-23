using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IReportService
{
    Task<StockReportDto> GetStockReportAsync();
    Task<MortalityReportDto> GetMortalityReportAsync(DateTime? startDate = null, DateTime? endDate = null, int? speciesId = null, int? supplierId = null);
    Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate, int page, int pageSize);
    Task<InventoryValuationDto> GetInventoryValuationAsync();
    Task<SupplierPerformanceReportDto> GetSupplierPerformanceAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<InventoryTurnoverReportDto> GetInventoryTurnoverAsync(int? speciesId = null, int? supplierId = null);
    Task<ProfitabilityReportDto> GetProfitabilityReportAsync(DateTime startDate, DateTime endDate);
}
