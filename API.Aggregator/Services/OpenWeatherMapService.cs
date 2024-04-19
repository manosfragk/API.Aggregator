using API.Aggregator.Helpers;
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Newtonsoft.Json;

namespace API_Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the OpenWeatherMap API to retrieve weather information.
    /// </summary>
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private const string _ApiKey = "API KEY";

        /// <summary>
        /// Constructor that injects the HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        public OpenWeatherMapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Asynchronously retrieves weather information for a given city.
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
                    OpenWeatherMapResponse? weatherMapResponse;
                    try
                    {
                        weatherMapResponse = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(content);
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception($"Error deserializing OpenWeatherMap API response: {ex.Message}");
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
                    throw new HttpRequestException($"OpenWeatherMap API request failed with status code {response.StatusCode}");
                }
            }
        }
    }
}
