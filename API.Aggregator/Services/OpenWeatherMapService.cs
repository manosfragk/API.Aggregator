using API.Aggregator.Helpers;
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API_Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the OpenWeatherMap API to retrieve weather information.
    /// </summary>
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ApiKey;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OpenWeatherMapService> _logger;

        /// <summary>
        /// Constructor that injects the HttpClient instance and ILogger.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="cache">The IMemoryCache instance for caching data.</param>
        /// <param name="logger">The ILogger instance for logging messages.</param>
        public OpenWeatherMapService(HttpClient httpClient, IMemoryCache cache, ILogger<OpenWeatherMapService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        }

        /// <summary>
        /// Asynchronously retrieves weather information for a given city.
        /// Attempts to retrieve data from cache first. If not found, fetches data from the external weather API and stores it in the cache.
        /// Logs any errors encountered during the retrieval process.
        /// </summary>
        /// <param name="city">The city name for which to retrieve weather data.</param>
        /// <returns>An WeatherInfo object containing city, weather description, and temperature (in Celsius) or an empty object on error.</returns>
        public async Task<IAggregatorService> GetWeatherDataAsync(string city)
        {
            // Check cache first
            // Generate a dynamic cache key based on date and city
            var cacheKey = GetCacheKey(city);
            WeatherInfo weatherData;
            if (!_cache.TryGetValue(cacheKey, out weatherData))
            {
                // Fetch data from external API if not cached
                try
                {
                    weatherData = await GetWeatherAsync(city);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching OpenWeatherMap data: {Message}", ex.Message);
                    weatherData = null; // Set weatherData to null on error
                }

                // Set cache entry with expiration (adjust expiration as needed)
                if (weatherData != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Cache for 30 minutes

                    _cache.Set(cacheKey, weatherData, cacheEntryOptions);
                }
            }

            return weatherData ?? new WeatherInfo();
        }

        /// <summary>
        /// Generates a cache key for storing OpenWeatherMap data.
        /// </summary>
        /// <param name="city">The city for which to generate the cache key.</param>
        /// <returns>A string representing the cache key.</returns>
        private string GetCacheKey(string city)
        {
            // Use a combination of "OpenWeatherMap" prefix, current date, and city address for uniqueness.
            return $"OpenWeatherMap{DateTime.UtcNow.ToString("yyyyMMdd")}_{city}";
        }

        /// <summary>
        /// Asynchronously retrieves weather information for a given city.
        /// Logs any errors encountered during the retrieval process.
        /// </summary>
        /// <param name="city">The city name for which to retrieve weather data.</param>
        /// <returns>An WeatherInfo object containing city, weather description, and temperature (in Celsius) or an empty object on error.</returns>
        public async Task<WeatherInfo> GetWeatherAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_ApiKey}";

            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    OpenWeatherMapResponse weatherMapResponse;
                    try
                    {
                        weatherMapResponse = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(content);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Error deserializing OpenWeatherMap API response: {Message}", ex.Message);
                        throw; // Re-throw exception for caller to handle
                    }

                    if (weatherMapResponse == null)
                        return new WeatherInfo();

                    var weatherInfo = new WeatherInfo
                    {
                        City = weatherMapResponse.Name,
                        Description = weatherMapResponse.Weather?.FirstOrDefault()?.Description ?? string.Empty,
                        Temperature = Helper.ConvertKelvinToCelsius(weatherMapResponse.Main.Temp),
                    };

                    return weatherInfo;
                }
                else
                {
                    _logger.LogError("OpenWeatherMap API request failed with status code: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"OpenWeatherMap API request failed with status code {response.StatusCode}");
                }
            }
        }
    }
}
