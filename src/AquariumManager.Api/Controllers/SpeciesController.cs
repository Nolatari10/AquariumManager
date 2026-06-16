using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpeciesController : ControllerBase
{
    private readonly ISpeciesService _speciesService;

    public SpeciesController(ISpeciesService speciesService)
    {
        _speciesService = speciesService;
    }

    // GET: api/Species?page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _speciesService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/Species/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeciesDto>> GetById(int id)
    {
        var species = await _speciesService.GetByIdAsync(id);
        if (species is null)
            return NotFound();

        return Ok(species);
    }

    // POST: api/Species
    [HttpPost]
    public async Task<ActionResult<SpeciesDto>> Create(CreateSpeciesDto dto)
    {
        if(dto.MinPH > dto.MaxPH)
            return BadRequest("MinPH must not be greater than MaxPH.");
        if(dto.MinTemperature > dto.MaxTemperature)
            return BadRequest("MinTemperature must not be greater than MaxTemperature.");

        var created = await _speciesService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/Species/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSpeciesDto dto)
    {
         var result = await _speciesService.UpdateAsync(id, dto);

    if (!result.Success)
    {
        if (result.ErrorMessage == "Species not found.")
            return NotFound(result.ErrorMessage);

        return BadRequest(result.ErrorMessage);
    }

    return NoContent();
    }

    // POST: api/Species/bulk-import
    [HttpPost("bulk-import")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> BulkImport(BulkImportSpeciesDto dto)
    {
        if (dto.Species is null || dto.Species.Count == 0)
            return BadRequest("No species provided.");

        var result = await _speciesService.BulkImportAsync(dto.Species);
        return Ok(result);
    }

    // DELETE: api/Species/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _speciesService.DeleteAsync(id);
        return NoContent();
    }

    // POST: api/Species/batch-delete
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BulkDelete(BulkDeleteSpeciesDto dto)
    {
        if (dto.Ids is null || dto.Ids.Count == 0)
            return BadRequest("No ids provided.");

        var result = await _speciesService.BulkDeleteAsync(dto.Ids);
        return Ok(result);
    }
}
