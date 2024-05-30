using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using API.Aggregator.Models;
using API.Aggregator.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace API_Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the IpGeolocation API to retrieve geolocation information.
    /// </summary>
    public class IpGeolocationService : IIpGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ApiKey;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IpGeolocationService> _logger;

        /// <summary>
        /// Constructor that injects the HttpClient instance and ILogger.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="cache">The IMemoryCache instance for caching data.</param>
        /// <param name="logger">The ILogger instance for logging messages.</param>
        public IpGeolocationService(HttpClient httpClient, IMemoryCache cache, ILogger<IpGeolocationService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        }

        /// <summary>
        /// Retrieves geolocation information for the user's IP address.
        /// Attempts to retrieve data from cache first. If not found, fetches data from the external API.
        /// Logs any errors encountered during the retrieval process.
        /// </summary>
        /// <param name="city">The city name for which to retrieve weather data.</param>
        /// <returns>An IpGeolocationInfo object containing IP address and city (if available) or an empty object on error.</returns>
        public async Task<IAggregatorService> GetIpGeolocationDataAsync(string city)
        {

            if (string.IsNullOrEmpty(_ApiKey))
                return new IpGeolocationInfo() as IAggregatorService;

            // Check cache first
            // Generate a dynamic cache key based on date and city
            var cacheKey = GetCacheKey(city); 
            IpGeolocationInfo ipData;
            if (!_cache.TryGetValue(cacheKey, out ipData))
            {
                // Fetch data from external API if not cached
                try
                {
                    ipData = await GetIpGeolocationAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching IP geolocation data: {Message}", ex.Message);
                    ipData = new IpGeolocationInfo();
                }

                // Set cache entry with expiration (adjust expiration as needed)
                if (ipData != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Cache for 30 minutes

                    _cache.Set(cacheKey, ipData, cacheEntryOptions);
                }
            }

            return ipData as IAggregatorService;
        }

        /// <summary>
        /// Generates a cache key for storing IpGeolocation data.
        /// </summary>
        /// <param name="city">The city for which to generate the cache key.</param>
        /// <returns>A string representing the cache key.</returns>
        private string GetCacheKey(string city)
        {
            // Use a combination of "IpGeolocation" prefix, current date, and city address for uniqueness.
            return $"IpGeolocation_{DateTime.UtcNow.ToString("yyyyMMdd")}_{city}";
        }

        /// <summary>
        /// Asynchronously retrieves geolocation information for the user's IP address from the external API.
        /// Logs any errors encountered during the API call or deserialization process.
        /// Re-throws exceptions for the caller to handle (prevents complete service failure).
        /// </summary>
        /// <returns>An IpGeolocationInfo object containing IP address and city (if available) or an empty object on error.</returns>
        public async Task<IpGeolocationInfo> GetIpGeolocationAsync()
        {
            var parameters = new Dictionary<string, string>()
            {
                ["apiKey"] = _ApiKey
            };

            var url = $"https://api.ipgeolocation.io/ipgeo?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";

            using (var response = await _httpClient.GetAsync(url))
            {

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    IpGeolocationApiResponse ipGeolocationApiResponse;
                    try
                    {
                        ipGeolocationApiResponse = JsonConvert.DeserializeObject<IpGeolocationApiResponse>(content);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing IpGeolocation API response: {Message}", ex.Message);
                        throw; // Re-throw exception for caller to handle
                    }

                    if (ipGeolocationApiResponse == null)
                    {
                        _logger.LogWarning("IpGeolocation API response: no data found");
                        return new IpGeolocationInfo();
                    }

                    var ipInfo = new IpGeolocationInfo
                    {
                        Ip = ipGeolocationApiResponse.Ip,
                        City = ipGeolocationApiResponse.City,
                    };

                    return ipInfo;
                }
                else
                {
                    _logger.LogError("IpGeolocation API request failed with status code: {StatusCode}", response.StatusCode);
                    throw new Exception($"IpGeolocation API request failed with status code: {response.StatusCode}");
                }
            }
        }
    }
}
