using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using PriceMonitor.Services;
using PriceMonitor.Settings;

var builder = Host.CreateApplicationBuilder(args);

// Configure options
builder.Services.Configure<PriceMonitorSettings>(
    builder.Configuration.GetSection("PriceMonitorSettings"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ScrapeApiSettings>(
    builder.Configuration.GetSection("ScrapeApiSettings"));
builder.Services.Configure<TelegramSettings>(
    builder.Configuration.GetSection("TelegramSettings"));

// Register HTTP client with enhanced configuration for Amazon
builder.Services.AddHttpClient<IAmazonPriceScraper, AmazonPriceScraper>((sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<ScrapeApiSettings>>().Value;
        // Set timeout
        client.Timeout = TimeSpan.FromSeconds(30);
        client.BaseAddress = new Uri(settings.ApiBaseUrl);
        var authString = $"{settings.Username}:{settings.Password}";
        var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Auth}");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Add HttpClient for Telegram
builder.Services.AddHttpClient<ITelegramService, TelegramService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Register services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ITelegramService, TelegramService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddHostedService<PriceMonitor.PriceMonitor>();

var host = builder.Build();
host.Run();

// Define retry policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // HttpRequestException, 5XX status codes, 408 status code
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // 429 status code
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// Define circuit breaker policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
}