namespace PriceMonitor.Settings;

public class PriceMonitorSettings
{
    public string Asin { get; set; } = string.Empty;
    public decimal PriceThreshold { get; set; }
    public int CheckIntervalMinutes { get; set; } = 5;
}