using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

// Coordinates all sub-repositories for a tank: water params, maintenance, fertilization, photos, and target ranges.
// Each tank detail DTO is enriched with latest-activity indicators (last test, last dose, last maintenance, photo count).
public class TankService : ITankService
{
    private readonly ITankRepository _tankRepo;
    private readonly IWaterParameterLogRepository _waterRepo;
    private readonly IMaintenanceLogRepository _maintenanceRepo;
    private readonly IFertilizationLogRepository _fertilizationRepo;
    private readonly ITankPhotoRepository _photoRepo;
    private readonly ITargetParameterRangeRepository _targetRepo;
    private readonly IFertilizerPresetRepository _presetRepo;
    private readonly ICurrentUserService _currentUser;

    public TankService(
        ITankRepository tankRepo,
        IWaterParameterLogRepository waterRepo,
        IMaintenanceLogRepository maintenanceRepo,
        IFertilizationLogRepository fertilizationRepo,
        ITankPhotoRepository photoRepo,
        ITargetParameterRangeRepository targetRepo,
        IFertilizerPresetRepository presetRepo,
        ICurrentUserService currentUser)
    {
        _tankRepo = tankRepo;
        _waterRepo = waterRepo;
        _maintenanceRepo = maintenanceRepo;
        _fertilizationRepo = fertilizationRepo;
        _photoRepo = photoRepo;
        _targetRepo = targetRepo;
        _presetRepo = presetRepo;
        _currentUser = currentUser;
    }

    // Creates a tank and returns basic info (no log indicators since it's new)
    public async Task<TankDto> CreateAsync(int ownerUserId, CreateTankDto dto)
    {
        var tank = new Tank(
            ownerUserId, dto.Name, dto.SizeLiters, dto.TankType,
            dto.Substrate, dto.Co2Injection, dto.LightDescription,
            dto.FilterDescription, dto.HeaterSetpointCelsius);

        tank.TenantId = _currentUser.TenantId;
        await _tankRepo.AddAsync(tank);
        return MapToDto(tank);
    }

    // Returns full tank detail with latest-activity indicators populated
    public async Task<TankDto?> GetByIdAsync(int id)
    {
        var tank = await _tankRepo.GetByIdAsync(_currentUser.TenantId, id);
        if (tank is null) return null;

        var dto = MapToDto(tank);
        await PopulateIndicators(tank, dto);
        return dto;
    }

    // Returns list of tanks belonging to a specific owner, each with summary indicators
    public async Task<IReadOnlyList<TankSummaryDto>> GetForOwnerAsync(int ownerUserId)
    {
        var tanks = await _tankRepo.GetByOwnerAsync(_currentUser.TenantId, ownerUserId);
        var result = new List<TankSummaryDto>();
        foreach (var tank in tanks)
            result.Add(await BuildSummary(tank));
        return result;
    }

    // Filters by owner, tank type, and active status; each result includes latest-activity summary
    public async Task<IReadOnlyList<TankSummaryDto>> GetAllAsync(int? ownerUserId = null, string? tankType = null, bool? isActive = null)
    {
        Domain.Entities.TankType? typeFilter = null;
        if (!string.IsNullOrWhiteSpace(tankType) && Enum.TryParse<Domain.Entities.TankType>(tankType, true, out var parsed))
            typeFilter = parsed;

        var tanks = await _tankRepo.GetAllAsync(_currentUser.TenantId, ownerUserId, typeFilter, isActive);
        var result = new List<TankSummaryDto>();
        foreach (var tank in tanks)
            result.Add(await BuildSummary(tank));
        return result;
    }

    public async Task UpdateAsync(int id, UpdateTankDto dto)
    {
        var tank = await _tankRepo.GetByIdAsync(_currentUser.TenantId, id)
            ?? throw new InvalidOperationException("Tank not found.");

        tank.UpdateInfo(dto.Name, dto.SizeLiters, dto.TankType, dto.Substrate,
            dto.Co2Injection, dto.LightDescription, dto.FilterDescription, dto.HeaterSetpointCelsius);

        if (dto.IsActive.HasValue && !dto.IsActive.Value)
            tank.Deactivate();

        await _tankRepo.UpdateAsync(tank);
    }

    // Soft-deletes by setting IsActive=false
    public async Task DeleteAsync(int id)
    {
        await _tankRepo.DeleteAsync(id);
    }

