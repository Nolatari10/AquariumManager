
using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISpeciesVariantRepository _variantRepository;
    private readonly IInventoryLotService _inventoryLotService;
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(
        ISaleRepository saleRepository,
        ISpeciesRepository speciesRepository,
        ISpeciesVariantRepository variantRepository,
        IInventoryLotService inventoryLotService,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _speciesRepository = speciesRepository;
        _variantRepository = variantRepository;
        _inventoryLotService = inventoryLotService;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<SaleDto>> CreateSaleAsync(CreateSaleDto saleDto)
    {
        if (saleDto.Items == null || saleDto.Items.Count == 0)
            return OperationResult<SaleDto>.Fail("La venta debe tener al menos un item.");

        foreach (var item in saleDto.Items)
        {
            if (item.SpeciesId <= 0)
                return OperationResult<SaleDto>.Fail("Cada item debe tener un SpeciesId valido.");

            if (item.Quantity <= 0)
                return OperationResult<SaleDto>.Fail("La cantidad debe ser mayor que 0.");

            if (item.UnitPrice < 0)
                return OperationResult<SaleDto>.Fail("El precio unitario debe ser mayor o igual que 0.");
        }

        foreach (var item in saleDto.Items)
        {
            var species = await _speciesRepository.GetByIdAsync(item.SpeciesId);
            if (species is null)
                return OperationResult<SaleDto>.Fail($"La especie con Id {item.SpeciesId} no existe.");

            if (item.SpeciesVariantId.HasValue)
            {
                var variant = await _variantRepository.GetByIdAsync(item.SpeciesVariantId.Value);
                if (variant is null || variant.SpeciesId != item.SpeciesId)
                    return OperationResult<SaleDto>.Fail($"La variante con Id {item.SpeciesVariantId} no existe para esta especie.");
            }
        }

        foreach (var item in saleDto.Items)
        {
            BiologicalStockDto? stockDto;
            if (item.SpeciesVariantId.HasValue)
                stockDto = await _inventoryLotService.GetBiologicalStockDtoBySpeciesVariantIdAsync(item.SpeciesVariantId.Value);
            else
                stockDto = await _inventoryLotService.GetBiologicalStockDtoBySpeciesAsync(item.SpeciesId);

            var available = stockDto?.CurrentBiologicalStock ?? 0;

            if (available < item.Quantity)
            {
                return OperationResult<SaleDto>.Fail(
                    $"No hay stock suficiente para la especie {item.SpeciesId}. " +
                    $"Disponible: {available}, solicitado: {item.Quantity}.");
            }
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var sale = new Sale
            {
                Date = saleDto.Date,
                CustomerName = saleDto.CustomerName
            };

            foreach (var itemDto in saleDto.Items)
            {
                var remainingToSell = itemDto.Quantity;

                var saleItem = new SaleItem
                {
                    SpeciesId = itemDto.SpeciesId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    SpeciesVariantId = itemDto.SpeciesVariantId
                };
                sale.Items.Add(saleItem);

                List<InventoryLotDto> openLots;
                if (itemDto.SpeciesVariantId.HasValue)
                {
                    openLots = (await _inventoryLotService.GetBySpeciesVariantIdAsync(itemDto.SpeciesVariantId.Value))
                        .Where(l => l.CurrentStock > 0)
                        .OrderBy(l => l.ArrivalDate)
                        .ToList();
                }
                else
                {
                    openLots = (await _inventoryLotService.GetBySpeciesIdAsync(itemDto.SpeciesId))
                        .Where(l => l.CurrentStock > 0)
                        .OrderBy(l => l.ArrivalDate)
                        .ToList();
                }

                foreach (var lotDto in openLots)
                {
                    if (remainingToSell <= 0) break;

                    var toDeduct = Math.Min(remainingToSell, lotDto.CurrentStock);

                    if (toDeduct > 0)
                    {
                        await _inventoryLotService.RegisterMortalityAsync(
                            new RegisterMortalityDto
                            {
                                InventoryLotId = lotDto.Id,
                                Date = saleDto.Date,
                                Quantity = toDeduct,
                                Cause = "Sold"
                            });
                        remainingToSell -= toDeduct;
                    }
                }
            }

            await _saleRepository.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return OperationResult<SaleDto>.Ok(MapToDto(sale));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return OperationResult<SaleDto>.Fail($"Error al crear la venta: {ex.Message}");
        }
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        return sale is null ? null : MapToDto(sale);
    }

    public async Task<IReadOnlyList<SaleDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
        return sales.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<SaleDto>> GetPagedAsync(int page, int pageSize)
    {
        var sales = await _saleRepository.GetPagedAsync(page, pageSize);
        var totalCount = await _saleRepository.GetCountAsync();
        return new PagedResult<SaleDto>
        {
            Items = sales.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            Date = sale.Date,
            CustomerName = sale.CustomerName,
            TotalAmount = sale.Items.Sum(i => i.Quantity * i.UnitPrice),
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                SpeciesId = i.SpeciesId,
                SpeciesVariantId = i.SpeciesVariantId,
                VariantName = i.SpeciesVariant?.VariantName ?? string.Empty,
                SpeciesCommonName = i.Species?.CommonName ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
