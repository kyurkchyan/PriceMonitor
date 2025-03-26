using System.Net.Http.Json;

namespace PriceMonitor.Services;

public class AmazonPriceScraper(ILogger<AmazonPriceScraper> logger, HttpClient client)
    : IAmazonPriceScraper
{
    public async Task<Product> ScrapeProductInfoAsync(string asin)
    {
        try
        {
            var response = await client.PostAsJsonAsync("/v1/queries", new
            {
                source = "amazon_product",
                query = asin,
                geo_location = "19720",
                parse = true
            });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<ScrapeResponse>();

            if (content?.Results == null || content.Results.Length == 0) throw new Exception("No results found");

            var result = content.Results[0];
            return result.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scraping Amazon product information: {Message}", ex.Message);
            throw;
        }
    }

    private record ScrapeResponse(Result[] Results);

    private record Result(Product Content);
}