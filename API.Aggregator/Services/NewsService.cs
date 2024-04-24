using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Linq;

namespace API.Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the News API to retrieve news articles for a given city.
    /// </summary>
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private const string _ApiKey = "API KEY";
        private readonly IMemoryCache _cache;
        private readonly ILogger<NewsService> _logger;

        /// <summary>
        /// Constructor that injects the HttpClient instance and ILogger.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="cache">The IMemoryCache instance for caching data.</param>
        /// <param name="logger">The ILogger instance for logging messages.</param>
        public NewsService(HttpClient httpClient, IMemoryCache cache, ILogger<NewsService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously retrieves a list of news articles for a given city.
        /// Attempts to retrieve data from cache first. If not found, fetches data from the external news API and stores it in the cache (optional).
        /// Logs any errors encountered during the retrieval process.
        /// </summary>
        /// <param name="city">The city name for which to retrieve news articles.</param>
        /// <returns>A task that resolves to a list of NewsArticle objects containing news data for the specified city, or an empty list on error.</returns>
        public async Task<List<NewsArticle>?> GetNewsDataAsync(string city)
        {
            // Check cache first
            // Generate a dynamic cache key based on date and city
            var cacheKey = GetCacheKey(city);
            List<NewsArticle>? cachedNewsArticles = new List<NewsArticle>();
            if (_cache.TryGetValue(cacheKey, out cachedNewsArticles))
            {
                // Check cache expiration (optional)
                if (IsCacheValid(cachedNewsArticles)) // Implement IsCacheValid logic
                {
                    return cachedNewsArticles; // Return cached data if valid
                }
            }

            // Fetch data from external API if not cached or expired
            try
            {
                var newsArticles = await GetNewsAsync(city);
                // Consider filtering or transforming data before caching (optional)
                _cache.Set(cacheKey, newsArticles, GetCacheOptions()); // Set cache with appropriate options
                return newsArticles; // Return fetched data directly
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news data for city '{City}': {Message}", city, ex.Message);
                return new List<NewsArticle>(); // Return empty list on error
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="cachedNewsArticles">The cached news articles.</param>
        /// <returns>True if the cached data is valid, False otherwise.</returns>
        private bool IsCacheValid(List<NewsArticle>? cachedNewsArticles)
        {
            if (cachedNewsArticles == null)
                return false;
            return cachedNewsArticles.All(article => DateTime.UtcNow - article?.LastUpdated < TimeSpan.FromMinutes(15)); // Adjust expiration as needed
        }

        /// <summary>
        /// (Optional) Get cache options for setting the cache entry.
        /// You can configure expiration time, priority, etc. based on your needs.
        /// </summary>
        /// <returns>A MemoryCacheEntryOptions object with desired cache settings.</returns>
        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Adjust expiration as needed
        }

        /// <summary>
        /// Generates a cache key for storing NewsService data.
        /// </summary>
        /// <param name="city">The city for which to generate the cache key.</param>
        /// <returns>A string representing the cache key.</returns>
        private string GetCacheKey(string city)
        {
            // Use a combination of "IpGeolocation" prefix, current date, and city address for uniqueness.
            return $"IpGeolocation_{DateTime.UtcNow.ToString("yyyyMMdd")}_{city}";
        }


        /// <summary>
        /// Asynchronously retrieves a list of news articles related to the given city.
        /// </summary>
        /// <param name="city">The city name for which to retrieve news articles.</param>
        /// <returns>A list of NewsArticle objects containing titles and URLs, or an empty list on error.</returns>
        public async Task<List<NewsArticle>?> GetNewsAsync(string city)
        {

            var parameters = new Dictionary<string, string>()
            {
                ["q"] = city,
                ["from"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ["sortBy"] = "popularity",
                ["pageSize"] = "4",
                ["apiKey"] = _ApiKey
            };

            var url = $"https://newsapi.org/v2/everything?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    NewsArticleResponse? newsResponse;
                    try
                    {
                        newsResponse = JsonConvert.DeserializeObject<NewsArticleResponse>(content);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Error deserializing News API response: {Message}", ex.Message);
                        throw; // Re-throw exception for caller to handle
                    }

                    if (newsResponse == null)
                        return new List<NewsArticle>();

                    var newsArticles = new List<NewsArticle>();
                    newsArticles = newsResponse?.Articles?.Select(article => new NewsArticle
                    {
                        Title = article.Title,
                        Url = article.Url,
                    }).ToList() ?? new List<NewsArticle>();

                    return newsArticles;
                }
                else
                {
                    _logger.LogError("News API request failed with status code: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"News API request failed with status code {response.StatusCode}");
                }

            }
        }
    }
}
