
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
                var newsTask = _newsService.GetNewsDataAsync(city).ConfigureAwait(false);// Avoid potential deadlocks in UI applications.
                                                                                     // This ensures the continuations after the await calls do not capture the synchronization context
                                                                                     // of the current thread, allowing tasks to complete on any available thread.
                
                var weatherTask = _openWeatherMapService.GetWeatherDataAsync(city).ConfigureAwait(false);
                var ipInfoTask = _ipGeolocationService.GetIpGeolocationDataAsync(city).ConfigureAwait(false);

                var news = await newsTask;
                var weatherData = await weatherTask;
                var ipData = await ipInfoTask;

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
