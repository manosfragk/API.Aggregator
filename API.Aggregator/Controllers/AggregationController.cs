
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace API.Aggregator.Controllers
{
    /// <summary>
    /// Controller class responsible for handling API aggregation requests.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IOpenWeatherMapService _openWeatherMapService;
        private readonly IIpGeolocationService _ipGeolocationService;
        private readonly INewsService _newsService;
        private readonly ILogger<AggregationController> _logger;

        /// <summary>
        /// Constructor for dependency injection of services and logger.
        /// </summary>
        /// <param name="openWeatherMapService">OpenWeatherMap service instance.</param>
        /// <param name="ipGeolocationService">IpGeolocation service instance.</param>
        /// <param name="newsService">News service instance.</param>
        /// <param name="logger">ILogger instance for logging messages.</param>
        public AggregationController(IOpenWeatherMapService openWeatherMapService, IIpGeolocationService ipGeolocationService, INewsService newsService, ILogger<AggregationController> logger)
        {
            _openWeatherMapService = openWeatherMapService;
            _ipGeolocationService = ipGeolocationService;
            _newsService = newsService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves aggregated data (weather, news, geolocation) for a given city.
        /// Attempts to retrieve data concurrently for weather, news, and geolocation.
        /// Logs any errors encountered during the retrieval process.
        /// Returns an ActionResult containing the aggregated data or an error message.
        /// </summary>
        /// <param name="city">The city name for which to retrieve data.</param>
        /// <returns>An ActionResult containing the aggregated data or an error message.</returns>
        [HttpGet("GetAggregatedData")]
        public async Task<ActionResult<AggregatedData>> GetAggregatedData(string city)
        {
            try
            {
                // Create a list of tasks to hold the asynchronous operations

                List<Task<IAggregatorService>> tasks =
                [
                    _newsService.GetNewsDataAsync(city),
                    _openWeatherMapService.GetWeatherDataAsync(city),
                    _ipGeolocationService.GetIpGeolocationDataAsync(city),
                ];

                // Wait for all tasks to complete concurrently
                var results = await Task.WhenAll(tasks);


                var news = results[0] as List<NewsArticle>;
                var weatherData = results[1] as WeatherInfo;
                var ipData = results[2] as IpGeolocationInfo;

                var aggregatedData = new AggregatedData
                {
                    WeatherInfo = weatherData,
                    IpInfo = ipData,
                    NewsArticles = news ?? new List<NewsArticle>()
                };

                return Ok(aggregatedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching aggregated data: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
