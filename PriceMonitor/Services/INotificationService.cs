namespace PriceMonitor.Services;

public interface INotificationService
{
    Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice, string productUrl);
}