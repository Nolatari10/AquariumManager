using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;
using AquariumManager.Domain.Entities;
using AquariumManager.Domain.Interfaces;

namespace AquariumManager.Application.Services;

public class SpeciesService : ISpeciesService
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISpeciesVariantRepository _variantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public SpeciesService(ISpeciesRepository speciesRepository, ISpeciesVariantRepository variantRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _speciesRepository = speciesRepository;
        _variantRepository = variantRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<SpeciesDto> CreateAsync(CreateSpeciesDto dto)
    {
        var species = new Species(
            dto.CommonName,
            dto.ScientificName,
            dto.Type,
            dto.Variety,
            dto.MinPH,
            dto.MaxPH,
            dto.MinTemperature,
            dto.MaxTemperature,
            dto.CompatibilityNotes,
            dto.Category,
            dto.Notes,
            dto.ImageUrl
        );

        species.TenantId = _currentUser.TenantId;

        await _speciesRepository.AddAsync(species);

        var variantName = string.IsNullOrWhiteSpace(dto.Variety) ? "Standard" : dto.Variety.Trim();
        var standardVariant = new SpeciesVariant(species.Id, variantName);
        standardVariant.TenantId = _currentUser.TenantId;
        await _variantRepository.AddAsync(standardVariant);

        return MapToDto(species);
    }

    public async Task<SpeciesDto?> GetByIdAsync(int id)
    {
        var species = await _speciesRepository.GetByIdAsync(_currentUser.TenantId, id);
        return species is null ? null : MapToDto(species);
    }

    public async Task<IReadOnlyList<SpeciesDto>> GetAllAsync()
    {
        var list = await _speciesRepository.GetAllAsync(_currentUser.TenantId);
        return list.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<SpeciesDto>> GetPagedAsync(int page, int pageSize)
    {
        var items = await _speciesRepository.GetPagedAsync(_currentUser.TenantId, page, pageSize);
        var totalCount = await _speciesRepository.GetCountAsync(_currentUser.TenantId);
        return new PagedResult<SpeciesDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async  Task<OperationResult> UpdateAsync(int id, UpdateSpeciesDto dto)
    {
         if (dto.MinPH > dto.MaxPH)
        return OperationResult.Fail("MinPH no puede ser mayor que MaxPH.");

    if (dto.MinTemperature > dto.MaxTemperature)
        return OperationResult.Fail("MinTemperature no puede ser mayor que MaxTemperature.");

        var species = await _speciesRepository.GetByIdAsync(_currentUser.TenantId, id)
                      ?? throw new InvalidOperationException("La especie especificada no existe.");

        species.UpdateInfo(
            dto.CommonName,
            dto.ScientificName,
            dto.Type,
            dto.Variety,
            dto.MinPH,
            dto.MaxPH,
            dto.MinTemperature,
            dto.MaxTemperature,
            dto.CompatibilityNotes,
            dto.Category,
            dto.Notes, 
            dto.ImageUrl
        );

        await _speciesRepository.UpdateAsync(species);
        return OperationResult.Ok();
    }

    public async Task DeleteAsync(int id)
    {
        var hasLots = await _variantRepository.HasInventoryLotsForSpeciesAsync(_currentUser.TenantId, id);
        if (hasLots)
            throw new InvalidOperationException("Cannot delete this species because one or more variants have linked inventory lots.");

        await _speciesRepository.DeleteAsync(id);
    }

    public async Task<BulkDeleteResultDto> BulkDeleteAsync(List<int> ids)
    {
        var result = new BulkDeleteResultDto { Requested = ids.Count };
        var deletableIds = new List<int>();

        foreach (var id in ids)
        {
            var hasLots = await _variantRepository.HasInventoryLotsForSpeciesAsync(_currentUser.TenantId, id);
            if (hasLots)
            {
                result.Skipped++;
                result.Errors.Add($"Species Id={id} cannot be deleted because it has linked inventory lots.");
            }
            else
            {
                deletableIds.Add(id);
            }
        }

        if (deletableIds.Count > 0)
        {
            await _speciesRepository.DeleteRangeAsync(deletableIds);
            result.Deleted = deletableIds.Count;
        }

        return result;
    }

    public async Task<BulkImportResultDto> BulkImportAsync(List<CreateSpeciesDto> dtos)
    {
        var result = new BulkImportResultDto { TotalProcessed = dtos.Count };
        var createdSpecies = new List<(Species species, string variety)>();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.CommonName))
            {
                result.Skipped++;
                result.Errors.Add($"Row skipped: missing CommonName");
                continue;
            }

            try
            {
                var species = new Species(
                    dto.CommonName.Trim(),
                    dto.ScientificName?.Trim() ?? string.Empty,
                    dto.Type?.Trim() ?? string.Empty,
                    dto.Variety?.Trim() ?? string.Empty,
                    dto.MinPH,
                    dto.MaxPH,
                    dto.MinTemperature,
                    dto.MaxTemperature,
                    dto.CompatibilityNotes?.Trim(),
                    string.IsNullOrWhiteSpace(dto.Category) ? "Other" : dto.Category.Trim(),
                    dto.Notes?.Trim(),
                    dto.ImageUrl?.Trim() ?? string.Empty
                );

                species.TenantId = _currentUser.TenantId;

                _speciesRepository.Track(species);
                createdSpecies.Add((species, dto.Variety?.Trim() ?? string.Empty));
                result.Created++;
            }
            catch (Exception ex)
            {
                result.Skipped++;
                result.Errors.Add($"'{dto.CommonName}': {ex.Message}");
            }
        }

        if (result.Created > 0)
        {
            await _unitOfWork.SaveChangesAsync();

            foreach (var (species, variety) in createdSpecies)
            {
                var variantName = string.IsNullOrWhiteSpace(variety) ? "Standard" : variety;
                var variant = new SpeciesVariant(species.Id, variantName);
                variant.TenantId = _currentUser.TenantId;
                await _variantRepository.AddAsync(variant);
            }
        }

        return result;
    }

    private static SpeciesDto MapToDto(Species s) => new()
    {
        Id = s.Id,
        CommonName = s.CommonName,
        ScientificName = s.ScientificName,
        Type = s.Type,
        Variety = s.Variety,
        MinPH = s.MinPH,
        MaxPH = s.MaxPH,
        MinTemperature = s.MinTemperature,
        MaxTemperature = s.MaxTemperature,
        CompatibilityNotes = s.CompatibilityNotes,
        Category = s.Category,
        Notes = s.Notes,
        ImageUrl = s.ImageUrl
    };
}
