using API.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Aggregator.Tests.Models
{
    /// <summary>
    /// Unit tests for the data classes used in the API aggregation project.
    /// </summary>
    public class DataModelTests
    {
        [Fact]
        public void NewsArticle_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var newsArticle = new NewsArticle();

            // Act
            newsArticle.Title = "Test Article";
            newsArticle.Url = "https://www.example.com/article";

            // Assert
            Assert.Equal("Test Article", newsArticle.Title);
            Assert.Equal("https://www.example.com/article", newsArticle.Url);
        }

        [Fact]
        public void NewsArticleResponse_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var newsResponse = new NewsArticleResponse();

            // Act
            newsResponse.Status = "ok";
            newsResponse.Articles.Add(new NewsArticle { Title = "Sample News", Url = "https://sample.com" });

            // Assert
            Assert.Equal("ok", newsResponse.Status);
            Assert.Single(newsResponse.Articles);
            Assert.Equal("Sample News", newsResponse.Articles[0].Title);
        }

        [Fact]
        public void Weather_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var weather = new Weather();

            // Act
            weather.Description = "clear sky";

            // Assert
            Assert.Equal("clear sky", weather.Description);
        }

        [Fact]
        public void Main_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var main = new Main();

            // Act
            main.Temp = 298.15;

            // Assert
            Assert.Equal(298.15, main.Temp);
        }

        [Fact]
        public void WeatherInfo_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var weatherInfo = new WeatherInfo();

            // Act
            weatherInfo.City = "Athens";
            weatherInfo.Temperature = 25.0;
            weatherInfo.Description = "sunny";

            // Assert
            Assert.Equal("Athens", weatherInfo.City);
            Assert.Equal(25.0, weatherInfo.Temperature);
            Assert.Equal("sunny", weatherInfo.Description);
        }

        [Fact]
        public void IpGeolocationApiResponse_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var ipGeolocationApiResponse = new IpGeolocationApiResponse();

            // Act
            ipGeolocationApiResponse.Ip = "8.8.8.8";
            ipGeolocationApiResponse.City = "Larisa";

            // Assert
            Assert.Equal("8.8.8.8", ipGeolocationApiResponse.Ip);
            Assert.Equal("Larisa", ipGeolocationApiResponse.City);
        }

        [Fact]
        public void IpGeolocationInfo_Properties_SetAndGetCorrectly()
        {
            // Arrange
            var ipGeolocationInfo = new IpGeolocationInfo();

            // Act
            ipGeolocationInfo.Ip = "1.1.1.1";
            ipGeolocationInfo.City = "London";

            // Assert
            Assert.Equal("1.1.1.1", ipGeolocationInfo.Ip);
            Assert.Equal("London", ipGeolocationInfo.City);
        }
    }
}
