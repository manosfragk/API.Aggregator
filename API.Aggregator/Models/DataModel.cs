namespace API.Aggregator.Models
{

    #region News

    /// <summary>
    /// Represents a news article with title and URL.
    /// </summary>
    public class NewsArticle
    {
        /// <summary>
        /// Gets or sets the title of the news article.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Gets or sets the URL of the news article.
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// Gets or sets the LastUpdated of the news article.
        /// </summary>
        public DateTime LastUpdated {  get; set; }
    }

    /// <summary>
    /// Represents the response object from the News API.
    /// </summary>
    public class NewsArticleResponse
    {
        /// <summary>
        /// Gets or sets the status of the News API response.
        /// </summary>
        public string Status { get; set; } = "";

        /// <summary>
        /// Gets or sets a list of news articles returned by the News API.
        /// </summary>
        public List<NewsArticle> Articles { get; set; } = new List<NewsArticle>();
    }

    #endregion

    #region Weather

    /// <summary>
    /// Represents the response object from the OpenWeatherMap API.
    /// </summary>
    public class OpenWeatherMapResponse
    {
        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets a list of weather descriptions.
        /// </summary>
        public List<Weather> Weather { get; set; } = new List<Weather>();

        /// <summary>
        /// Gets or sets the main weather data.
        /// </summary>
        public Main Main { get; set; } = new Main();
    }

    /// <summary>
    /// Represents weather information.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// Gets or sets the weather description.
        /// </summary>
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Represents core weather data like temperature.
    /// </summary>
    public class Main
    {
        /// <summary>
        /// Gets or sets the temperature in Kelvin.
        /// </summary>
        public double Temp { get; set; }
    }

    /// <summary>
    /// Represents processed weather information with city, temperature (in Celsius), and description.
    /// </summary>
    public class WeatherInfo
    {
        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// Gets or sets the temperature in Celsius (converted from Kelvin).
        /// </summary>
        public double Temperature { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the weather description.
        /// </summary>
        public string Description { get; set; } = "";
    }

    #endregion

    #region IpGeolocation

    /// <summary>
    /// Represents the response object from the IpGeolocation API.
    /// </summary>
    public class IpGeolocationApiResponse
    {
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string Ip { get; set; } = "";

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string City { get; set; } = "";
    }

    /// <summary>
    /// Represents geolocation information for an IP address.
    /// </summary>
    public class IpGeolocationInfo
    {
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string Ip { get; set; } = "";

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string City { get; set; } = "";
    }

    #endregion

    #region Aggregation

    /// <summary>
    /// Represents the combined data from weather, news, and geolocation APIs.
    /// </summary>
    public class AggregatedData
    {
        /// <summary>
        /// Gets or sets a list of news articles.
        /// </summary>
        public List<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

        /// <summary>
        /// Gets or sets the weather information.
        /// </summary>
        public WeatherInfo WeatherInfo { get; set; } = new WeatherInfo();

        /// <summary>
        /// Gets or sets the geolocation information.
        /// </summary>
        public IpGeolocationInfo IpInfo { get; set; } = new IpGeolocationInfo();
    }

    #endregion
}
