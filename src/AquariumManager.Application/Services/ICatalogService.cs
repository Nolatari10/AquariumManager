using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ICatalogService
{
    Task<IReadOnlyList<CatalogItemDto>> GetCatalogAsync();
    Task<PagedResult<CatalogItemDto>> GetPagedAsync(int page, int pageSize);
}
