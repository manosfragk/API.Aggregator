
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Aggregator.Controllers
{
    /// <summary>
    /// Controller class for handling API aggregation requests.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IOpenWeatherMapService _openWeatherMapService;
        private readonly IIpGeolocationService _ipGeolocationService;
        private readonly INewsService _newsService;

        /// <summary>
        /// Constructor for dependency injection of services.
        /// </summary>
        /// <param name="openWeatherMapService">OpenWeatherMap service instance.</param>
        /// <param name="ipGeolocationService">IpGeolocation service instance.</param>
        /// <param name="newsService">News service instance.</param>
        public AggregationController(IOpenWeatherMapService openWeatherMapService, IIpGeolocationService ipGeolocationService, INewsService newsService)
        {
            _openWeatherMapService = openWeatherMapService;
            _ipGeolocationService = ipGeolocationService;
            _newsService = newsService;
        }

        /// <summary>
        /// Retrieves aggregated data (weather, news, geolocation) for a given city.
        /// </summary>
        /// <param name="city">The city name for which to retrieve data.</param>
        /// <returns>An ActionResult containing the aggregated data or an error message.</returns>
        [HttpGet("GetAggregatedData")]
        public async Task<ActionResult<AggregatedData>> GetAggregatedData(string city)
        {
            try
            {
                var newsTask = _newsService.GetNewsAsync(city).ConfigureAwait(false);
                var weatherTask = _openWeatherMapService.GetWeatherAsync(city).ConfigureAwait(false);
                var ipInfoTask = _ipGeolocationService.GetIpGeolocationAsync().ConfigureAwait(false);

                var news = await newsTask;
                var weatherData = await weatherTask;
                var ipData = await ipInfoTask;

                var aggregatedData = new AggregatedData
                {
                    WeatherInfo = weatherData,
                    IpInfo = ipData,
                    NewsArticles = news
                };

                return Ok(aggregatedData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
