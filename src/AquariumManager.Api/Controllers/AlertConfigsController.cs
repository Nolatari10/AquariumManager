using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertConfigsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertConfigsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConfigs()
    {
        var configs = await _alertService.GetAllConfigsAsync();
        return Ok(configs);
    }

    [HttpGet("{alertType}")]
    public async Task<IActionResult> GetByAlertType(string alertType)
    {
        var config = await _alertService.GetConfigByAlertTypeAsync(alertType);
        if (config is null)
            return NotFound();
        return Ok(config);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateConfig(int id, UpdateAlertConfigDto dto)
    {
        var result = await _alertService.UpdateConfigAsync(id, dto);
        if (!result.Success)
            return NotFound(result.ErrorMessage);
        return Ok(result.Data);
    }

    [HttpGet("high-mortality-alerts")]
    public async Task<IActionResult> GetActiveHighMortalityAlerts()
    {
        var alerts = await _alertService.GetActiveHighMortalityAlertsAsync();
        return Ok(alerts);
    }
}
