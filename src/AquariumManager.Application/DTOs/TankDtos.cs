using AquariumManager.Domain.Entities;

namespace AquariumManager.Application.DTOs;

public record CreateTankDto
{
    public string Name { get; set; } = string.Empty;
    public decimal SizeLiters { get; set; }
    public TankType TankType { get; set; }
    public string? Substrate { get; set; }
    public bool Co2Injection { get; set; }
    public string? LightDescription { get; set; }
    public string? FilterDescription { get; set; }
    public decimal? HeaterSetpointCelsius { get; set; }
}

public record UpdateTankDto
{
    public string Name { get; set; } = string.Empty;
    public decimal SizeLiters { get; set; }
    public TankType TankType { get; set; }
    public string? Substrate { get; set; }
    public bool Co2Injection { get; set; }
    public string? LightDescription { get; set; }
    public string? FilterDescription { get; set; }
    public decimal? HeaterSetpointCelsius { get; set; }
    public bool? IsActive { get; set; }
}

public record TankDto
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal SizeLiters { get; set; }
    public string TankType { get; set; } = string.Empty;
    public string? Substrate { get; set; }
    public bool Co2Injection { get; set; }
    public string? LightDescription { get; set; }
    public string? FilterDescription { get; set; }
    public decimal? HeaterSetpointCelsius { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Quick indicators
    public DateTime? LastWaterTestAt { get; set; }
    public DateTime? LastDoseAt { get; set; }
    public DateTime? LastMaintenanceAt { get; set; }
    public int PhotoCount { get; set; }
    public int TotalLogs { get; set; }
}

public record TankSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TankType { get; set; } = string.Empty;
    public decimal SizeLiters { get; set; }
    public string OwnerEmail { get; set; } = string.Empty;
    public DateTime? LastWaterTestAt { get; set; }
    public string? LastWaterTestSummary { get; set; }
    public DateTime? LastDoseAt { get; set; }
    public string? LastDoseSummary { get; set; }
    public DateTime? LastMaintenanceAt { get; set; }
    public string? LastMaintenanceSummary { get; set; }
    public bool IsActive { get; set; }
    public int WarningCount { get; set; }
}

public record CreateWaterParameterLogDto
{
    public DateTime MeasuredAt { get; set; }
    public decimal? pH { get; set; }
    public decimal? TemperatureCelsius { get; set; }
    public decimal? AmmoniaPpm { get; set; }
    public decimal? NitritePpm { get; set; }
    public decimal? NitratePpm { get; set; }
    public decimal? PhosphatePpm { get; set; }
    public decimal? PotassiumPpm { get; set; }
    public decimal? IronPpm { get; set; }
    public decimal? GeneralHardness { get; set; }
    public decimal? CarbonateHardness { get; set; }
    public int? TdsPpm { get; set; }
    public decimal? Co2Ppm { get; set; }
    public decimal? SalinityPpt { get; set; }
    public string? Notes { get; set; }
}

public record WaterParameterLogDto
{
    public int Id { get; set; }
    public int TankId { get; set; }
    public DateTime MeasuredAt { get; set; }
    public decimal? pH { get; set; }
    public decimal? TemperatureCelsius { get; set; }
    public decimal? AmmoniaPpm { get; set; }
    public decimal? NitritePpm { get; set; }
    public decimal? NitratePpm { get; set; }
    public decimal? PhosphatePpm { get; set; }
    public decimal? PotassiumPpm { get; set; }
    public decimal? IronPpm { get; set; }
    public decimal? GeneralHardness { get; set; }
    public decimal? CarbonateHardness { get; set; }
    public int? TdsPpm { get; set; }
    public decimal? Co2Ppm { get; set; }
    public decimal? SalinityPpt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateMaintenanceLogDto
{
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime PerformedAt { get; set; }
    public int? WaterChangePercent { get; set; }
    public decimal? WaterChangeLiters { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public int? ReminderFrequencyDays { get; set; }
}

public record MaintenanceLogDto
{
    public int Id { get; set; }
    public int TankId { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public int? WaterChangePercent { get; set; }
    public decimal? WaterChangeLiters { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public int? ReminderFrequencyDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateFertilizationLogDto
{
    public DateTime DosedAt { get; set; }
    public decimal DoseAmount { get; set; }
    public DoseUnit DoseUnit { get; set; }
    public FertilizerType FertilizerType { get; set; }
    public int? FertilizerPresetId { get; set; }
    public decimal? EstimatedNitratePpm { get; set; }
    public decimal? EstimatedPhosphatePpm { get; set; }
    public decimal? EstimatedPotassiumPpm { get; set; }
    public decimal? EstimatedIronPpm { get; set; }
    public bool IsScheduled { get; set; } = true;
    public bool IsAdjustment { get; set; }
    public string? Notes { get; set; }
}

public record FertilizationLogDto
{
    public int Id { get; set; }
    public int TankId { get; set; }
    public int? FertilizerPresetId { get; set; }
    public string? FertilizerPresetName { get; set; }
    public DateTime DosedAt { get; set; }
    public decimal DoseAmount { get; set; }
    public string DoseUnit { get; set; } = string.Empty;
    public string FertilizerType { get; set; } = string.Empty;
    public decimal? EstimatedNitratePpm { get; set; }
    public decimal? EstimatedPhosphatePpm { get; set; }
    public decimal? EstimatedPotassiumPpm { get; set; }
    public decimal? EstimatedIronPpm { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsAdjustment { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateTankPhotoDto
{
    public DateTime TakenAt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public LinkedLogType? LinkedLogType { get; set; }
    public int? LinkedLogId { get; set; }
}

public record TankPhotoDto
{
    public int Id { get; set; }
    public int TankId { get; set; }
    public DateTime TakenAt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? LinkedLogType { get; set; }
    public int? LinkedLogId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateFertilizerPresetDto
{
    public string Name { get; set; } = string.Empty;
    public FertilizerType FertilizerType { get; set; }
    public decimal DefaultDoseAmount { get; set; }
    public DoseUnit DefaultDoseUnit { get; set; }
    public decimal? NitratePerDose { get; set; }
    public decimal? PhosphatePerDose { get; set; }
    public decimal? PotassiumPerDose { get; set; }
    public decimal? IronPerDose { get; set; }
    public string? Notes { get; set; }
}

public record FertilizerPresetDto
{
    public int Id { get; set; }
    public int? OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FertilizerType { get; set; } = string.Empty;
    public decimal DefaultDoseAmount { get; set; }
    public string DefaultDoseUnit { get; set; } = string.Empty;
    public decimal? NitratePerDose { get; set; }
    public decimal? PhosphatePerDose { get; set; }
    public decimal? PotassiumPerDose { get; set; }
    public decimal? IronPerDose { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

public record TargetParameterRangeDto
{
    public string ParameterName { get; set; } = string.Empty;
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public record UpsertTargetRangesDto
{
    public List<TargetParameterRangeDto> Ranges { get; set; } = new();
}

public record TimelineEntryDto
{
    public string EntryType { get; set; } = string.Empty; // "WaterParameter", "Maintenance", "Fertilization", "Photo"
    public DateTime Timestamp { get; set; }
    public object? Data { get; set; }
}

public record TankTrendsDto
{
    public List<WaterParameterLogDto> Parameters { get; set; } = new();
    public List<FertilizationLogDto> Doses { get; set; } = new();
}
