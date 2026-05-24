using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface IInventoryLotRepository
{
    Task<InventoryLot?> GetByIdAsync(int tenantId, int id);
    Task<IReadOnlyList<InventoryLot>> GetBySpeciesIdAsync(int tenantId, int speciesId);
    Task<IReadOnlyList<InventoryLot>> GetBySpeciesVariantIdAsync(int tenantId, int speciesVariantId);
    Task<IReadOnlyList<InventoryLot>> GetOpenLotsBySpeciesVariantIdAsync(int tenantId, int speciesVariantId);
    Task AddAsync(InventoryLot lot);
    Task UpdateAsync(InventoryLot lot);
    Task<IReadOnlyList<InventoryLot>> GetAllAsync(int tenantId);
    Task<IReadOnlyList<InventoryLot>> GetPagedAsync(int tenantId, int page, int pageSize);
    Task<int> GetCountAsync(int tenantId);
}
