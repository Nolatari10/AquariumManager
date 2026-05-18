namespace AquariumManager.Domain.Entities;

public enum LinkedLogType
{
    WaterParameter,
    Maintenance,
    Fertilization
}

public class TankPhoto
{
    public int Id { get; private set; }
    public int TankId { get; private set; }
    public Tank Tank { get; private set; } = null!;
    public DateTime TakenAt { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string? Caption { get; private set; }
    public LinkedLogType? LinkedLogType { get; private set; }
    public int? LinkedLogId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TankPhoto() { }

    public TankPhoto(
        int tankId,
        DateTime takenAt,
        string imageUrl,
        string? caption = null,
        LinkedLogType? linkedLogType = null,
        int? linkedLogId = null)
    {
        TankId = tankId;
        TakenAt = takenAt;
        ImageUrl = imageUrl;
        Caption = caption;
        LinkedLogType = linkedLogType;
        LinkedLogId = linkedLogId;
        CreatedAt = DateTime.UtcNow;
    }
}
