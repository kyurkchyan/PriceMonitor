using Microsoft.Extensions.Options;
using PriceMonitor.Services;
using PriceMonitor.Settings;

namespace PriceMonitor;

public class PriceMonitor(
    ILogger<PriceMonitor> logger,
    IAmazonPriceScraper amazonPriceScraper,
    INotificationService notificationService,
    IOptions<PriceMonitorSettings> settings)
    : BackgroundService
{
    private readonly PriceMonitorSettings _settings = settings.Value;
    private bool _firstRun = true;

    // Track the last price to avoid sending multiple notifications for the same price change
    private decimal _lastNotifiedPrice = decimal.MaxValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Price Monitor service started at: {Time}", DateTimeOffset.Now);
        logger.LogInformation("Monitoring product : {Url}", _settings.Asin);
        logger.LogInformation("Price threshold set to: ${Threshold}", _settings.PriceThreshold);
        logger.LogInformation("Check interval set to: {Interval} minutes", _settings.CheckIntervalMinutes);
        logger.LogInformation("Notification methods: {Methods}", _settings.NotificationMethods);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var hour = DateTime.Now.Hour;
                if (hour is >= 7 and <= 24) await CheckProductPriceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking product price");
            }

            // Wait for the next check interval
            await Task.Delay(TimeSpan.FromMinutes(_settings.CheckIntervalMinutes), stoppingToken);
        }
    }

    private async Task CheckProductPriceAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Checking price at: {Time}", DateTimeOffset.Now);

        try
        {
            var (_, productName, currentPrice, url) = await amazonPriceScraper.ScrapeProductInfoAsync(_settings.Asin);
            logger.LogInformation("Current price for {ProductName}: ${Price}", productName, currentPrice);

            // Check if price is below threshold and we haven't already notified at this price or lower
            if (currentPrice < _settings.PriceThreshold && currentPrice < _lastNotifiedPrice)
            {
                logger.LogInformation("Price drop detected! Current: ${CurrentPrice}, Threshold: ${ThresholdPrice}",
                    currentPrice, _settings.PriceThreshold);

                await notificationService.SendPriceAlertAsync(
                    productName,
                    currentPrice,
                    _settings.PriceThreshold,
                    url);

                // Update the last notified price
                _lastNotifiedPrice = currentPrice;
            }
            else if (currentPrice < _lastNotifiedPrice)
            {
                // Update the last price even if we don't notify, to track price drops
                _lastNotifiedPrice = currentPrice;
                logger.LogInformation(
                    "Price dropped but still above threshold. Current: ${CurrentPrice}, Threshold: ${ThresholdPrice}",
                    currentPrice, _settings.PriceThreshold);
            }
            else if (currentPrice > _lastNotifiedPrice)
            {
                logger.LogInformation("Price increased from ${LastPrice} to ${CurrentPrice}",
                    _lastNotifiedPrice, currentPrice);
                _lastNotifiedPrice = currentPrice;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CheckProductPriceAsync");
        }
    }
}