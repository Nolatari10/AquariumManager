using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/species/{speciesId:int}/[controller]")]
[Authorize]
public class VariantsController : ControllerBase
{
    private readonly ISpeciesVariantService _variantService;

    public VariantsController(ISpeciesVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBySpecies(int speciesId)
    {
        var variants = await _variantService.GetBySpeciesIdAsync(speciesId);
        return Ok(variants);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpeciesVariantDto>> GetById(int speciesId, int id)
    {
        var variant = await _variantService.GetByIdAsync(id);
        if (variant is null || variant.SpeciesId != speciesId)
            return NotFound();
        return Ok(variant);
    }

    [HttpPost]
    public async Task<ActionResult<SpeciesVariantDto>> Create(int speciesId, CreateSpeciesVariantDto dto)
    {
        var result = await _variantService.CreateAsync(speciesId, dto);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetById), new { speciesId, id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int speciesId, int id, UpdateSpeciesVariantDto dto)
    {
        var result = await _variantService.UpdateAsync(speciesId, id, dto);
        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("not found"))
                return NotFound(result.ErrorMessage);
            return BadRequest(result.ErrorMessage);
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int speciesId, int id)
    {
        var result = await _variantService.DeleteAsync(speciesId, id);
        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("not found"))
                return NotFound(result.ErrorMessage);
            return BadRequest(result.ErrorMessage);
        }
        return NoContent();
    }
}
