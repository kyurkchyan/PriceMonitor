using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PriceMonitor.Settings;

namespace PriceMonitor.Services;

public class TelegramService(
    ILogger<TelegramService> logger,
    IOptions<TelegramSettings> telegramSettings,
    HttpClient httpClient)
    : ITelegramService
{
    private readonly TelegramSettings _telegramSettings = telegramSettings.Value;

    public async Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice,
        string productUrl)
    {
        try
        {
            // Check if Telegram configuration is valid
            if (string.IsNullOrEmpty(_telegramSettings.BotToken) || string.IsNullOrEmpty(_telegramSettings.ChatId))
            {
                logger.LogWarning("Telegram notification skipped due to missing configuration");
                return;
            }

            // Create message text
            var messageText = $"*Price Alert*\n\n" +
                              $"The price of *{productName}* has dropped below your threshold!\n\n" +
                              $"Current price: ${currentPrice:F2}\n" +
                              $"Your threshold: ${thresholdPrice:F2}\n\n" +
                              $"[View on Amazon]({productUrl})";

            // Prepare the request to the Telegram API
            var apiUrl = $"https://api.telegram.org/bot{_telegramSettings.BotToken}/sendMessage";

            var payload = new
            {
                chat_id = _telegramSettings.ChatId,
                text = messageText,
                parse_mode = "Markdown"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            // Send the request
            var response = await httpClient.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Price alert Telegram message sent successfully for {ProductName}", productName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send price alert via Telegram for {ProductName}", productName);
        }
    }
}