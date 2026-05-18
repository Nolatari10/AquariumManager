namespace AquariumManager.Domain.Entities;

public enum TankType
{
    PlantedHighTech,
    PlantedLowTech,
    Aquascape,
    Biotope,
    Shrimp,
    Breeding,
    Quarantine,
    Other
}

public class Tank
{
    public int Id { get; private set; }
    public int OwnerUserId { get; private set; }
    public User OwnerUser { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public decimal SizeLiters { get; private set; }
    public TankType TankType { get; private set; }
    public string? Substrate { get; private set; }
    public bool Co2Injection { get; private set; }
    public string? LightDescription { get; private set; }
    public string? FilterDescription { get; private set; }
    public decimal? HeaterSetpointCelsius { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<WaterParameterLog> WaterParameterLogs { get; private set; } = new List<WaterParameterLog>();
    public ICollection<MaintenanceLog> MaintenanceLogs { get; private set; } = new List<MaintenanceLog>();
    public ICollection<FertilizationLog> FertilizationLogs { get; private set; } = new List<FertilizationLog>();
    public ICollection<TankPhoto> TankPhotos { get; private set; } = new List<TankPhoto>();
    public ICollection<TargetParameterRange> TargetParameterRanges { get; private set; } = new List<TargetParameterRange>();

    private Tank() { }

    public Tank(
        int ownerUserId,
        string name,
        decimal sizeLiters,
        TankType tankType,
        string? substrate = null,
        bool co2Injection = false,
        string? lightDescription = null,
        string? filterDescription = null,
        decimal? heaterSetpointCelsius = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tank name is required.", nameof(name));
        if (sizeLiters <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(sizeLiters));

        OwnerUserId = ownerUserId;
        Name = name;
        SizeLiters = sizeLiters;
        TankType = tankType;
        Substrate = substrate;
        Co2Injection = co2Injection;
        LightDescription = lightDescription;
        FilterDescription = filterDescription;
        HeaterSetpointCelsius = heaterSetpointCelsius;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(
        string name,
        decimal sizeLiters,
        TankType tankType,
        string? substrate,
        bool co2Injection,
        string? lightDescription,
        string? filterDescription,
        decimal? heaterSetpointCelsius)
    {
        Name = name;
        SizeLiters = sizeLiters;
        TankType = tankType;
        Substrate = substrate;
        Co2Injection = co2Injection;
        LightDescription = lightDescription;
        FilterDescription = filterDescription;
        HeaterSetpointCelsius = heaterSetpointCelsius;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
