using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class InventoryLotService : IInventoryLotService
{
    private readonly IInventoryLotRepository _lotRepository;
    private readonly ISpeciesVariantRepository _variantRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryLotService(
        IInventoryLotRepository lotRepository,
        ISpeciesVariantRepository variantRepository,
        ISupplierRepository supplierRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _lotRepository = lotRepository;
        _variantRepository = variantRepository;
        _supplierRepository = supplierRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryLotDto> CreateLotAsync(CreateInventoryLotDto dto)
    {
        if (dto.InitialQuantity <= 0)
            throw new InvalidOperationException("Cantidad inicial debe ser mayor a cero.");
        if (dto.DeadOnArrival < 0)
            throw new InvalidOperationException("Decesos al llegar no debe ser negativa.");
        if (dto.DeadOnArrival > dto.InitialQuantity)
            throw new InvalidOperationException("Decesos al llegar no puede exceder la cantidad inicial.");
        if (dto.UnitCost <= 0)
            throw new InvalidOperationException("Costo unitario debe ser mayor a cero.");

        var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, dto.SpeciesVariantId)
                      ?? throw new InvalidOperationException($"La variante {dto.SpeciesVariantId} no fue encontrada.");

        Supplier? supplier = null;
        if (dto.SupplierId.HasValue)
        {
            supplier = await _supplierRepository.GetByIdAsync(_currentUser.TenantId, dto.SupplierId.Value)
                       ?? throw new InvalidOperationException($"Proveedor {dto.SupplierId.Value} no fue encontrado.");
        }

        var lot = new InventoryLot(
            speciesVariantId: dto.SpeciesVariantId,
            arrivalDate: dto.ArrivalDate,
            initialQuantity: dto.InitialQuantity,
            deadOnArrival: dto.DeadOnArrival,
            unitCost: dto.UnitCost,
            supplierId: dto.SupplierId,
            batchNumber: dto.BatchNumber,
            notes: dto.Notes
        );

        lot.TenantId = _currentUser.TenantId;

        await _lotRepository.AddAsync(lot);

        return MapToDto(lot, variant, supplier);
    }

    public async Task<OperationResult<BulkInventoryLotCreateResponseDto>> CreateLotsBulkAsync(BulkInventoryLotCreateRequestDto request)
    {
        if (request.Items is null || request.Items.Count == 0)
            return OperationResult<BulkInventoryLotCreateResponseDto>.Fail("Debe incluir al menos un lote.");

        var errors = new List<string>();
        var validItems = new List<(BulkInventoryLotCreateItemDto item, SpeciesVariant variant, Supplier? supplier)>();

        for (var i = 0; i < request.Items.Count; i++)
        {
            var item = request.Items[i];
            var rowLabel = $"Fila {i + 1}";

            if (item.InitialQuantity <= 0)
                errors.Add($"{rowLabel}: Cantidad inicial debe ser mayor a cero.");
            if (item.DeadOnArrival < 0)
                errors.Add($"{rowLabel}: Decesos al llegar no debe ser negativa.");
            if (item.DeadOnArrival > item.InitialQuantity)
                errors.Add($"{rowLabel}: Decesos al llegar no puede exceder la cantidad inicial.");
            if (item.UnitCost <= 0)
                errors.Add($"{rowLabel}: Costo unitario debe ser mayor a cero.");

            var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, item.SpeciesVariantId);
            if (variant is null)
                errors.Add($"{rowLabel}: La variante {item.SpeciesVariantId} no fue encontrada.");

            Supplier? supplier = null;
            if (item.SupplierId.HasValue)
            {
                supplier = await _supplierRepository.GetByIdAsync(_currentUser.TenantId, item.SupplierId.Value);
                if (supplier is null)
                    errors.Add($"{rowLabel}: Proveedor {item.SupplierId.Value} no fue encontrado.");
            }

            if (variant is not null)
                validItems.Add((item, variant, supplier));
        }

        if (errors.Count > 0)
            return OperationResult<BulkInventoryLotCreateResponseDto>.Fail(string.Join("; ", errors));

        try
        {
            var lots = new List<InventoryLot>();
            var totalQuantity = 0;

            foreach (var (item, variant, supplier) in validItems)
            {
                var lot = new InventoryLot(
                    speciesVariantId: item.SpeciesVariantId,
                    arrivalDate: item.ArrivalDate,
                    initialQuantity: item.InitialQuantity,
                    deadOnArrival: item.DeadOnArrival,
                    unitCost: item.UnitCost,
                    supplierId: item.SupplierId,
                    batchNumber: item.BatchNumber,
                    notes: item.Notes
                );
                lot.TenantId = _currentUser.TenantId;
                lots.Add(lot);
                totalQuantity += item.InitialQuantity;
            }

            await _unitOfWork.BeginTransactionAsync();
            _lotRepository.AddRange(lots);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var createdIds = lots.Select(l => l.Id).ToList();

            return OperationResult<BulkInventoryLotCreateResponseDto>.Ok(new BulkInventoryLotCreateResponseDto
            {
                CreatedLotIds = createdIds,
                TotalCreated = createdIds.Count,
                TotalQuantity = totalQuantity
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return OperationResult<BulkInventoryLotCreateResponseDto>.Fail(ex.Message);
        }
    }

    public async Task<InventoryLotDto?> GetByIdAsync(int id)
    {
        var lot = await _lotRepository.GetByIdAsync(_currentUser.TenantId, id);
        if (lot is null) return null;
        return MapToDto(lot, lot.SpeciesVariant, lot.Supplier);
    }

    public async Task<InventoryLot?> GetLotEntityByIdAsync(int id)
    {
        return await _lotRepository.GetByIdAsync(_currentUser.TenantId, id);
    }

    public async Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesIdAsync(int speciesId)
    {
        var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, speciesId);
        return lots.Select(lot => MapToDto(lot, lot.SpeciesVariant, lot.Supplier)).ToList();
    }

    public async Task<IReadOnlyList<InventoryLotDto>> GetBySpeciesVariantIdAsync(int speciesVariantId)
    {
        var lots = await _lotRepository.GetBySpeciesVariantIdAsync(_currentUser.TenantId, speciesVariantId);
        return lots.Select(lot => MapToDto(lot, lot.SpeciesVariant, lot.Supplier)).ToList();
    }

    public async Task RegisterMortalityAsync(RegisterMortalityDto dto)
    {
        var lot = await _lotRepository.GetByIdAsync(_currentUser.TenantId, dto.InventoryLotId)
                  ?? throw new InvalidOperationException("Inventory lot not found.");

        lot.RegisterMortality(dto.Date, dto.Quantity, dto.Cause, dto.Notes);
        await _lotRepository.UpdateAsync(lot);
    }

    public async Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesAsync(int speciesId)
    {
        var lots = await _lotRepository.GetBySpeciesIdAsync(_currentUser.TenantId, speciesId);
        return BuildBiologicalStockDto(speciesId, lots);
    }

    public async Task<BiologicalStockDto?> GetBiologicalStockDtoBySpeciesVariantIdAsync(int speciesVariantId)
    {
        var lots = await _lotRepository.GetBySpeciesVariantIdAsync(_currentUser.TenantId, speciesVariantId);
        if (lots.Count == 0) return null;

        var variant = await _variantRepository.GetByIdAsync(_currentUser.TenantId, speciesVariantId);
        var speciesId = variant?.SpeciesId ?? 0;

        return BuildBiologicalStockDto(speciesId, lots);
    }

    private static BiologicalStockDto? BuildBiologicalStockDto(int speciesId, IReadOnlyList<InventoryLot> lots)
    {
        if (lots.Count == 0) return null;

        var currentStock = lots.Sum(l =>
            l.InitialQuantity - l.DeadOnArrival - (l.MortalityRecords?.Sum(m => m.Quantity) ?? 0));

        return new BiologicalStockDto
        {
            SpeciesId = speciesId,
            CommonName = lots.FirstOrDefault()?.SpeciesVariant?.Species?.CommonName ?? string.Empty,
            CurrentBiologicalStock = currentStock
        };
    }

    public async Task<IReadOnlyList<InventoryLotDto>> GetAllAsync()
    {
        var lots = await _lotRepository.GetAllAsync(_currentUser.TenantId);
        return lots.Select(lot => MapToDto(lot, lot.SpeciesVariant, lot.Supplier)).ToList();
    }

    public async Task<PagedResult<InventoryLotDto>> GetPagedAsync(int page, int pageSize)
    {
        var lots = await _lotRepository.GetPagedAsync(_currentUser.TenantId, page, pageSize);
        var totalCount = await _lotRepository.GetCountAsync(_currentUser.TenantId);
        return new PagedResult<InventoryLotDto>
        {
            Items = lots.Select(lot => MapToDto(lot, lot.SpeciesVariant, lot.Supplier)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<LotHistoryDto?> GetLotHistoryAsync(int lotId)
    {
        var lot = await _lotRepository.GetByIdAsync(_currentUser.TenantId, lotId);
        if (lot is null) return null;

        var speciesName = lot.SpeciesVariant?.Species?.CommonName
                       ?? lot.SpeciesVariant?.VariantName
                       ?? string.Empty;

        var events = new List<LotHistoryEventDto>();

        events.Add(new LotHistoryEventDto
        {
            Date = lot.ArrivalDate,
            EventType = "Arrival",
            Description = $"Lot created — {lot.InitialQuantity} units arrived (DOA: {lot.DeadOnArrival})",
            Quantity = lot.InitialQuantity,
            Notes = lot.Notes
        });

        foreach (var record in lot.MortalityRecords.OrderBy(r => r.Date))
        {
            var isSold = string.Equals(record.Cause, "Sold", StringComparison.OrdinalIgnoreCase);
            events.Add(new LotHistoryEventDto
            {
                Date = record.Date,
                EventType = isSold ? "Sold" : "Mortality",
                Description = isSold
                    ? $"{record.Quantity} unit(s) sold"
                    : $"{record.Quantity} unit(s) lost — {record.Cause ?? "Unknown cause"}",
                Quantity = record.Quantity,
                Cause = record.Cause,
                Notes = record.Notes,
                RelatedRecordId = record.Id
            });
        }

        return new LotHistoryDto
        {
            LotId = lot.Id,
            SpeciesName = speciesName,
            VariantName = lot.SpeciesVariant?.VariantName,
            SupplierName = lot.Supplier?.Name,
            ArrivalDate = lot.ArrivalDate,
            InitialQuantity = lot.InitialQuantity,
            DeadOnArrival = lot.DeadOnArrival,
            CurrentStock = lot.GetCurrentStock(),
            UnitCost = lot.UnitCost,
            BatchNumber = lot.BatchNumber,
            Events = events
        };
    }

    private static InventoryLotDto MapToDto(InventoryLot lot, SpeciesVariant? variant, Supplier? supplier)
    {
        return new InventoryLotDto
        {
            Id = lot.Id,
            SpeciesVariantId = lot.SpeciesVariantId,
            SpeciesName = variant?.Species?.CommonName ?? variant?.VariantName ?? string.Empty,
            VariantName = variant?.VariantName ?? string.Empty,
            SpeciesCommonName = variant?.Species?.CommonName ?? string.Empty,
            ArrivalDate = lot.ArrivalDate,
            InitialQuantity = lot.InitialQuantity,
            DeadOnArrival = lot.DeadOnArrival,
            TotalMortality = lot.GetTotalDeaths(),
            CurrentStock = lot.GetCurrentStock(),
            UnitCost = lot.UnitCost,
            SupplierId = lot.SupplierId,
            SupplierName = supplier?.Name,
            BatchNumber = lot.BatchNumber,
            Notes = lot.Notes
        };
    }
}
