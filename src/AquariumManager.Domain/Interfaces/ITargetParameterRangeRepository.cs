using AquariumManager.Domain.Entities;

namespace AquariumManager.Domain.Interfaces;

public interface ITargetParameterRangeRepository
{
    Task<IReadOnlyList<TargetParameterRange>> GetByTankAsync(int tankId);
    Task UpsertAsync(int tankId, List<(ParameterName Name, decimal MinValue, decimal MaxValue, string Unit)> ranges);
}
