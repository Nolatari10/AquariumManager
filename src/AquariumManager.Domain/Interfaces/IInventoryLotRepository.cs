using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IInventoryLotRepository
{
    Task<InventoryLot?> GetByIdAsync(int id);
    Task<IReadOnlyList<InventoryLot>> GetBySpeciesIdAsync(int speciesId);
    Task<IReadOnlyList<InventoryLot>> GetBySpeciesVariantIdAsync(int speciesVariantId);
    Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesVariantIdAsync(int speciesVariantId);
    Task AddAsync(InventoryLot lot);
    Task UpdateAsync(InventoryLot lot);
    Task<IReadOnlyList<InventoryLot>> GetAllAsync();
    Task<IReadOnlyList<InventoryLot>> GetPagedAsync(int page, int pageSize);
    Task<int> GetCountAsync();
}
