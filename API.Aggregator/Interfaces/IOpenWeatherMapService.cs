using API.Aggregator.Models;

namespace API.Aggregator.Interfaces
{
    /// <summary>
    /// Interface that defines a service contract for retrieving weather information.
    /// </summary>
    public interface IOpenWeatherMapService
    {
        /// <summary>
        /// Asynchronously retrieves weather information for a given city.
        /// </summary>
        /// <param name="city">The city name for which to retrieve weather data.</param>
        /// <returns>A Task that resolves to a WeatherInfo object containing city, weather description, and temperature (in Celsius), or an empty object on error.</returns>
        Task<WeatherInfo> GetWeatherAsync(string city);
    }
}
