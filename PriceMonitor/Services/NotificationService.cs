using Microsoft.Extensions.Options;
using PriceMonitor.Settings;

namespace PriceMonitor.Services;

public class NotificationService(
    ILogger<NotificationService> logger,
    IEmailService emailService,
    ITelegramService telegramService,
    IOptions<PriceMonitorSettings> settings)
    : INotificationService
{
    private readonly PriceMonitorSettings _settings = settings.Value;

    public async Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice,
        string productUrl)
    {
        logger.LogInformation("Sending price alert notifications for {ProductName}", productName);

        var tasks = new List<Task>();

        // Send email notification if enabled
        if (_settings.NotificationMethods.HasFlag(NotificationMethod.Email))
            tasks.Add(emailService.SendPriceAlertAsync(productName, currentPrice, thresholdPrice, productUrl));

        // Send Telegram notification if enabled
        if (_settings.NotificationMethods.HasFlag(NotificationMethod.Telegram))
            tasks.Add(telegramService.SendPriceAlertAsync(productName, currentPrice, thresholdPrice, productUrl));

        // Wait for all notifications to complete
        if (tasks.Count > 0)
            await Task.WhenAll(tasks);
        else
            logger.LogWarning("No notification methods enabled for {ProductName}", productName);
    }
}