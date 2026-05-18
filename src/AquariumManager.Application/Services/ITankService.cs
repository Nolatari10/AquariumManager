using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ITankService
{
    Task<TankDto> CreateAsync(int ownerUserId, CreateTankDto dto);
    Task<TankDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<TankSummaryDto>> GetForOwnerAsync(int ownerUserId);
    Task<IReadOnlyList<TankSummaryDto>> GetAllAsync(int? ownerUserId = null, string? tankType = null, bool? isActive = null);
    Task UpdateAsync(int id, UpdateTankDto dto);
    Task DeleteAsync(int id);

    // Water parameter logs
    Task<WaterParameterLogDto> AddWaterParameterAsync(int tankId, CreateWaterParameterLogDto dto);
    Task<IReadOnlyList<WaterParameterLogDto>> GetWaterParametersAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50);

    // Maintenance logs
    Task<MaintenanceLogDto> AddMaintenanceAsync(int tankId, CreateMaintenanceLogDto dto);
    Task<IReadOnlyList<MaintenanceLogDto>> GetMaintenanceLogsAsync(int tankId, DateTime? from = null, DateTime? to = null, string? type = null, int page = 1, int pageSize = 50);

    // Fertilization logs
    Task<FertilizationLogDto> AddFertilizationAsync(int tankId, CreateFertilizationLogDto dto);
    Task<IReadOnlyList<FertilizationLogDto>> GetFertilizationLogsAsync(int tankId, DateTime? from = null, DateTime? to = null, string? type = null, int page = 1, int pageSize = 50);

    // Photos
    Task<TankPhotoDto> AddPhotoAsync(int tankId, CreateTankPhotoDto dto);
    Task<IReadOnlyList<TankPhotoDto>> GetPhotosAsync(int tankId, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20);
    Task DeletePhotoAsync(int photoId);

    // Trends and timeline
    Task<TankTrendsDto> GetTrendsAsync(int tankId, DateTime? from = null, DateTime? to = null);
    Task<IReadOnlyList<TimelineEntryDto>> GetTimelineAsync(int tankId, int page = 1, int pageSize = 50);

    // Target ranges
    Task<IReadOnlyList<TargetParameterRangeDto>> GetTargetRangesAsync(int tankId);
    Task UpsertTargetRangesAsync(int tankId, List<TargetParameterRangeDto> ranges);
}
