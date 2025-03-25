namespace PriceMonitor.Services;

public interface IAmazonPriceScraper
{
    Task<Product> ScrapeProductInfoAsync(string asin);
}