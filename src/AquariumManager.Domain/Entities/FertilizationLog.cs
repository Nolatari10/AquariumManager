namespace AquariumManager.Domain.Entities;

public enum FertilizerType
{
    Macro,
    Micro,
    AllInOne,
    Iron,
    Potassium,
    Other
}

public enum DoseUnit
{
    ml,
    grams,
    pumps,
    drops,
    tsp
}

public class FertilizationLog
{
    public int Id { get; private set; }
    public int TankId { get; private set; }
    public Tank Tank { get; private set; } = null!;
    public int? FertilizerPresetId { get; private set; }
    public FertilizerPreset? FertilizerPreset { get; private set; }
    public DateTime DosedAt { get; private set; }
    public decimal DoseAmount { get; private set; }
    public DoseUnit DoseUnit { get; private set; }
    public FertilizerType FertilizerType { get; private set; }
    public decimal? EstimatedNitratePpm { get; private set; }
    public decimal? EstimatedPhosphatePpm { get; private set; }
    public decimal? EstimatedPotassiumPpm { get; private set; }
    public decimal? EstimatedIronPpm { get; private set; }
    public bool IsScheduled { get; private set; } = true;
    public bool IsAdjustment { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private FertilizationLog() { }

    public FertilizationLog(
        int tankId,
        DateTime dosedAt,
        decimal doseAmount,
        DoseUnit doseUnit,
        FertilizerType fertilizerType,
        int? fertilizerPresetId = null,
        decimal? estimatedNitratePpm = null,
        decimal? estimatedPhosphatePpm = null,
        decimal? estimatedPotassiumPpm = null,
        decimal? estimatedIronPpm = null,
        bool isScheduled = true,
        bool isAdjustment = false,
        string? notes = null)
    {
        TankId = tankId;
        DosedAt = dosedAt;
        DoseAmount = doseAmount;
        DoseUnit = doseUnit;
        FertilizerType = fertilizerType;
        FertilizerPresetId = fertilizerPresetId;
        EstimatedNitratePpm = estimatedNitratePpm;
        EstimatedPhosphatePpm = estimatedPhosphatePpm;
        EstimatedPotassiumPpm = estimatedPotassiumPpm;
        EstimatedIronPpm = estimatedIronPpm;
        IsScheduled = isScheduled;
        IsAdjustment = isAdjustment;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }
}
