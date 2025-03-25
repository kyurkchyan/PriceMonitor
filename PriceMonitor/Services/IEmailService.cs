namespace PriceMonitor.Services;

public interface IEmailService
{
    Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice, string productUrl);
}