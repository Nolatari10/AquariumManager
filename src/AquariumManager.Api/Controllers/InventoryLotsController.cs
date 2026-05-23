using AquariumManager.Application.DTOs;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryLotsController : ControllerBase
{
    private readonly IInventoryLotService _inventoryLotService;

    public InventoryLotsController(IInventoryLotService inventoryLotService)
    {
        _inventoryLotService = inventoryLotService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryLotDto>> GetById(int id)
    {
        var lot = await _inventoryLotService.GetByIdAsync(id);
        if (lot is null)
            return NotFound();

        return Ok(lot);
    }

    [HttpGet("by-species/{speciesId:int}")]
    public async Task<ActionResult<IEnumerable<InventoryLotDto>>> GetBySpecies(int speciesId)
    {
        var lots = await _inventoryLotService.GetBySpeciesIdAsync(speciesId);
        return Ok(lots);
    }

    [HttpGet("by-variant/{speciesVariantId:int}")]
    public async Task<ActionResult<IEnumerable<InventoryLotDto>>> GetBySpeciesVariant(int speciesVariantId)
    {
        var lots = await _inventoryLotService.GetBySpeciesVariantIdAsync(speciesVariantId);
        return Ok(lots);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryLotDto>> Create(CreateInventoryLotDto dto)
    {
        var created = await _inventoryLotService.CreateLotAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("register-mortality")]
    public async Task<IActionResult> RegisterMortality(RegisterMortalityDto dto)
    {
        await _inventoryLotService.RegisterMortalityAsync(dto);
        return NoContent();
    }

    [HttpGet("biological-stock/{speciesId:int}")]
    public async Task<ActionResult<BiologicalStockDto>> GetBiologicalStock(int speciesId)
    {
        var stock = await _inventoryLotService.GetBiologicalStockDtoBySpeciesAsync(speciesId);
        if (stock is null)
            return NotFound();

        return Ok(stock);
    }

    [HttpGet("biological-stock-variant/{speciesVariantId:int}")]
    public async Task<ActionResult<BiologicalStockDto>> GetBiologicalStockByVariant(int speciesVariantId)
    {
        var stock = await _inventoryLotService.GetBiologicalStockDtoBySpeciesVariantIdAsync(speciesVariantId);
        if (stock is null)
            return NotFound();

        return Ok(stock);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryLotDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _inventoryLotService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<LotHistoryDto>> GetLotHistory(int id)
    {
        var history = await _inventoryLotService.GetLotHistoryAsync(id);
        if (history is null)
            return NotFound();

        return Ok(history);
    }
}
