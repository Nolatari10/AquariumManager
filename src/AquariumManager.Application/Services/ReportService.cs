using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class ReportService : IReportService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IInventoryLotRepository _lotRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICurrentUserService _currentUser;

    public ReportService(
        ISpeciesRepository speciesRepository,
        IInventoryLotRepository lotRepository,
        ISaleRepository saleRepository,
        ISupplierRepository supplierRepository,
        ICurrentUserService currentUser)
    {
        _speciesRepository = speciesRepository;
        _lotRepository = lotRepository;
        _saleRepository = saleRepository;
        _supplierRepository = supplierRepository;
        _currentUser = currentUser;
    }

    public async Task<StockReportDto> GetStockReportAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync(_currentUser.TenantId);
        var report = new StockReportDto();

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);
            var openLots = lots.Where(l => l.GetCurrentStock() > 0).ToList();

            if (openLots.Count == 0) continue;

            var totalStock = openLots.Sum(l => l.GetCurrentStock());
            var totalCostValue = openLots.Sum(l => l.GetCurrentStock() * l.UnitCost);

            report.Items.Add(new StockReportItemDto
            {
                SpeciesId = species.Id,
                CommonName = species.CommonName,
                ScientificName = species.ScientificName,
                Category = species.Category,
                CurrentStock = totalStock,
                TotalCostValue = totalCostValue,
                Lots = openLots.Select(l => new LotBreakdownDto
                {
                    LotId = l.Id,
                    ArrivalDate = l.ArrivalDate,
                    InitialQuantity = l.InitialQuantity,
                    CurrentStock = l.GetCurrentStock(),
                    SupplierName = l.Supplier?.Name,
                    UnitCost = l.UnitCost
                }).OrderBy(l => l.ArrivalDate).ToList()
            });
        }

        report.TotalSpecies = report.Items.Count;
        report.TotalStock = report.Items.Sum(i => i.CurrentStock);

        return report;
    }

    public async Task<MortalityReportDto> GetMortalityReportAsync(DateTime? startDate = null, DateTime? endDate = null, int? speciesId = null, int? supplierId = null)
    {
        var speciesList = speciesId.HasValue
            ? (await _speciesRepository.GetByIdAsync(_currentUser.TenantId, speciesId.Value) is { } s ? new[] { s } : Array.Empty<Species>())
            : await _speciesRepository.GetAllAsync(_currentUser.TenantId);

        var report = new MortalityReportDto();

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);

            foreach (var lot in lots)
            {
                if (supplierId.HasValue && lot.SupplierId != supplierId.Value) continue;

                var records = lot.MortalityRecords.AsEnumerable();
                if (startDate.HasValue)
                    records = records.Where(r => r.Date >= startDate.Value);
                if (endDate.HasValue)
                    records = records.Where(r => r.Date <= endDate.Value);

                var recordsList = records.ToList();
                if (recordsList.Count == 0) continue;

                var sold = recordsList.Where(r => string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase)).Sum(r => r.Quantity);
                var otherCauses = recordsList.Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase)).Sum(r => r.Quantity);

                report.Summaries.Add(new MortalitySummaryDto
                {
                    SpeciesId = species.Id,
                    CommonName = species.CommonName,
                    SupplierName = lot.Supplier?.Name,
                    TotalDeaths = sold + otherCauses,
                    Sold = sold,
                    OtherCauses = otherCauses,
                    Records = recordsList
                        .OrderByDescending(r => r.Date)
                        .Select(r => new MortalityRecordDto
                        {
                            RecordId = r.Id,
                            LotId = r.InventoryLotId,
                            Date = r.Date,
                            Quantity = r.Quantity,
                            Cause = r.Cause,
                            Notes = r.Notes
                        }).ToList()
                });
            }
        }

        report.TotalDeaths = report.Summaries.Sum(s => s.TotalDeaths);
        report.TotalSold = report.Summaries.Sum(s => s.Sold);
        report.TotalOtherCauses = report.Summaries.Sum(s => s.OtherCauses);

        return report;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 50)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 50;

        var allSales = await _saleRepository.GetByDateRangeAsync(_currentUser.TenantId, startDate, endDate);
        var report = new SalesReportDto();

        var salesList = allSales.Select(s => new SalesSummaryDto
        {
            SaleId = s.Id,
            Date = s.Date,
            CustomerName = s.CustomerName,
            SaleType = s.SaleType.ToString(),
            TotalAmount = s.Items.Sum(i => i.Quantity * i.UnitPrice),
            ItemCount = s.Items.Count
        }).OrderByDescending(s => s.Date).ToList();

        var paged = salesList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        report.Sales = paged;
        report.TotalRevenue = salesList.Sum(s => s.TotalAmount);
        report.TotalItemsSold = allSales.Sum(s => s.Items.Sum(i => i.Quantity));

        var speciesGroups = allSales
            .SelectMany(s => s.Items)
            .GroupBy(i => new { i.SpeciesId, SpeciesName = i.Species?.CommonName ?? "Unknown" })
            .Select(g => new TopSpeciesDto
            {
                CommonName = g.Key.SpeciesName,
                TotalQuantitySold = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(t => t.TotalQuantitySold)
            .Take(10)
            .ToList();

        report.TopSpecies = speciesGroups;

        return report;
    }

    public async Task<InventoryValuationDto> GetInventoryValuationAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync(_currentUser.TenantId);
        var byCategory = new Dictionary<string, ValuationByCategoryDto>();

        int totalUnits = 0;
        decimal totalCostValue = 0;
        int totalLots = 0;

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);
            var openLots = lots.Where(l => l.GetCurrentStock() > 0).ToList();

            foreach (var lot in openLots)
            {
                var stock = lot.GetCurrentStock();
                var costValue = stock * lot.UnitCost;
                totalUnits += stock;
                totalCostValue += costValue;
                totalLots++;

                if (!byCategory.ContainsKey(species.Category))
                {
                    byCategory[species.Category] = new ValuationByCategoryDto
                    {
                        Category = species.Category,
                        UnitsInStock = 0,
                        TotalCostValue = 0
                    };
                }

                byCategory[species.Category].UnitsInStock += stock;
                byCategory[species.Category].TotalCostValue += costValue;
            }
        }

        foreach (var cat in byCategory.Values)
        {
            cat.AverageUnitCost = cat.UnitsInStock > 0 ? cat.TotalCostValue / cat.UnitsInStock : 0;
        }

        return new InventoryValuationDto
        {
            TotalCostValue = totalCostValue,
            TotalUnitsInStock = totalUnits,
            TotalLots = totalLots,
            AverageUnitCost = totalUnits > 0 ? totalCostValue / totalUnits : 0,
            ByCategory = byCategory.Values.OrderByDescending(c => c.TotalCostValue).ToList()
        };
    }

    public async Task<SupplierPerformanceReportDto> GetSupplierPerformanceAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var suppliers = await _supplierRepository.GetAllWithLotsAsync(_currentUser.TenantId);
        var report = new SupplierPerformanceReportDto();
        var performances = new List<SupplierPerformanceDto>();

        foreach (var supplier in suppliers)
        {
            var lots = supplier.InventoryLots.ToList();
            if (lots.Count == 0) continue;

            var totalDOA = lots.Sum(l => l.DeadOnArrival);
            var mortalityRecords = lots.SelectMany(l => l.MortalityRecords).AsEnumerable();

            if (startDate.HasValue)
                mortalityRecords = mortalityRecords.Where(r => r.Date >= startDate.Value);
            if (endDate.HasValue)
                mortalityRecords = mortalityRecords.Where(r => r.Date <= endDate.Value);

            var recordsList = mortalityRecords.ToList();
            var nonSoldMortality = recordsList
                .Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.Quantity);

            var totalInitial = lots.Sum(l => l.InitialQuantity);
            var viableInitial = totalInitial - totalDOA;
            var mortalityRate = viableInitial > 0
                ? Math.Round((decimal)nonSoldMortality / viableInitial * 100, 1)
                : 0;

            var costLost = recordsList
                .Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.Quantity * r.InventoryLot.UnitCost);

            performances.Add(new SupplierPerformanceDto
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                TotalLotsReceived = lots.Count,
                TotalDOA = totalDOA,
                NonSoldMortality = nonSoldMortality,
                CostLostToMortality = costLost,
                MortalityRatePercent = mortalityRate
            });
        }

        var ranked = performances
            .OrderByDescending(p => p.MortalityRatePercent)
            .Select((p, i) => { p.Rank = i + 1; return p; })
            .ToList();

        report.Suppliers = ranked;
        report.TotalCostLost = ranked.Sum(p => p.CostLostToMortality);
        report.AverageMortalityRate = ranked.Count > 0
            ? Math.Round(ranked.Average(p => p.MortalityRatePercent), 1)
            : 0;

        return report;
    }

    public async Task<InventoryTurnoverReportDto> GetInventoryTurnoverAsync(int? speciesId = null, int? supplierId = null)
    {
        var speciesList = speciesId.HasValue
            ? (await _speciesRepository.GetByIdAsync(_currentUser.TenantId, speciesId.Value) is { } s ? new[] { s } : Array.Empty<Species>())
            : await _speciesRepository.GetAllAsync(_currentUser.TenantId);

        var report = new InventoryTurnoverReportDto();
        var today = DateTime.UtcNow;

        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);
            foreach (var lot in lots)
            {
                if (lot.GetCurrentStock() <= 0) continue;
                if (supplierId.HasValue && lot.SupplierId != supplierId.Value) continue;

                var daysInStock = (today - lot.ArrivalDate).Days;
                var soldQuantity = lot.MortalityRecords
                    .Where(r => string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase))
                    .Sum(r => r.Quantity);

                string agingStatus;
                if (daysInStock < 30) agingStatus = "Fresh";
                else if (daysInStock <= 90) agingStatus = "Aging";
                else agingStatus = "Old";

                report.Lots.Add(new InventoryTurnoverDto
                {
                    LotId = lot.Id,
                    SpeciesName = species.CommonName,
                    SupplierName = lot.Supplier?.Name,
                    ArrivalDate = lot.ArrivalDate,
                    DaysInStock = daysInStock,
                    CurrentStock = lot.GetCurrentStock(),
                    InitialQuantity = lot.InitialQuantity,
                    SoldQuantity = soldQuantity,
                    UnitCost = lot.UnitCost,
                    AgingStatus = agingStatus,
                    CostAtRisk = lot.GetCurrentStock() * lot.UnitCost
                });
            }
        }

        report.Lots = report.Lots.OrderByDescending(l => l.DaysInStock).ToList();
        report.FreshLots = report.Lots.Count(l => l.AgingStatus == "Fresh");
        report.AgingLots = report.Lots.Count(l => l.AgingStatus == "Aging");
        report.OldLots = report.Lots.Count(l => l.AgingStatus == "Old");
        report.AverageDaysInStock = report.Lots.Count > 0
            ? Math.Round(report.Lots.Average(l => (decimal)l.DaysInStock), 1)
            : 0;

        return report;
    }

    public async Task<ProfitabilityReportDto> GetProfitabilityReportAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(_currentUser.TenantId, startDate, endDate);
        var speciesList = await _speciesRepository.GetAllAsync(_currentUser.TenantId);

        var revenueBySpecies = new Dictionary<int, (decimal Revenue, int Quantity)>();
        foreach (var sale in sales)
        {
            foreach (var item in sale.Items)
            {
                if (!revenueBySpecies.ContainsKey(item.SpeciesId))
                    revenueBySpecies[item.SpeciesId] = (0, 0);

                var (rev, qty) = revenueBySpecies[item.SpeciesId];
                revenueBySpecies[item.SpeciesId] = (rev + item.Quantity * item.UnitPrice, qty + item.Quantity);
            }
        }

        var costBySpecies = new Dictionary<int, decimal>();
        foreach (var species in speciesList)
        {
            var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);
            foreach (var lot in lots)
            {
                var soldRecords = lot.MortalityRecords
                    .Where(r => r.Date >= startDate && r.Date <= endDate
                             && string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase));

                foreach (var record in soldRecords)
                {
                    if (!costBySpecies.ContainsKey(species.Id))
                        costBySpecies[species.Id] = 0;

                    costBySpecies[species.Id] += record.Quantity * lot.UnitCost;
                }
            }
        }

        var report = new ProfitabilityReportDto();

        foreach (var species in speciesList)
        {
            if (!revenueBySpecies.ContainsKey(species.Id) && !costBySpecies.ContainsKey(species.Id))
                continue;

            var (revenue, quantity) = revenueBySpecies.GetValueOrDefault(species.Id);
            var cost = costBySpecies.GetValueOrDefault(species.Id);
            var profit = revenue - cost;
            var marginPercent = revenue > 0 ? Math.Round(profit / revenue * 100, 1) : 0;

            report.BySpecies.Add(new ProfitabilityBySpeciesDto
            {
                SpeciesId = species.Id,
                CommonName = species.CommonName,
                Category = species.Category,
                QuantitySold = quantity,
                Revenue = revenue,
                Cost = cost,
                Profit = profit,
                MarginPercent = marginPercent
            });
        }

        report.BySpecies = report.BySpecies.OrderByDescending(s => s.Profit).ToList();
        report.TotalRevenue = report.BySpecies.Sum(s => s.Revenue);
        report.TotalCost = report.BySpecies.Sum(s => s.Cost);
        report.GrossProfit = report.TotalRevenue - report.TotalCost;
        report.ProfitMarginPercent = report.TotalRevenue > 0
            ? Math.Round(report.GrossProfit / report.TotalRevenue * 100, 1)
            : 0;

        return report;
    }
}
