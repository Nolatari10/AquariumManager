using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: api/Reports/stock
    [HttpGet("stock")]
    public async Task<IActionResult> GetStockReport()
    {
        var report = await _reportService.GetStockReportAsync();
        return Ok(report);
    }

    // GET: api/Reports/mortality?startDate=X&endDate=Y&speciesId=Z&supplierId=W
    [HttpGet("mortality")]
    public async Task<IActionResult> GetMortalityReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? speciesId = null,
        [FromQuery] int? supplierId = null)
    {
        var report = await _reportService.GetMortalityReportAsync(startDate, endDate, speciesId, supplierId);
        return Ok(report);
    }

    // GET: api/Reports/sales?startDate=X&endDate=Y
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (startDate > endDate)
            return BadRequest("startDate must be before endDate.");

        var report = await _reportService.GetSalesReportAsync(startDate, endDate, page, pageSize);
        return Ok(report);
    }

    // GET: api/Reports/valuation
    [HttpGet("valuation")]
    public async Task<IActionResult> GetInventoryValuation()
    {
        var report = await _reportService.GetInventoryValuationAsync();
        return Ok(report);
    }

    // GET: api/Reports/supplier-performance?startDate=X&endDate=Y
    [HttpGet("supplier-performance")]
    public async Task<IActionResult> GetSupplierPerformance(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var report = await _reportService.GetSupplierPerformanceAsync(startDate, endDate);
        return Ok(report);
    }

    // GET: api/Reports/inventory-turnover?speciesId=X&supplierId=Y
    [HttpGet("inventory-turnover")]
    public async Task<IActionResult> GetInventoryTurnover(
        [FromQuery] int? speciesId = null,
        [FromQuery] int? supplierId = null)
    {
        var report = await _reportService.GetInventoryTurnoverAsync(speciesId, supplierId);
        return Ok(report);
    }

    // GET: api/Reports/profitability?startDate=X&endDate=Y
    [HttpGet("profitability")]
    public async Task<IActionResult> GetProfitabilityReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest("startDate must be before endDate.");

        var report = await _reportService.GetProfitabilityReportAsync(startDate, endDate);
        return Ok(report);
    }
}
