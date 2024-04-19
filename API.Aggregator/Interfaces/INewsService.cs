using API.Aggregator.Models;

namespace API.Aggregator.Interfaces
{
    /// <summary>
    /// Interface that defines a service contract for retrieving news articles.
    /// </summary>
    public interface INewsService
    {

        /// <summary>
        /// Asynchronously retrieves a list of news articles related to a given city.
        /// </summary>
        /// <param name="city">The city name for which to retrieve news articles.</param>
        /// <returns>A Task that resolves to a list of NewsArticle objects containing titles and URLs, or an empty list on error.</returns>
        Task<List<NewsArticle>> GetNewsAsync(string city);
    }
}
