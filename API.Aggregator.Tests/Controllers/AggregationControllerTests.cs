using API.Aggregator.Controllers;
using API.Aggregator.Interfaces;
using API.Aggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.Aggregator.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the AggregationController class.
    /// </summary>
    public class AggregationControllerTests
    {
        private Mock<IOpenWeatherMapService> _mockOpenWeatherMapService;
        private Mock<IIpGeolocationService> _mockIpGeolocationService;
        private Mock<INewsService> _mockNewsService;
        private Mock<ILogger<AggregationController>> _mockLogger;

        /// <summary>
        /// Initializes a new instance of the AggregationControllerTests class.
        /// </summary>
        public AggregationControllerTests()
        {
            _mockOpenWeatherMapService = new Mock<IOpenWeatherMapService>();
            _mockIpGeolocationService = new Mock<IIpGeolocationService>();
            _mockNewsService = new Mock<INewsService>();
            _mockLogger = new Mock<ILogger<AggregationController>>();
        }

        /// <summary>
        /// Tests that GetAggregatedData returns an Ok response with the aggregated data on a successful response from all dependent services.
        /// </summary>
        [Fact]
        public async Task GetAggregatedData_ReturnsOk_OnSuccessfulResponse()
        {
            // Arrange
            var mockWeatherData = new WeatherInfo { City = "Athens", Description = "sunny", Temperature = 25 };
            var mockIpInfo = new IpGeolocationInfo { Ip = "8.8.8.8", City = "Larisa" };
            var mockNewsArticles = new List<NewsArticle>() { new NewsArticle { Title = "Sample news article 1", Url = "https://www.example.com/article1" } };

            _mockOpenWeatherMapService.Setup(s => s.GetWeatherAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(mockWeatherData));
            _mockIpGeolocationService.Setup(s => s.GetIpGeolocationAsync())
                .Returns(Task.FromResult(mockIpInfo));
            _mockNewsService.Setup(s => s.GetNewsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(mockNewsArticles));

            var controller = new AggregationController(_mockOpenWeatherMapService.Object, _mockIpGeolocationService.Object, _mockNewsService.Object);

            // Act
            var result = await controller.GetAggregatedData("London");

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var aggregatedData = okObjectResult.Value as AggregatedData;
            Assert.NotNull(aggregatedData);
            Assert.Equal(mockWeatherData, aggregatedData.WeatherInfo);
            Assert.Equal(mockIpInfo, aggregatedData.IpInfo);
            Assert.Equal(mockNewsArticles, aggregatedData.NewsArticles);
        }

        /// <summary>
        /// Tests that GetAggregatedData returns an InternalServerError status code on an exception from any dependent service.
        /// </summary>
        [Fact]
        public async Task GetAggregatedData_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockOpenWeatherMapService.Setup(s => s.GetWeatherAsync(It.IsAny<string>()))
                .Throws(new Exception("Simulated exception"));
            _mockIpGeolocationService.Setup(s => s.GetIpGeolocationAsync())
                .Returns(Task.FromResult(new IpGeolocationInfo()));
            _mockNewsService.Setup(s => s.GetNewsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<NewsArticle>()));

            var controller = new AggregationController(_mockOpenWeatherMapService.Object, _mockIpGeolocationService.Object, _mockNewsService.Object);

            // Act
            var result = await controller.GetAggregatedData("New York");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode?)objectResult.StatusCode);
        }
    }
}
