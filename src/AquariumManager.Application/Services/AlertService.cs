using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertConfigRepository _configRepository;
    private readonly IInventoryLotRepository _lotRepository;
    private readonly ICurrentUserService _currentUser;
    private const string HighMortalityAlertType = "HighMortalityRate";

    public AlertService(
        IAlertConfigRepository configRepository,
        IInventoryLotRepository lotRepository,
        ICurrentUserService currentUser)
    {
        _configRepository = configRepository;
        _lotRepository = lotRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<HighMortalityAlertDto>> GetActiveHighMortalityAlertsAsync()
    {
        var config = await _configRepository.GetByAlertTypeAsync(_currentUser.TenantId, HighMortalityAlertType);
        var threshold = config is { IsEnabled: true } ? config.ThresholdValue : 15m;

        var lots = await _lotRepository.GetAllAsync(_currentUser.TenantId);
        var alerts = new List<HighMortalityAlertDto>();
        var today = DateTime.UtcNow;

        foreach (var lot in lots)
        {
            if (lot.GetCurrentStock() <= 0) continue;

            var nonSoldMortality = lot.MortalityRecords
                .Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.Quantity);

            var viableInitial = lot.InitialQuantity - lot.DeadOnArrival;
            var mortalityRate = viableInitial > 0
                ? Math.Round((decimal)nonSoldMortality / viableInitial * 100, 1)
                : 0;

            if (mortalityRate < threshold) continue;

            var costLost = lot.MortalityRecords
                .Where(r => !string.Equals(r.Cause, "Sold", StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.Quantity * lot.UnitCost);

            var variantName = lot.SpeciesVariant?.VariantName ?? string.Empty;
            var speciesName = lot.SpeciesVariant?.Species?.CommonName ?? variantName;

            alerts.Add(new HighMortalityAlertDto
            {
                LotId = lot.Id,
                SpeciesName = speciesName,
                SupplierName = lot.Supplier?.Name,
                VariantName = variantName,
                ArrivalDate = lot.ArrivalDate,
                CurrentStock = lot.GetCurrentStock(),
                InitialQuantity = lot.InitialQuantity,
                DeadOnArrival = lot.DeadOnArrival,
                NonSoldMortality = nonSoldMortality,
                MortalityRatePercent = mortalityRate,
                UnitCost = lot.UnitCost,
                CostLost = costLost,
                DaysSinceArrival = (today - lot.ArrivalDate).Days,
                ThresholdExceeded = true
            });
        }

        return alerts
            .OrderByDescending(a => a.MortalityRatePercent)
            .ThenByDescending(a => a.CostLost)
            .ToList();
    }

    public async Task<IReadOnlyList<AlertConfigDto>> GetAllConfigsAsync()
    {
        var configs = await _configRepository.GetAllAsync(_currentUser.TenantId);
        return configs.Select(MapToDto).ToList();
    }

    public async Task<AlertConfigDto?> GetConfigByAlertTypeAsync(string alertType)
    {
        var config = await _configRepository.GetByAlertTypeAsync(_currentUser.TenantId, alertType);
        return config is null ? null : MapToDto(config);
    }

    public async Task<OperationResult<AlertConfigDto>> UpdateConfigAsync(int id, UpdateAlertConfigDto dto)
    {
        var config = await _configRepository.GetByIdAsync(_currentUser.TenantId, id);
        if (config is null)
            return OperationResult<AlertConfigDto>.Fail("Alert config not found.");

        if (config.TenantId != _currentUser.TenantId)
            return OperationResult<AlertConfigDto>.Fail("Cross-tenant access denied.");

        config.ThresholdValue = dto.ThresholdValue;
        config.IsEnabled = dto.IsEnabled;

        await _configRepository.UpdateAsync(config);

        return OperationResult<AlertConfigDto>.Ok(MapToDto(config));
    }

    private static AlertConfigDto MapToDto(AlertConfig config)
    {
        return new AlertConfigDto
        {
            Id = config.Id,
            AlertType = config.AlertType,
            ThresholdValue = config.ThresholdValue,
            IsEnabled = config.IsEnabled
        };
    }
}
