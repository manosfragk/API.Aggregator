using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using API.Aggregator.Models;
using API.Aggregator.Interfaces;
using Newtonsoft.Json;

namespace API_Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the IpGeolocation API to retrieve geolocation information.
    /// </summary>
    public class IpGeolocationService : IIpGeolocationService
    {
        private readonly HttpClient _httpClient;
        private const string _ApiKey = "ddbc3af8c2074cc2b887ca9dd1732cdb";

        /// <summary>
        /// Constructor that injects the HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        public IpGeolocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Asynchronously retrieves geolocation information for the user's IP address.
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

                    IpGeolocationApiResponse? ipGeolocationApiResponse;
                    try
                    {
                        ipGeolocationApiResponse = JsonConvert.DeserializeObject<IpGeolocationApiResponse>(content);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error deserializing IpGeolocation API response: {ex.Message}");
                    }

                    if (ipGeolocationApiResponse == null)
                        return new IpGeolocationInfo();

                    var ipInfo = new IpGeolocationInfo
                    {
                        Ip = ipGeolocationApiResponse.Ip,
                        City = ipGeolocationApiResponse.City,
                    };

                    return ipInfo;
                }
                else
                {
                    throw new Exception($"IpGeolocation API request failed with status code: {response.StatusCode}");
                }
            }
        }
    }
}
