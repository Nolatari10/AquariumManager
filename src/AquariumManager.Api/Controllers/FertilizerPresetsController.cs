using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FertilizerPresetsController : ControllerBase
{
    private readonly IFertilizerPresetService _presetService;

    public FertilizerPresetsController(IFertilizerPresetService presetService)
    {
        _presetService = presetService;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET: api/FertilizerPresets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FertilizerPresetDto>>> GetAll()
    {
        var presets = await _presetService.GetAllAsync(GetUserId());
        return Ok(presets);
    }

    // POST: api/FertilizerPresets
    [HttpPost]
    public async Task<ActionResult<FertilizerPresetDto>> Create(CreateFertilizerPresetDto dto)
    {
        var created = await _presetService.CreateAsync(GetUserId(), dto);
        return Ok(created);
    }

    // PUT: api/FertilizerPresets/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateFertilizerPresetDto dto)
    {
        try
        {
            await _presetService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound("Preset not found.");
        }
    }

    // DELETE: api/FertilizerPresets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _presetService.DeleteAsync(id);
        return NoContent();
    }
}
