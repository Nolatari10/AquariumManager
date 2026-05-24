namespace AquariumManager.Domain.Entities;

public class WaterParameterLog
{
    public int Id { get; private set; }
    public int TenantId { get; set; }
    public int TankId { get; private set; }
    public Tank Tank { get; private set; } = null!;
    public DateTime MeasuredAt { get; private set; }

    public decimal? pH { get; private set; }
    public decimal? TemperatureCelsius { get; private set; }
    public decimal? AmmoniaPpm { get; private set; }
    public decimal? NitritePpm { get; private set; }
    public decimal? NitratePpm { get; private set; }
    public decimal? PhosphatePpm { get; private set; }
    public decimal? PotassiumPpm { get; private set; }
    public decimal? IronPpm { get; private set; }
    public decimal? GeneralHardness { get; private set; }
    public decimal? CarbonateHardness { get; private set; }
    public int? TdsPpm { get; private set; }
    public decimal? Co2Ppm { get; private set; }
    public decimal? SalinityPpt { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WaterParameterLog() { }

    public WaterParameterLog(
        int tankId,
        DateTime measuredAt,
        decimal? pH = null,
        decimal? temperatureCelsius = null,
        decimal? ammoniaPpm = null,
        decimal? nitritePpm = null,
        decimal? nitratePpm = null,
        decimal? phosphatePpm = null,
        decimal? potassiumPpm = null,
        decimal? ironPpm = null,
        decimal? generalHardness = null,
        decimal? carbonateHardness = null,
        int? tdsPpm = null,
        decimal? co2Ppm = null,
        decimal? salinityPpt = null,
        string? notes = null)
    {
        TankId = tankId;
        MeasuredAt = measuredAt;
        this.pH = pH;
        TemperatureCelsius = temperatureCelsius;
        AmmoniaPpm = ammoniaPpm;
        NitritePpm = nitritePpm;
        NitratePpm = nitratePpm;
        PhosphatePpm = phosphatePpm;
        PotassiumPpm = potassiumPpm;
        IronPpm = ironPpm;
        GeneralHardness = generalHardness;
        CarbonateHardness = carbonateHardness;
        TdsPpm = tdsPpm;
        Co2Ppm = co2Ppm;
        SalinityPpt = salinityPpt;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }
}
