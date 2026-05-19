using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;

namespace AquariumManager.Application.Services;

public interface IInventoryLotService
{
    Task<InventoryLotDto> CreateLotAsync(CreateInventoryLotDto dto);
    Task<InventoryLotDto?> GetByIdAsync(int id);
    Task<InventoryLot?> GetLotEntityByIdAsync(int id);
    Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesIdAsync(int speciesId);
    Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesVariantIdAsync(int speciesVariantId);
    Task RegisterMortalityAsync(RegisterMortalityDto dto);
    Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesAsync(int speciesId);
    Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesVariantIdAsync(int speciesVariantId);
    Task<IReadOnlyList<InventoryLotDto>> GetAllAsync();
    Task<PagedResult<InventoryLotDto>> GetPagedAsync(int page, int pageSize);
}
