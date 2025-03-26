using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PriceMonitor.Settings;

namespace PriceMonitor.Services;

public class EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
    : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendPriceAlertAsync(string productName, decimal currentPrice, decimal thresholdPrice,
        string productUrl)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Price Monitor", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", _emailSettings.ToEmail));
            message.Subject = $"Price Alert: {productName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h1>Price Alert</h1>
                    <p>The price of <strong>{productName}</strong> has dropped below your threshold!</p>
                    <p>Current price: ${currentPrice:F2}</p>
                    <p>Your threshold: ${thresholdPrice:F2}</p>
                    <p>View the product: <a href=""{productUrl}"">Amazon Link</a></p>
                ",
                TextBody = $@"
                    Price Alert
                    
                    The price of {productName} has dropped below your threshold!
                    Current price: ${currentPrice:F2}
                    Your threshold: ${thresholdPrice:F2}
                    
                    View the product: {productUrl}
                "
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl);
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Price alert email sent successfully for {ProductName}", productName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send price alert email for {ProductName}", productName);
        }
    }
}