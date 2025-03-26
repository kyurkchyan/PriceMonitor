namespace PriceMonitor.Settings;

[Flags]
public enum NotificationMethod
{
    None = 0,
    Email = 1,
    Telegram = 2,
    All = Email | Telegram
}

public class PriceMonitorSettings
{
    public string Asin { get; set; } = string.Empty;
    public decimal PriceThreshold { get; set; }
    public int CheckIntervalMinutes { get; set; } = 5;
    public NotificationMethod NotificationMethods { get; set; } = NotificationMethod.Email;
}