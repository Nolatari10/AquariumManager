namespace AquariumManager.Domain.Entities;

public class FertilizerPreset
{
    public int Id { get; private set; }
    public int TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public int? OwnerUserId { get; private set; }
    public User? OwnerUser { get; private set; }
    public FertilizerType FertilizerType { get; private set; }
    public decimal DefaultDoseAmount { get; private set; }
    public DoseUnit DefaultDoseUnit { get; private set; }
    public decimal? NitratePerDose { get; private set; }
    public decimal? PhosphatePerDose { get; private set; }
    public decimal? PotassiumPerDose { get; private set; }
    public decimal? IronPerDose { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    private FertilizerPreset() { }

    public FertilizerPreset(
        string name,
        FertilizerType fertilizerType,
        decimal defaultDoseAmount,
        DoseUnit defaultDoseUnit,
        int? ownerUserId = null,
        decimal? nitratePerDose = null,
        decimal? phosphatePerDose = null,
        decimal? potassiumPerDose = null,
        decimal? ironPerDose = null,
        string? notes = null)
    {
        Name = name;
        FertilizerType = fertilizerType;
        DefaultDoseAmount = defaultDoseAmount;
        DefaultDoseUnit = defaultDoseUnit;
        OwnerUserId = ownerUserId;
        NitratePerDose = nitratePerDose;
        PhosphatePerDose = phosphatePerDose;
        PotassiumPerDose = potassiumPerDose;
        IronPerDose = ironPerDose;
        Notes = notes;
    }
}
