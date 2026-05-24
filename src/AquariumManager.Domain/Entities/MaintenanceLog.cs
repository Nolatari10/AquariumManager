namespace AquariumManager.Domain.Entities;

public enum MaintenanceType
{
    WaterChange,
    FilterCleaning,
    PlantTrimming,
    SubstrateVacuuming,
    GlassCleaning,
    EquipmentCheck,
    Other
}

public class MaintenanceLog
{
    public int Id { get; private set; }
    public int TenantId { get; set; }
    public int TankId { get; private set; }
    public Tank Tank { get; private set; } = null!;
    public MaintenanceType MaintenanceType { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public int? WaterChangePercent { get; private set; }
    public decimal? WaterChangeLiters { get; private set; }
    public int? DurationMinutes { get; private set; }
    public string? Notes { get; private set; }
    public int? ReminderFrequencyDays { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MaintenanceLog() { }

    public MaintenanceLog(
        int tankId,
        MaintenanceType maintenanceType,
        DateTime performedAt,
        int? waterChangePercent = null,
        decimal? waterChangeLiters = null,
        int? durationMinutes = null,
        string? notes = null,
        int? reminderFrequencyDays = null)
    {
        TankId = tankId;
        MaintenanceType = maintenanceType;
        PerformedAt = performedAt;
        WaterChangePercent = waterChangePercent;
        WaterChangeLiters = waterChangeLiters;
        DurationMinutes = durationMinutes;
        Notes = notes;
        ReminderFrequencyDays = reminderFrequencyDays;
        CreatedAt = DateTime.UtcNow;
    }
}
