using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISpeciesVariantRepository _variantRepository;
    private readonly IInventoryLotService _inventoryLotService;
    private readonly ICurrentUserService _currentUser;

    public CatalogService(
        ISpeciesRepository speciesRepository,
        ISpeciesVariantRepository variantRepository,
        IInventoryLotService inventoryLotService,
        ICurrentUserService currentUser)
    {
        _speciesRepository = speciesRepository;
        _variantRepository = variantRepository;
        _inventoryLotService = inventoryLotService;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CatalogItemDto>> GetCatalogAsync()
    {
        var speciesList = await _speciesRepository.GetAllAsync(_currentUser.TenantId);
        var result = new List<CatalogItemDto>();

        foreach (var species in speciesList)
        {
            var variants = await _variantRepository.GetBySpeciesIdAsync(_currentUser.TenantId, species.Id);

            foreach (var variant in variants)
            {
                var lots = await _inventoryLotService.GetBySpeciesVariantIdAsync(variant.Id);
                var lotsWithStock = lots.Where(l => l.CurrentStock > 0).ToList();

                if (lotsWithStock.Count == 0) continue;

                var totalStock = lotsWithStock.Sum(l => l.CurrentStock);
                var latestLot = lotsWithStock.OrderByDescending(l => l.ArrivalDate).First();
                var imageUrl = !string.IsNullOrWhiteSpace(variant.ImageUrl)
                    ? variant.ImageUrl
                    : !string.IsNullOrWhiteSpace(species.ImageUrl)
                        ? species.ImageUrl
                        : string.Empty;

                result.Add(new CatalogItemDto
                {
                    SpeciesVariantId = variant.Id,
                    VariantName = variant.VariantName,
                    SpeciesId = species.Id,
                    CommonName = species.CommonName,
                    ScientificName = species.ScientificName,
                    Category = species.Category,
                    TotalStock = totalStock,
                    LatestUnitCost = latestLot.UnitCost,
                    ImageUrl = imageUrl,
                    MinPH = species.MinPH,
                    MaxPH = species.MaxPH,
                    MinTemperature = species.MinTemperature,
                    MaxTemperature = species.MaxTemperature
                });
            }
        }

        return result;
    }

    public async Task<PagedResult<CatalogItemDto>> GetPagedAsync(int page, int pageSize)
    {
        var allItems = await GetCatalogAsync();
        var totalCount = allItems.Count;

        var pagedItems = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<CatalogItemDto>
        {
            Items = pagedItems,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
