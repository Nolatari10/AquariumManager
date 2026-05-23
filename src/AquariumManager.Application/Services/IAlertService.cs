using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface IAlertService
{
    Task<IReadOnlyList<HighMortalityAlertDto>> GetActiveHighMortalityAlertsAsync();
    Task<IReadOnlyList<AlertConfigDto>> GetAllConfigsAsync();
    Task<AlertConfigDto?> GetConfigByAlertTypeAsync(string alertType);
    Task<OperationResult<AlertConfigDto>> UpdateConfigAsync(int id, UpdateAlertConfigDto dto);
}
