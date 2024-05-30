using API.Aggregator.Models;

namespace API.Aggregator.Interfaces
{
    /// <summary>
    /// Interface that defines a service contract for retrieving geolocation information.
    /// </summary>
    public interface IIpGeolocationService
    {
        /// <summary>
        /// Asynchronously retrieves geolocation information for the user's IP address.
        /// </summary>
        Task<IAggregatorService> GetIpGeolocationDataAsync(string city);
    }
}