    // Persists a new water test result (nullable fields — users log only what they measure)
    public async Task<WaterParameterLogDto> AddWaterParameterAsync(int tankId, CreateWaterParameterLogDto dto)
    {
        var log = new WaterParameterLog(
            tankId, dto.MeasuredAt, dto.pH, dto.TemperatureCelsius, dto.AmmoniaPpm,
            dto.NitritePpm, dto.NitratePpm, dto.PhosphatePpm, dto.PotassiumPpm,
            dto.IronPpm, dto.GeneralHardness, dto.CarbonateHardness, dto.TdsPpm,
            dto.Co2Ppm, dto.SalinityPpt, dto.Notes);

        await _waterRepo.AddAsync(log);
        return MapWaterDto(log);
    }

    public async Task<IReadOnlyList<WaterParameterLogDto>> GetWaterParametersAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50)
    {
        var logs = await _waterRepo.GetByTankAsync(tankId, from, to, page, pageSize);
        return logs.Select(MapWaterDto).ToList();
    }

    // Stores a maintenance event with optional reminder frequency
    public async Task<MaintenanceLogDto> AddMaintenanceAsync(int tankId, CreateMaintenanceLogDto dto)
    {
        var log = new MaintenanceLog(
            tankId, dto.MaintenanceType, dto.PerformedAt, dto.WaterChangePercent,
            dto.WaterChangeLiters, dto.DurationMinutes, dto.Notes, dto.ReminderFrequencyDays);

        await _maintenanceRepo.AddAsync(log);
        return MapMaintenanceDto(log);
    }

    public async Task<IReadOnlyList<MaintenanceLogDto>> GetMaintenanceLogsAsync(int tankId, DateTime? from = null, DateTime? to = null, string? type = null, int page = 1, int pageSize = 50)
    {
        MaintenanceType? typeFilter = null;
        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<MaintenanceType>(type, true, out var parsed))
            typeFilter = parsed;

        var logs = await _maintenanceRepo.GetByTankAsync(tankId, from, to, typeFilter, page, pageSize);
        return logs.Select(MapMaintenanceDto).ToList();
    }

    // Records a fertilizer dose with estimated ppm added to the water column (NO₃, PO₄, K, Fe).
    // Flags distinguish routine scheduled doses from deliberate one-off adjustments.
    public async Task<FertilizationLogDto> AddFertilizationAsync(int tankId, CreateFertilizationLogDto dto)
    {
        var log = new FertilizationLog(
            tankId, dto.DosedAt, dto.DoseAmount, dto.DoseUnit, dto.FertilizerType,
            dto.FertilizerPresetId, dto.EstimatedNitratePpm, dto.EstimatedPhosphatePpm,
            dto.EstimatedPotassiumPpm, dto.EstimatedIronPpm, dto.IsScheduled, dto.IsAdjustment, dto.Notes);

        await _fertilizationRepo.AddAsync(log);
        return MapFertilizationDto(log);
    }

    public async Task<IReadOnlyList<FertilizationLogDto>> GetFertilizationLogsAsync(int tankId, DateTime? from = null, DateTime? to = null, string? type = null, int page = 1, int pageSize = 50)
    {
        FertilizerType? typeFilter = null;
        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<FertilizerType>(type, true, out var parsed))
            typeFilter = parsed;

        var logs = await _fertilizationRepo.GetByTankAsync(tankId, from, to, typeFilter, page, pageSize);
        return logs.Select(MapFertilizationDto).ToList();
    }

    // Photo can be optionally linked to a log entry (water test, maintenance, or dose) via polymorphic FK
    public async Task<TankPhotoDto> AddPhotoAsync(int tankId, CreateTankPhotoDto dto)
    {
        var photo = new TankPhoto(tankId, dto.TakenAt, dto.ImageUrl, dto.Caption, dto.LinkedLogType, dto.LinkedLogId);
        await _photoRepo.AddAsync(photo);
        return MapPhotoDto(photo);
    }

    public async Task<IReadOnlyList<TankPhotoDto>> GetPhotosAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20)
    {
        var photos = await _photoRepo.GetByTankAsync(tankId, from, to, page, pageSize);
        return photos.Select(MapPhotoDto).ToList();
    }

    public async Task DeletePhotoAsync(int photoId)
    {
        await _photoRepo.DeleteAsync(photoId);
    }

    // Returns parameter measurements and dosing events in chronological order, suitable for chart overlays
    public async Task<TankTrendsDto> GetTrendsAsync(int tankId, DateTime? from = null, DateTime? to = null)
    {
        var parameters = await _waterRepo.GetByTankAsync(tankId, from, to, 1, 500);
        var doses = await _fertilizationRepo.GetByTankAsync(tankId, from, to, null, 1, 500);

        return new TankTrendsDto
        {
            Parameters = parameters.OrderBy(p => p.MeasuredAt).Select(MapWaterDto).ToList(),
            Doses = doses.OrderBy(d => d.DosedAt).Select(MapFertilizationDto).ToList()
        };
    }

    // Merges water tests, maintenance, fertilization, and photos into a single reverse-chronological feed.
    // Fetches a balanced slice from each type so the timeline isn't biased toward one category.
    public async Task<IReadOnlyList<TimelineEntryDto>> GetTimelineAsync(int tankId, int page = 1, int pageSize = 50)
    {
        var entries = new List<TimelineEntryDto>();

        var waterLogs = await _waterRepo.GetByTankAsync(tankId, null, null, page, pageSize / 4 + 1);
        foreach (var w in waterLogs)
            entries.Add(new TimelineEntryDto { EntryType = "WaterParameter", Timestamp = w.MeasuredAt, Data = MapWaterDto(w) });

        var maintenanceLogs = await _maintenanceRepo.GetByTankAsync(tankId, null, null, null, page, pageSize / 4 + 1);
        foreach (var m in maintenanceLogs)
            entries.Add(new TimelineEntryDto { EntryType = "Maintenance", Timestamp = m.PerformedAt, Data = MapMaintenanceDto(m) });

        var fertLogs = await _fertilizationRepo.GetByTankAsync(tankId, null, null, null, page, pageSize / 4 + 1);
        foreach (var f in fertLogs)
            entries.Add(new TimelineEntryDto { EntryType = "Fertilization", Timestamp = f.DosedAt, Data = MapFertilizationDto(f) });

        var photos = await _photoRepo.GetByTankAsync(tankId, null, null, page, pageSize / 4 + 1);
        foreach (var p in photos)
            entries.Add(new TimelineEntryDto { EntryType = "Photo", Timestamp = p.TakenAt, Data = MapPhotoDto(p) });

        return entries.OrderByDescending(e => e.Timestamp).Take(pageSize).ToList();
    }

    public async Task<IReadOnlyList<TargetParameterRangeDto>> GetTargetRangesAsync(int tankId)
    {
        var ranges = await _targetRepo.GetByTankAsync(tankId);
        return ranges.Select(r => new TargetParameterRangeDto
        {
            ParameterName = r.ParameterName.ToString(),
            MinValue = r.MinValue,
            MaxValue = r.MaxValue,
            Unit = r.Unit
        }).ToList();
    }

    // Replaces all target ranges for a tank atomically (delete old + insert new)
    public async Task UpsertTargetRangesAsync(int tankId, List<TargetParameterRangeDto> ranges)
    {
        var data = ranges.Select(r =>
        {
            Enum.TryParse<ParameterName>(r.ParameterName, out var name);
            return (name, r.MinValue, r.MaxValue, r.Unit);
        }).ToList();

        await _targetRepo.UpsertAsync(tankId, data);
    }

    // Builds a list-dashboard summary: latest test, dose, and maintenance for a single tank
    private async Task<TankSummaryDto> BuildSummary(Tank tank)
    {
        var summary = new TankSummaryDto
        {
            Id = tank.Id, Name = tank.Name, TankType = tank.TankType.ToString(),
            SizeLiters = tank.SizeLiters, OwnerEmail = tank.OwnerUser?.Email ?? string.Empty,
            IsActive = tank.IsActive
        };

        var waterLogs = await _waterRepo.GetByTankAsync(tank.Id, null, null, 1, 1);
        if (waterLogs.Any())
        {
            var last = waterLogs.First();
            summary.LastWaterTestAt = last.MeasuredAt;
            summary.LastWaterTestSummary = $"pH: {last.pH}, NO₃: {last.NitratePpm}";
        }

        var fertLogs = await _fertilizationRepo.GetByTankAsync(tank.Id, null, null, null, 1, 1);
        if (fertLogs.Any())
        {
            var last = fertLogs.First();
            summary.LastDoseAt = last.DosedAt;
            summary.LastDoseSummary = $"{last.FertilizerPreset?.Name ?? "Custom"} {last.DoseAmount}{last.DoseUnit}";
        }

        var maintLogs = await _maintenanceRepo.GetByTankAsync(tank.Id, null, null, null, 1, 1);
        if (maintLogs.Any())
        {
            var last = maintLogs.First();
            summary.LastMaintenanceAt = last.PerformedAt;
            summary.LastMaintenanceSummary = $"{last.MaintenanceType}";
        }

        return summary;
    }

    // Fills in LastWaterTestAt, LastDoseAt, LastMaintenanceAt, and PhotoCount on the detail DTO
    private async Task PopulateIndicators(Tank tank, TankDto dto)
    {
        var waterLogs = await _waterRepo.GetByTankAsync(tank.Id, null, null, 1, 1);
        if (waterLogs.Any()) dto.LastWaterTestAt = waterLogs.First().MeasuredAt;

        var fertLogs = await _fertilizationRepo.GetByTankAsync(tank.Id, null, null, null, 1, 1);
        if (fertLogs.Any()) dto.LastDoseAt = fertLogs.First().DosedAt;

        var maintLogs = await _maintenanceRepo.GetByTankAsync(tank.Id, null, null, null, 1, 1);
        if (maintLogs.Any()) dto.LastMaintenanceAt = maintLogs.First().PerformedAt;

        var photos = await _photoRepo.GetByTankAsync(tank.Id, null, null, 1, 1000);
        dto.PhotoCount = photos.Count;
    }

    private static TankDto MapToDto(Tank t) => new()
    {
        Id = t.Id, OwnerUserId = t.OwnerUserId, OwnerEmail = t.OwnerUser?.Email ?? string.Empty,
        Name = t.Name, SizeLiters = t.SizeLiters, TankType = t.TankType.ToString(),
        Substrate = t.Substrate, Co2Injection = t.Co2Injection,
        LightDescription = t.LightDescription, FilterDescription = t.FilterDescription,
        HeaterSetpointCelsius = t.HeaterSetpointCelsius, IsActive = t.IsActive,
        CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt
    };

    private static WaterParameterLogDto MapWaterDto(WaterParameterLog w) => new()
    {
        Id = w.Id, TankId = w.TankId, MeasuredAt = w.MeasuredAt,
        pH = w.pH, TemperatureCelsius = w.TemperatureCelsius,
        AmmoniaPpm = w.AmmoniaPpm, NitritePpm = w.NitritePpm, NitratePpm = w.NitratePpm,
        PhosphatePpm = w.PhosphatePpm, PotassiumPpm = w.PotassiumPpm, IronPpm = w.IronPpm,
        GeneralHardness = w.GeneralHardness, CarbonateHardness = w.CarbonateHardness,
        TdsPpm = w.TdsPpm, Co2Ppm = w.Co2Ppm, SalinityPpt = w.SalinityPpt,
        Notes = w.Notes, CreatedAt = w.CreatedAt
    };

    private static MaintenanceLogDto MapMaintenanceDto(MaintenanceLog m) => new()
    {
        Id = m.Id, TankId = m.TankId, MaintenanceType = m.MaintenanceType.ToString(),
        PerformedAt = m.PerformedAt, WaterChangePercent = m.WaterChangePercent,
        WaterChangeLiters = m.WaterChangeLiters, DurationMinutes = m.DurationMinutes,
        Notes = m.Notes, ReminderFrequencyDays = m.ReminderFrequencyDays, CreatedAt = m.CreatedAt
    };

    private static FertilizationLogDto MapFertilizationDto(FertilizationLog f) => new()
    {
        Id = f.Id, TankId = f.TankId, FertilizerPresetId = f.FertilizerPresetId,
        FertilizerPresetName = f.FertilizerPreset?.Name, DosedAt = f.DosedAt,
        DoseAmount = f.DoseAmount, DoseUnit = f.DoseUnit.ToString(), FertilizerType = f.FertilizerType.ToString(),
        EstimatedNitratePpm = f.EstimatedNitratePpm, EstimatedPhosphatePpm = f.EstimatedPhosphatePpm,
        EstimatedPotassiumPpm = f.EstimatedPotassiumPpm, EstimatedIronPpm = f.EstimatedIronPpm,
        IsScheduled = f.IsScheduled, IsAdjustment = f.IsAdjustment, Notes = f.Notes, CreatedAt = f.CreatedAt
    };

    private static TankPhotoDto MapPhotoDto(TankPhoto p) => new()
    {
        Id = p.Id, TankId = p.TankId, TakenAt = p.TakenAt, ImageUrl = p.ImageUrl,
        Caption = p.Caption, LinkedLogType = p.LinkedLogType?.ToString(), LinkedLogId = p.LinkedLogId,
        CreatedAt = p.CreatedAt
    };
}
