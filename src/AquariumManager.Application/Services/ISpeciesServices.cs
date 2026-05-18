using AquariumManager.Application.Common;
using AquariumManager.Application.DTOs;

namespace AquariumManager.Application.Services;

public interface ISpeciesService
{
    Task<SpeciesDto> CreateAsync(CreateSpeciesDto dto);
    Task<SpeciesDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<SpeciesDto>> GetAllAsync();
    Task<PagedResult<SpeciesDto>> GetPagedAsync(int page, int pageSize);
    Task<OperationResult> UpdateAsync(int id, UpdateSpeciesDto dto);
    Task DeleteAsync(int id);
    Task<BulkImportResultDto> BulkImportAsync(List<CreateSpeciesDto> dtos);
    Task<BulkDeleteResultDto> BulkDeleteAsync(List<int> ids);
}
