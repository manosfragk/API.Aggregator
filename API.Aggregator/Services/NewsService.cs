using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace API.Aggregator.Services
{
    /// <summary>
    /// Service class for interacting with the News API to retrieve news articles for a given city.
    /// </summary>
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private const string _ApiKey = "a69e751f20a4435fa8847c76fdf9252b";

        /// <summary>
        /// Constructor that injects the HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Asynchronously retrieves a list of news articles related to the given city.
        /// </summary>
        /// <param name="city">The city name for which to retrieve news articles.</param>
        /// <returns>A list of NewsArticle objects containing titles and URLs, or an empty list on error.</returns>
        public async Task<List<NewsArticle>> GetNewsAsync(string city)
        {

            var parameters = new Dictionary<string, string>()
            {
                ["q"] = city,
                ["from"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ["sortBy"] = "popularity",
                ["pageSize"] = "4",
                ["apiKey"] = _ApiKey
            };

            var url = $"https://newsapi.org/v2/everything?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    NewsArticleResponse? newsResponse;
                    try
                    {
                        newsResponse = JsonConvert.DeserializeObject<NewsArticleResponse>(content);
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception($"Error deserializing News API response: {ex.Message}");
                    }

                    if (newsResponse == null)
                        return new List<NewsArticle>();

                    var newsArticles = new List<NewsArticle>();
                    newsArticles = newsResponse?.Articles?.Select(article => new NewsArticle
                    {
                        Title = article.Title,
                        Url = article.Url,
                    }).ToList() ?? new List<NewsArticle>();

                    return newsArticles;
                }
                else
                {
                    throw new HttpRequestException($"News API request failed with status code {response.StatusCode}");

                }
            }
        }
    }
}
