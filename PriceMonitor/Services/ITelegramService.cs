namespace PriceMonitor.Services;

public interface ITelegramService
{
    Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice, string productUrl);
}