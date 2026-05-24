namespace AquariumManager.Domain.Entities;

public enum ParameterName
{
    pH,
    Nitrate,
    Phosphate,
    Iron,
    GH,
    KH,
    TDS,
    CO2,
    Temperature
}

public class TargetParameterRange
{
    public int Id { get; private set; }
    public int TenantId { get; set; }
    public int TankId { get; private set; }
    public Tank Tank { get; private set; } = null!;
    public ParameterName ParameterName { get; private set; }
    public decimal MinValue { get; private set; }
    public decimal MaxValue { get; private set; }
    public string Unit { get; private set; } = string.Empty;

    private TargetParameterRange() { }

    public TargetParameterRange(
        int tankId,
        ParameterName parameterName,
        decimal minValue,
        decimal maxValue,
        string unit)
    {
        TankId = tankId;
        ParameterName = parameterName;
        MinValue = minValue;
        MaxValue = maxValue;
        Unit = unit;
    }

    public void Update(decimal minValue, decimal maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}
