public class AlertConfig
{
    public int Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public decimal ThresholdValue { get; set; }
    public bool IsEnabled { get; set; } = true;
}
