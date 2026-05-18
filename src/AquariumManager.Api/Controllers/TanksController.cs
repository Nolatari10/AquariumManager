using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TanksController : ControllerBase
{
    private readonly ITankService _tankService;

    public TanksController(ITankService tankService)
    {
        _tankService = tankService;
    }

    // Extracts the authenticated user's ID from the JWT token
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Lists tanks for the current user with optional type/active filters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TankSummaryDto>>> GetAll(
        [FromQuery] string? tankType = null,
        [FromQuery] bool? isActive = null)
    {
        var tanks = await _tankService.GetAllAsync(GetUserId(), tankType, isActive);
        return Ok(tanks);
    }

    // Returns full tank detail with latest-activity indicators
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TankDto>> GetById(int id)
    {
        var tank = await _tankService.GetByIdAsync(id);
        if (tank is null) return NotFound();
        return Ok(tank);
    }

    [HttpPost]
    public async Task<ActionResult<TankDto>> Create(CreateTankDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Tank name is required.");
        if (dto.SizeLiters <= 0)
            return BadRequest("Size must be greater than zero.");

        var created = await _tankService.CreateAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTankDto dto)
    {
        try
        {
            await _tankService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound("Tank not found.");
        }
    }

    // Soft-deletes (sets IsActive=false)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tankService.DeleteAsync(id);
        return NoContent();
    }

    // Returns water test results in reverse chronological order (newest first)
    [HttpGet("{id:int}/water-parameters")]
    public async Task<ActionResult<IEnumerable<WaterParameterLogDto>>> GetWaterParameters(
        int id, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _tankService.GetWaterParametersAsync(id, from, to, page, pageSize);
        return Ok(logs);
    }

    // Logs a water test result — all parameter fields are optional
    [HttpPost("{id:int}/water-parameters")]
    public async Task<ActionResult<WaterParameterLogDto>> AddWaterParameter(int id, CreateWaterParameterLogDto dto)
    {
        var log = await _tankService.AddWaterParameterAsync(id, dto);
        return Ok(log);
    }

    [HttpGet("{id:int}/maintenance")]
    public async Task<ActionResult<IEnumerable<MaintenanceLogDto>>> GetMaintenanceLogs(
        int id, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] string? type = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _tankService.GetMaintenanceLogsAsync(id, from, to, type, page, pageSize);
        return Ok(logs);
    }

    // Logs a maintenance event (water change, trimming, filter cleaning, etc.)
    [HttpPost("{id:int}/maintenance")]
    public async Task<ActionResult<MaintenanceLogDto>> AddMaintenance(int id, CreateMaintenanceLogDto dto)
    {
        var log = await _tankService.AddMaintenanceAsync(id, dto);
        return Ok(log);
    }

    [HttpGet("{id:int}/fertilization")]
    public async Task<ActionResult<IEnumerable<FertilizationLogDto>>> GetFertilizationLogs(
        int id, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] string? type = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _tankService.GetFertilizationLogsAsync(id, from, to, type, page, pageSize);
        return Ok(logs);
    }

    // Logs a fertilizer dose with estimated ppm additions — marks scheduled vs one-off adjustments
    [HttpPost("{id:int}/fertilization")]
    public async Task<ActionResult<FertilizationLogDto>> AddFertilization(int id, CreateFertilizationLogDto dto)
    {
        var log = await _tankService.AddFertilizationAsync(id, dto);
        return Ok(log);
    }

    // Returns photos in reverse chronological order
    [HttpGet("{id:int}/photos")]
    public async Task<ActionResult<IEnumerable<TankPhotoDto>>> GetPhotos(
        int id, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var photos = await _tankService.GetPhotosAsync(id, from, to, page, pageSize);
        return Ok(photos);
    }

    // Adds a photo with optional link to a specific log entry (water test, maintenance, or dose)
    [HttpPost("{id:int}/photos")]
    public async Task<ActionResult<TankPhotoDto>> AddPhoto(int id, CreateTankPhotoDto dto)
    {
        var photo = await _tankService.AddPhotoAsync(id, dto);
        return Ok(photo);
    }

    [HttpDelete("photos/{photoId:int}")]
    public async Task<IActionResult> DeletePhoto(int photoId)
    {
        await _tankService.DeletePhotoAsync(photoId);
        return NoContent();
    }

    // Returns parameter measurements + dosing events for chart rendering
    [HttpGet("{id:int}/trends")]
    public async Task<ActionResult<TankTrendsDto>> GetTrends(
        int id, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var trends = await _tankService.GetTrendsAsync(id, from, to);
        return Ok(trends);
    }

    // Returns a merged, reverse-chronological feed of all log types + photos
    [HttpGet("{id:int}/timeline")]
    public async Task<ActionResult<IEnumerable<TimelineEntryDto>>> GetTimeline(
        int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var entries = await _tankService.GetTimelineAsync(id, page, pageSize);
        return Ok(entries);
    }

    [HttpGet("{id:int}/target-ranges")]
    public async Task<ActionResult<IEnumerable<TargetParameterRangeDto>>> GetTargetRanges(int id)
    {
        var ranges = await _tankService.GetTargetRangesAsync(id);
        return Ok(ranges);
    }

    // Atomically replaces all target ranges for a tank
    [HttpPut("{id:int}/target-ranges")]
    public async Task<IActionResult> UpsertTargetRanges(int id, UpsertTargetRangesDto dto)
    {
        await _tankService.UpsertTargetRangesAsync(id, dto.Ranges);
        return NoContent();
    }
}
